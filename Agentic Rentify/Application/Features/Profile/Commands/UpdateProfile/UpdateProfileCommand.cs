using Agentic_Rentify.Application.Interfaces;
using MediatR;

namespace Agentic_Rentify.Application.Features.Profile.Commands.UpdateProfile;

public class UpdateProfileCommand : IRequest<Unit>
{
    public string UserId { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Phone { get; set; }
    public string? Nationality { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? ProfileImage { get; set; }
}

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, Unit>
{
    private readonly IIdentityService _identityService;

    public UpdateProfileCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Unit> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _identityService.GetByIdAsync(request.UserId);
        
        if (user == null)
        {
            throw new Agentic_Rentify.Core.Exceptions.NotFoundException($"User with ID {request.UserId} not found");
        }

        // Update user properties
        if (!string.IsNullOrWhiteSpace(request.FirstName))
            user.FirstName = request.FirstName;
            
        if (!string.IsNullOrWhiteSpace(request.LastName))
            user.LastName = request.LastName;
            
        if (request.Phone != null)
            user.PhoneNumber = request.Phone;
            
        if (request.Nationality != null)
            user.Nationality = request.Nationality;
            
        if (request.DateOfBirth.HasValue)
            user.DateOfBirth = request.DateOfBirth.Value;
            
        if (request.Gender != null)
            user.Gender = request.Gender;
            
        if (request.ProfileImage != null)
            user.ProfileImage = request.ProfileImage;

        await _identityService.UpdateUserAsync(user);

        return Unit.Value;
    }
}
