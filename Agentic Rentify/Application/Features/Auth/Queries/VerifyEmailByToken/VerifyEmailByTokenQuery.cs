using MediatR;

public record VerifyEmailByTokenQuery(string Email, string Token) : IRequest<bool>;

public class VerifyEmailByTokenQueryHandler(IIdentityService identityService) : IRequestHandler<VerifyEmailByTokenQuery, bool>
{
    public async Task<bool> Handle(VerifyEmailByTokenQuery request, CancellationToken cancellationToken)
    {
        return await identityService.ConfirmEmailByTokenAsync(request.Email, request.Token);
    }
}
