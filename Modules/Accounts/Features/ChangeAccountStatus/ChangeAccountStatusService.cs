using Accounts.Models.Context;
using Microsoft.EntityFrameworkCore;

namespace Accounts.Features.ChangeAccountStatus;

internal class ChangeAccountStatusService : IChangeAccountStatusService
{
    private readonly IAccountManagementContext _context;

    public ChangeAccountStatusService(IAccountManagementContext context)
    {
        _context = context;
    }

    public async Task<(bool IsSuccess, string? ErrorMessage, ChangeAccountStatusResponseDto? Data)> ExecuteAsync(string accountNumber, ChangeAccountStatusRequestDto dto)
    {
        var account = await _context.Accounts
            .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);

        if (account == null)
        {
            return (false, "Account not found to update status.", null);
        }

        // Cập nhật trạng thái mới và thời gian ghi nhận thay đổi
        account.Status = dto.NewStatus;
        account.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();

        var responseData = new ChangeAccountStatusResponseDto(
            $"Account status successfully updated to: {dto.NewStatus}",
            account.AccountNumber,
            account.Status,
            account.UpdatedAt.Value
        );

        return (true, null, responseData);
    }
}