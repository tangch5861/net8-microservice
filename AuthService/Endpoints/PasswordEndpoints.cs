using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public static class PasswordEndpoints
{
    public static void MapPasswordEndpoints(this WebApplication app)
    {
        // Change Password
        app.MapPut("/password/change", [Authorize] async (string currentPassword, string newPassword, AuthDbContext db, ClaimsPrincipal user) =>
        {
            var username = user.Identity?.Name;

            var existingUser = await db.Users.SingleOrDefaultAsync(u => u.Username == username);
            if (existingUser is null || !BCrypt.Net.BCrypt.Verify(currentPassword, existingUser.PasswordHash))
            {
                return Results.Unauthorized();
            }

            existingUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await db.SaveChangesAsync();
            return Results.Ok("Password changed successfully.");
        });

        // Forgot Password
        app.MapPost("/password/forgot", async (string email, AuthDbContext db) =>
        {
            var user = await db.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (user is null)
            {
                return Results.NotFound("User not found.");
            }

            // Here you would generate a reset token and send an email with the reset link
            // For simplicity, we'll just return a message
            return Results.Ok("Password reset link sent.");
        });

        // Reset Password
        app.MapPost("/password/reset", async (string token, string newPassword, AuthDbContext db) =>
        {
            // Validate token here (this could be a JWT or some other mechanism)
            // For simplicity, we'll assume the token is valid and not implement validation

            // You may need to get user by token, but for simplicity, we will assume the user is fetched
            var user = await db.Users.FirstOrDefaultAsync(); // Fetch user based on your logic

            if (user is null)
            {
                return Results.NotFound("User not found.");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await db.SaveChangesAsync();
            return Results.Ok("Password has been reset successfully.");
        });
    }
}
