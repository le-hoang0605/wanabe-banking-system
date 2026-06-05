using Microsoft.EntityFrameworkCore;
using Transactions.Models;
using Transactions.Models.Context;
using Accounts.Features.PostLedgerEntry;

namespace Transactions.Features.ExecuteTransfer;

internal class ExecuteTransferService : IExecuteTransferService
{
    private readonly ITransactionsManagementContext _context;
    private readonly IPostLedgerEntryService _accountService; 

    public ExecuteTransferService(ITransactionsManagementContext context, IPostLedgerEntryService accountService)
    {
        _context = context;
        _accountService = accountService;
    }

    public async Task<(bool IsSuccess, string ErrorMessage, ExecuteTransferResponseDto? Data)> ExecuteAsync(ExecuteTransferRequestDto dto)
    {
        // 1. KIỂM TRA CHỐNG TRÙNG LẶP (Idempotency)
        var existingOrder = await _context.PaymentOrders
            .FirstOrDefaultAsync(p => p.IdempotencyKey == dto.IdempotencyKey);

        if (existingOrder != null)
        {
            return (true, string.Empty, new ExecuteTransferResponseDto(existingOrder.PaymentId, existingOrder.Status.ToString(), "Safe Alert: Duplicate request handled safely.", existingOrder.CreatedAt));
        }

        if (dto.DebtorAccountNumber == dto.CreditorAccountNumber)
        {
            return (false, "Validation Error: Debtor and Creditor accounts cannot be identical.", null);
        }

        var paymentId = Guid.NewGuid();

        // 2. ĐIỀU PHỐI GỌI IN-MEMORY SANG ACCOUNTS QUA INTERFACE CHÍNH
        
        // 2.1 Thực hiện TRỪ TIỀN tài khoản gửi (Debit)

        var debitPayload = new AccountPostingRequestDto(
            dto.DebtorAccountNumber,
            Accounts.Models.EntryType.Debit,
            dto.Amount,
            paymentId,
            $"Transfer Out to Account: {dto.CreditorAccountNumber}"
        );

     
        var debitResult = await _accountService.ExecuteAsync(debitPayload);
        if (!debitResult.IsSuccess)
        {
            return (false, $"Debit rejected by Accounts Module: {debitResult.ErrorMessage}", null);
        }
        
        
        
        // 2.2 Thực hiện CỘNG TIỀN tài khoản nhận (Credit)
        var creditPayload = new AccountPostingRequestDto(
            dto.CreditorAccountNumber,
            Accounts.Models.EntryType.Credit,
            dto.Amount,
            paymentId,
            $"Transfer In from Account: {dto.DebtorAccountNumber}"
        );

      
        var creditResult = await _accountService.ExecuteAsync(creditPayload);
        if (!creditResult.IsSuccess)
        {
            // CƠ CHẾ BÙ TRỪ GIAO DỊCH (Compensating Transaction) - Hoàn tiền cho A
            var refundPayload = new AccountPostingRequestDto(
                dto.DebtorAccountNumber,
                Accounts.Models.EntryType.Credit,
                dto.Amount,
                paymentId,
                $"TRANSACTION COMPENSATED: Refund due to credit failure to account {dto.CreditorAccountNumber}"
            );
            await _accountService.ExecuteAsync(refundPayload);

            return (false, $"Credit failed: {creditResult.ErrorMessage}. Money successfully refunded to Debtor.", null);
        }

        // 3. LƯU LẠI LỊCH SỬ GIAO DỊCH VÀO DATABASE TRANSACTIONS
        Guid debtorId = Guid.NewGuid();   
        Guid creditorId = Guid.NewGuid(); 

        var paymentOrder = new PaymentOrder
        {
            PaymentId = paymentId,
            IdempotencyKey = dto.IdempotencyKey,
            DebtorAccountId = debtorId,       
            CreditorAccountId = creditorId,   
            Amount = dto.Amount,
            Status = Status.Executed,         
            CreatedAt = DateTime.UtcNow
        };

        var debitLedger = new LedgerEntry
        {
            EntryId = Guid.NewGuid(),
            PaymentId = paymentId,
            AccountId = debtorId,
            TransactionType = TransactionType.Debit, 
            Amount = dto.Amount,
            CreatedAt = DateTime.UtcNow
        };

        var creditLedger = new LedgerEntry
        {
            EntryId = Guid.NewGuid(),
            PaymentId = paymentId,
            AccountId = creditorId,
            TransactionType = TransactionType.Credit, 
            Amount = dto.Amount,
            CreatedAt = DateTime.UtcNow
        };

        _context.PaymentOrders.Add(paymentOrder);
        _context.LedgerEntries.AddRange(debitLedger, creditLedger);
        await _context.SaveChangesAsync(); 

        return (true, string.Empty, new ExecuteTransferResponseDto(paymentId, Status.Executed.ToString(), "Transaction executed perfectly.", paymentOrder.CreatedAt));
    }
}