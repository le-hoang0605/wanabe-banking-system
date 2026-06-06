using Accounts.Models;
using Accounts.Models.Context;

namespace Accounts.Features.CreateAccount;

internal class CreateAccountService : ICreateAccountService
{
    private readonly IAccountManagementContext _context;

    public CreateAccountService(IAccountManagementContext context)
    {
        _context = context;
    }

    public async Task<CreateAccountResponseDto> ExecuteAsync(CreateAccountRequestDto dto)
    {
        
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

       
        _context.Accounts.Add(newAccount);
        await _context.SaveChangesAsync();
        
        return new CreateAccountResponseDto(
            newAccount.AccountId,
            newAccount.AccountNumber,
            newAccount.Balance,
            newAccount.Currency,
            newAccount.Status.ToString()
        );
    }
}