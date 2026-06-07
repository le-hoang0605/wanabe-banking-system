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
        public async Task<(bool IsValid, TransferResultDto? DuplicateResult, TransferSessionDto? Session)> PrepareWithdrawAsync(WithdrawRequestWithKeyDto request)
        {
            var existingOrder = await _context.PaymentOrders.AsNoTracking()
                .FirstOrDefaultAsync(p => p.IdempotencyKey == request.IdempotencyKey);

            if (existingOrder != null)
            {
                var duplicateResult = new TransferResultDto(
                    IsSuccess: existingOrder.Status == Status.Executed,
                    Message: $"Duplicate withdraw. Current status: {existingOrder.Status}",
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
                CreditorAccountId = Guid.Empty, // withdraw doesnt have creditor
                Amount = request.Amount,
                Status = Status.Processing,
                CreatedAt = DateTime.UtcNow
            };
            // we only have debitor
            var debitEntry = new LedgerEntry
            {
                EntryId = Guid.NewGuid(),
                PaymentId = order.PaymentId,
                AccountId = request.DebtorAccountId,
                TransactionType = TransactionType.Debit,
                Amount = request.Amount,
                CreatedAt = DateTime.UtcNow
            };

            _context.PaymentOrders.Add(order);
            _context.LedgerEntries.Add(debitEntry);
            await _context.SaveChangesAsync();

            var session = new TransferSessionDto(order.PaymentId, order.DebtorAccountId, order.CreditorAccountId, order.Amount);
            return (true, null, session);
        }
        public async Task<(bool IsValid, TransferResultDto? DuplicateResult, TransferSessionDto? Session)> PrepareDepositAsync(DepositRequestWithKeyDto request)
        {
            // check key
            var existingOrder = await _context.PaymentOrders.AsNoTracking()
                .FirstOrDefaultAsync(p => p.IdempotencyKey == request.IdempotencyKey);

            if (existingOrder != null)
            {
                var duplicateResult = new TransferResultDto(
                    IsSuccess: existingOrder.Status == Status.Executed,
                    Message: $"Duplicate deposit. Current status: {existingOrder.Status}",
                    PaymentId: existingOrder.PaymentId,
                    Status: existingOrder.Status.ToString()
                );
                return (false, duplicateResult, null);
            }

            //create deposit order
            var order = new PaymentOrder
            {
                PaymentId = Guid.NewGuid(),
                IdempotencyKey = request.IdempotencyKey,
                DebtorAccountId = Guid.Empty,      // we only have credit
                CreditorAccountId = request.CreditorAccountId,
                Amount = request.Amount,
                Status = Status.Processing,
                CreatedAt = DateTime.UtcNow
            };

            // we only have credit
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
            _context.LedgerEntries.Add(creditEntry);
            await _context.SaveChangesAsync();

            var session = new TransferSessionDto(order.PaymentId, order.DebtorAccountId, order.CreditorAccountId, order.Amount);
            return (true, null, session);
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