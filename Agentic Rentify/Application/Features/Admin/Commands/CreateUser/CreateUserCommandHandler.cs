using Agentic_Rentify.Core.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Agentic_Rentify.Application.Features.Admin.Commands.CreateUser;

public class CreateUserCommandHandler(UserManager<ApplicationUser> userManager) 
    : IRequestHandler<CreateUserCommand, string>
{
    public async Task<string> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new BadRequestException("A user with this email already exists.");
        }

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName ?? string.Empty,
            LastName = request.LastName ?? string.Empty,
            PhoneNumber = request.Phone,
            Nationality = request.Nationality ?? string.Empty,
            ProfileImage = request.ProfileImage,
            IsVerified = true,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(user, request.Password ?? "DefaultPassword123!");
        
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new BadRequestException($"User creation failed: {errors}");
        }

        // Assign role if provided, default to Client
        var role = string.IsNullOrWhiteSpace(request.Role) ? "Client" : request.Role;
        await userManager.AddToRoleAsync(user, role);

        return user.Id;
    }
}
