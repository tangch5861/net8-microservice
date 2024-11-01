using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        app.MapGet("/users", [Authorize] async (AuthDbContext db) =>
        {
            var users = await db.Users.ToListAsync();
            return Results.Ok(users);
        });

        app.MapGet("/users/{id}", [Authorize] async (Guid id, AuthDbContext db) =>
        {
            var user = await db.Users.FindAsync(id);
            return user is not null ? Results.Ok(user) : Results.NotFound();
        });

        app.MapDelete("/users/{id}", [Authorize] async (Guid id, AuthDbContext db) =>
        {
            var user = await db.Users.FindAsync(id);
            if (user is null) return Results.NotFound();

            db.Users.Remove(user);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        app.MapPut("/users/{id}/roles", async (Guid id, List<string> roles, AuthDbContext db) =>
        {
            var user = await db.Users.FindAsync(id);
            if (user is null)
            {
                return Results.NotFound();
            }

            user.Roles = roles; // Update the user's roles
            user.UpdatedAt = DateTime.UtcNow; // Update the updated timestamp
            await db.SaveChangesAsync();
            return Results.Ok(user);
        });
    }
}
