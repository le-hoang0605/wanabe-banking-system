using System.ComponentModel.DataAnnotations;

namespace wanabe_banking_system.UseCases.RegisterOrchestrator;

public record RegisterOchestratorRequestDto
(
    [Required] string FullName,
    [Required] string Email,
    [Required] string Password
    );