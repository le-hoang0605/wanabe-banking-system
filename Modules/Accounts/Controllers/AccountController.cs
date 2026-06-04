using Accounts.DTOs;
using Accounts.Models;
using Accounts.Models.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Accounts.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IAccountManagementContext _context;
    public AccountController(IAccountManagementContext context)
    {
        _context = context;
    }
    [HttpGet]
    public IActionResult Test()
    {
        return Ok("safe and sound");
    }

    [HttpGet("{accountNumber}")]
    public async Task<IActionResult> GetByAccountNumber(string accountNumber)
    {
        var account = await _context.Accounts
            .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);

        if (account == null)
        {
            return NotFound(new { message = $"Account number {accountNumber} not found." });
        }

        return Ok(account);
    }

    // 3. Get all accounts belonging to a specific customer (PartyId)
    [HttpGet("party/{partyId}")]
    public async Task<IActionResult> GetByPartyId(Guid partyId)
    {
        var accounts = await _context.Accounts
            .Where(a => a.PartyId == partyId)
            .ToListAsync();

        return Ok(accounts);
    }

    // 4. Open a new banking account
    [HttpPost]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountDto dto)
    {
        // Generates a unique 9-digit account number sequence
        string generatedAccountNumber = new Random().Next(100000000, 999999999).ToString();

        var newAccount = new Account
        {
            AccountId = Guid.NewGuid(),
            PartyId = dto.PartyId,
            AccountNumber = generatedAccountNumber,
            Balance = dto.InitialBalance,
            Currency = dto.Currency,
            Status = Status.Active,
            Role = dto.IsManager ? Role.Manager : Role.User,
            CreatedAt = DateTime.UtcNow
        };

        // If there is an initial deposit, append it immediately to the internal ledger
        if (dto.InitialBalance > 0)
        {
            var initialLedger = new AccountLedgerEntry
            {
                EntryId = Guid.NewGuid(),
                AccountNumber = generatedAccountNumber,
                Type = EntryType.Credit,
                Amount = dto.InitialBalance,
                TransactionId = Guid.NewGuid(), // System-generated transaction ID for initialization
                Description = "Initial balance deposit setup upon account opening.",
                CreatedAt = DateTime.UtcNow
            };
            _context.AccountLedgerEntries.Add(initialLedger);
        }

        _context.Accounts.Add(newAccount);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetByAccountNumber), new { accountNumber = newAccount.AccountNumber }, newAccount);
    }

    // 5. Append ledger entry (Ledger-First design rule)
    [HttpPost("post-entry")]
    public async Task<IActionResult> PostLedgerEntry([FromBody] AccountPostingDto dto)
    {
        var account = await _context.Accounts
            .FirstOrDefaultAsync(a => a.AccountNumber == dto.AccountNumber);

        if (account == null)
            return NotFound(new { message = "Bank account number does not exist." });

        if (account.Status == Status.Suspended)
            return BadRequest(new { message = "Account is suspended. Posting rejected." });

        // BUSINESS VALIDATION: Ensure sufficient balance for Debit (withdrawal) transactions
        if (dto.Type == EntryType.Debit && account.Balance < dto.Amount)
        {
            return BadRequest(new { message = "Transaction rejected. Insufficient account balance." });
        }

        // STEP 1: Append an immutable row into the internal transaction log ledger
        var ledgerEntry = new AccountLedgerEntry
        {
            EntryId = Guid.NewGuid(),
            AccountNumber = dto.AccountNumber,
            Type = dto.Type,
            Amount = dto.Amount,
            TransactionId = dto.TransactionId, // Cross-reference tracking with Transactions module
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow
        };
        _context.AccountLedgerEntries.Add(ledgerEntry);

        // STEP 2: Synchronize the Read-Cache Snapshot (Balance field)
        if (dto.Type == EntryType.Credit)
            account.Balance += dto.Amount;
        else
            account.Balance -= dto.Amount;

        account.UpdatedAt = DateTime.UtcNow;

        // STEP 3: Commit the entire unit of work transactionally to the database
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Ledger entry posted successfully.",
            currentBalance = account.Balance,
            entryId = ledgerEntry.EntryId
        });
    }

    // 6. Audit Endpoint: Re-calculates balance dynamically from all historical ledger rows
    [HttpGet("{accountNumber}/audit")]
    public async Task<IActionResult> AuditAccountBalance(string accountNumber)
    {
        var account = await _context.Accounts
            .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);

        if (account == null)
            return NotFound(new { message = "Account not found." });

        var entries = await _context.AccountLedgerEntries
            .Where(e => e.AccountNumber == accountNumber)
            .ToListAsync();

        double totalCredits = entries.Where(e => e.Type == EntryType.Credit).Sum(e => e.Amount);
        double totalDebits = entries.Where(e => e.Type == EntryType.Debit).Sum(e => e.Amount);
        double calculatedBalance = totalCredits - totalDebits;

        // Verify data integrity between the cached balance and historical logs
        bool isIntegrityValid = (Math.Abs(account.Balance - calculatedBalance) < 0.001);

        return Ok(new
        {
            accountNumber = accountNumber,
            cachedBalanceInTable = account.Balance,
            realCalculatedBalanceFromLedger = calculatedBalance,
            totalTransactionEntries = entries.Count,
            dataIntegrity = isIntegrityValid ? "SECURE (100% Match)" : "CRITICAL WARNING: UNAUTHORIZED TAMPERING DETECTED!"
        });
    }

    // 7. Freeze or Unfreeze a bank account
    [HttpPut("{accountNumber}/change-status")]
    public async Task<IActionResult> ChangeStatus(string accountNumber, [FromBody] Status newStatus)
    {
        var account = await _context.Accounts
            .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);

        if (account == null)
        {
            return NotFound(new { message = "Account not found to update status." });
        }

        account.Status = newStatus;
        account.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(new { message = $"Account status successfully updated to: {newStatus}" });
    }
}