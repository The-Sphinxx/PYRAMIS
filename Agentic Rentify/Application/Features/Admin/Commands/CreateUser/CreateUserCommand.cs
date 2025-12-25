using MediatR;

namespace Agentic_Rentify.Application.Features.Admin.Commands.CreateUser;

public class CreateUserCommand : IRequest<string>
{
    public string Email { get; set; } = string.Empty;
    public string? Password { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Phone { get; set; }
    public string? Nationality { get; set; }
    public string? ProfileImage { get; set; }
    public string? Role { get; set; }
}
