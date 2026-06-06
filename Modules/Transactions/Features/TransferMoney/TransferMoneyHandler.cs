using Microsoft.EntityFrameworkCore;
using Transactions.DTOs;
using Transactions.Models;
using Transactions.Models.Context;

namespace Transactions.Features.TransferMoney
{
    internal class TransferMoneyHandler : ITransferMoneyHandler
    {
        private readonly TransactionManagementContext _context;

        public TransferMoneyHandler(TransactionManagementContext context)
        {
            _context = context;
        }

        public async Task<(bool IsValid, TransferResultDto? DuplicateResult, TransferSessionDto? Session)> PrepareTransferAsync(TransferRequestWithKeyDto request)
        {
            var existingOrder = await _context.PaymentOrders.AsNoTracking()
                .FirstOrDefaultAsync(p => p.IdempotencyKey == request.IdempotencyKey);

            if (existingOrder != null)
            {
                var duplicateResult = new TransferResultDto(
                    IsSuccess: existingOrder.Status == Status.Executed,
                    Message: $"Duplicate transaction. Current status: {existingOrder.Status}",
                    PaymentId: existingOrder.PaymentId,
                    Status: existingOrder.Status.ToString()
                );
                return (false, duplicateResult, null);
            }
            var order = new PaymentOrder
            {
                PaymentId = Guid.NewGuid(),
                IdempotencyKey = request.IdempotencyKey,
                DebtorAccountId = request.DebtorAccountId,
                CreditorAccountId = request.CreditorAccountId,
                Amount = request.Amount,
                Status = Status.Processing,
                CreatedAt = DateTime.UtcNow
            };
            var debitEntry = new LedgerEntry
            {
                EntryId = Guid.NewGuid(),
                PaymentId = order.PaymentId,
                AccountId = request.DebtorAccountId,
                TransactionType = TransactionType.Debit,
                Amount = request.Amount,
                CreatedAt = DateTime.UtcNow
            };

            var creditEntry = new LedgerEntry
            {
                EntryId = Guid.NewGuid(),
                PaymentId = order.PaymentId,
                AccountId = request.CreditorAccountId,
                TransactionType = TransactionType.Credit,
                Amount = request.Amount,
                CreatedAt = DateTime.UtcNow
            };

            _context.PaymentOrders.Add(order);
            _context.LedgerEntries.AddRange(debitEntry, creditEntry);
            await _context.SaveChangesAsync();
            var session = new TransferSessionDto(order.PaymentId, order.DebtorAccountId, order.CreditorAccountId, order.Amount);

            return (true, null, session);
        }

        public async Task ConfirmTransferAsync(Guid paymentId, bool isSuccess, string errorMessage)
        {
            var localOrder = await _context.PaymentOrders.FirstOrDefaultAsync(o => o.PaymentId == paymentId);
            if (localOrder == null) return;
            localOrder.Status = isSuccess ? Status.Executed : Status.Failed;
            await _context.SaveChangesAsync();
        }
    }
}