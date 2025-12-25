namespace Agentic_Rentify.Application.Features.Admin.DTOs;

public record UserResponseDto
{
    public string Id { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public string Nationality { get; init; } = string.Empty;
    public string? ProfileImage { get; init; }
    public bool IsVerified { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTimeOffset? LockoutEnd { get; init; }
    public string Role { get; init; } = "Client";
}
