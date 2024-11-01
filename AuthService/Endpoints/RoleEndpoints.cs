using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

public static class RoleEndpoints
{
    public static void MapRoleEndpoints(this WebApplication app)
    {
        // Get User Roles
        app.MapGet("/users/{id}/roles", [Authorize(Roles = "Admin")] async (Guid id, AuthDbContext db) =>
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);
            return user is not null ? Results.Ok(user.Roles) : Results.NotFound("User not found.");
        });

        // Assign Role to User
        app.MapPost("/users/{id}/roles", [Authorize(Roles = "Admin")] async (Guid id, string role, AuthDbContext db) =>
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user is null)
            {
                return Results.NotFound("User not found.");
            }

            // Ensure roles is initialized and add role to user
            if (user.Roles == null)
            {
                user.Roles = new List<string>(); // Initialize if null
            }

            if (!user.Roles.Contains(role))
            {
                user.Roles.Add(role); // Add role to user's roles
                await db.SaveChangesAsync();
            }

            return Results.Ok("Role assigned successfully.");
        });

        // Remove Role from User
        app.MapDelete("/users/{id}/roles/{role}", [Authorize(Roles = "Admin")] async (Guid id, string role, AuthDbContext db) =>
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user is null)
            {
                return Results.NotFound("User not found.");
            }

            // Check if the role exists and remove it
            if (user.Roles != null && user.Roles.Contains(role))
            {
                user.Roles.Remove(role); // Remove role from user's roles
                await db.SaveChangesAsync();
            }

            return Results.Ok("Role removed successfully.");
        });
    }
}
