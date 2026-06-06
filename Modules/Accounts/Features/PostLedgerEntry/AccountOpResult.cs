namespace Accounts.Features.PostLedgerEntry;

// DTO kết quả trả ra ngoài cho Orchestrator
public record AccountOpResult(bool IsSuccess, string ErrorMessage);