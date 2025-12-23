using MediatR;

public class CreateAdminCommand : IRequest<object>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class CreateAdminCommandHandler(IIdentityService identityService) : IRequestHandler<CreateAdminCommand, object>
{
    public async Task<object> Handle(CreateAdminCommand request, CancellationToken cancellationToken)
    {
        var (id, email) = await identityService.CreateAdminAsync(request.Email, request.Password, request.FirstName, request.LastName);
        return new { id, email };
    }
}
