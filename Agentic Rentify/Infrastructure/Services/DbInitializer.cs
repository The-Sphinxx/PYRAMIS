using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

// Seeder to ensure default roles and users exist at startup
public class DbInitializer(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    ILogger<DbInitializer> logger)
{
    public async Task SeedAsync()
    {
        try
        {
            // 1) Ensure roles exist
            string[] roles = ["Admin", "Client", "SuperAdmin"];
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var roleResult = await roleManager.CreateAsync(new IdentityRole(role));
                    if (!roleResult.Succeeded)
                    {
                        var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                        logger.LogError("Failed creating role {Role}: {Errors}", role, errors);
                    }
                    else
                    {
                        logger.LogInformation("Role {Role} created", role);
                    }
                }
            }

            // 2) Ensure Admin user exists
            const string adminEmail = "admin@rentify.com";
            const string adminPassword = "Admin@123!"; // Meets policy: upper, lower, digit, non-alpha, length
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser is null)
            {
                adminUser = new ApplicationUser
                {
                    Email = adminEmail,
                    UserName = adminEmail,
                    EmailConfirmed = true,
                    FirstName = "System",
                    LastName = "Admin",
                    IsVerified = true,
                    CreatedAt = DateTime.UtcNow
                };

                var createResult = await userManager.CreateAsync(adminUser, adminPassword);
                if (!createResult.Succeeded)
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    logger.LogError("Failed creating admin user: {Errors}", errors);
                }
                else
                {
                    logger.LogInformation("Admin user created: {Email}", adminEmail);
                }
            }

            // Ensure Admin is in role
            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                var addRoleResult = await userManager.AddToRoleAsync(adminUser, "Super Admin");
                if (!addRoleResult.Succeeded)
                {
                    var errors = string.Join(", ", addRoleResult.Errors.Select(e => e.Description));
                    logger.LogError("Failed assigning Admin role: {Errors}", errors);
                }
            }

            // 3) Ensure Client user exists
            const string clientEmail = "client@rentify.com";
            const string clientPassword = "Client@123!"; // Meets policy
            var clientUser = await userManager.FindByEmailAsync(clientEmail);
            if (clientUser is null)
            {
                clientUser = new ApplicationUser
                {
                    Email = clientEmail,
                    UserName = clientEmail,
                    EmailConfirmed = true,
                    FirstName = "Default",
                    LastName = "Client",
                    IsVerified = true,
                    CreatedAt = DateTime.UtcNow
                };

                var createResult = await userManager.CreateAsync(clientUser, clientPassword);
                if (!createResult.Succeeded)
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    logger.LogError("Failed creating default client: {Errors}", errors);
                }
                else
                {
                    logger.LogInformation("Client user created: {Email}", clientEmail);
                }
            }

            // Ensure Client in role
            if (!await userManager.IsInRoleAsync(clientUser, "Client"))
            {
                var addRoleResult = await userManager.AddToRoleAsync(clientUser, "Client");
                if (!addRoleResult.Succeeded)
                {
                    var errors = string.Join(", ", addRoleResult.Errors.Select(e => e.Description));
                    logger.LogError("Failed assigning Client role: {Errors}", errors);
                }
            }

            logger.LogInformation("Database seeding completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during database seeding.");
            throw;
        }
    }
}
