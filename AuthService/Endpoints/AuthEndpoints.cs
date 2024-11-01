using AuthService.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var key = Encoding.ASCII.GetBytes("memo_words_microservices_app_is_my_new_app"); // Use a secure key here

        app.MapPost("/register", async (User user, AuthDbContext db) =>
        {
            // Check if the username already exists
            if (await db.Users.AnyAsync(u => u.Username == user.Username))
            {
                return Results.BadRequest("Username already exists.");
            }

            // Check if the email already exists
            if (await db.Users.AnyAsync(u => u.Email == user.Email))
            {
                return Results.BadRequest("Email already exists.");
            }

            // Simple password hashing
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

            // Set the CreatedAt and UpdatedAt fields
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = null; // Not updated yet

            db.Users.Add(user);
            await db.SaveChangesAsync();
            return Results.Ok(user);
        });


        // Login User
        app.MapPost("/login", async (string username, string password, AuthDbContext db) =>
        {
            var user = await db.Users.SingleOrDefaultAsync(u => u.Username == username);
            if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return Results.Unauthorized();

            var tokenHandler = new JwtSecurityTokenHandler();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username, user.Id.ToString())
            };

            // Add role claims
            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = "memoword_issuer", // Your issuer
                Audience = "memoword_audience", // Your audience
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Results.Ok(new { Token = tokenHandler.WriteToken(token) });
        });


        // Google Login
        app.MapGet("/login/google", async (HttpContext httpContext) =>
        {
            var properties = new AuthenticationProperties { RedirectUri = "/auth/signin-google" };
            await httpContext.ChallengeAsync("Google", properties);
        });

        // Google Callback Handler
        app.MapGet("/auth/signin-google", async (HttpContext httpContext) =>
        {
            var result = await httpContext.AuthenticateAsync("Google");
            if (!result.Succeeded)
            {
                return Results.BadRequest("Google authentication failed.");
            }

            var claims = result.Principal.Claims.ToList();
            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            // Custom logic: check if user exists, create JWT, etc.
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.Name, name),
            new Claim(ClaimTypes.Email, email)
        }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Results.Ok(new { Token = tokenHandler.WriteToken(token) });
        });

        // Facebook Login
        app.MapGet("/login/facebook", async (HttpContext httpContext) =>
        {
            var properties = new AuthenticationProperties { RedirectUri = "/auth/signin-facebook" };
            await httpContext.ChallengeAsync("Facebook", properties);
        });

        // Facebook Callback Handler
        app.MapGet("/auth/signin-facebook", async (HttpContext httpContext) =>
        {
            var result = await httpContext.AuthenticateAsync("Facebook");
            if (!result.Succeeded)
            {
                return Results.BadRequest("Facebook authentication failed.");
            }

            var claims = result.Principal.Claims.ToList();
            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            // Custom logic: check if user exists, create JWT, etc.
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.Name, name),
            new Claim(ClaimTypes.Email, email)
        }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Results.Ok(new { Token = tokenHandler.WriteToken(token) });
        });

    }
}
