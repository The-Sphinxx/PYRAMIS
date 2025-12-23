using MediatR;

public class RegisterClientCommand : IRequest<AuthResponseDto>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Nationality { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string? ProfileImage { get; set; }
}

public class RegisterClientCommandHandler(IIdentityService identityService) : IRequestHandler<RegisterClientCommand, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(RegisterClientCommand request, CancellationToken cancellationToken)
    {
        return await identityService.RegisterAsync(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName,
            request.Gender,
            request.Nationality,
            request.DateOfBirth,
            request.ProfileImage
        );
    }
}
