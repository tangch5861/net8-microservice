using Microsoft.EntityFrameworkCore;
using UserProfileService.Data;
using UserProfileService.Models;
using JwtAuthLibrary;
using Microsoft.OpenApi.Models;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// Add JWT Authentication using the shared library
builder.Services.AddJwtAuthentication(
    issuer: "memoword_issuer",
    audience: "memoword_audience",
    secretKey: "memo_words_microservices_app_is_my_new_app"
);

// Add Authorization service (Fix for your error)
builder.Services.AddAuthorization();  // Required for enforcing policies or roles

// Add UserProfileDbContext with PostgreSQL connection
services.AddDbContext<UserProfileDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));

// Add HttpClient for external API calls (e.g., AuthService)
services.AddHttpClient("AuthService", client =>
{
    client.BaseAddress = new Uri("https://localhost:5001/");
    client.Timeout = TimeSpan.FromSeconds(30); // Set a reasonable timeout for requests
});

// Add Swagger for API documentation
services.AddEndpointsApiExplorer();
// Add Swagger services with JWT configuration
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "UserProfileService API", Version = "v1" });

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();  // Now authorization is properly registered and used

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Secure all endpoints to require authenticated users
// Get all user profiles
app.MapGet("/userprofiles", async (UserProfileDbContext db) =>
    await db.UserProfiles.ToListAsync())
    .RequireAuthorization();  // Ensure only authenticated users can access

// Get user profile by ID
app.MapGet("/userprofiles/{id}", async (UserProfileDbContext db, Guid id) =>
{
    return await db.UserProfiles.FindAsync(id) is UserProfile userProfile
        ? Results.Ok(userProfile)
        : Results.NotFound();
}).RequireAuthorization(); // Require authentication

// Create new user profile, verifying with AuthService
app.MapPost("/userprofiles", async (IHttpClientFactory httpClientFactory, UserProfileDbContext db, UserProfile userProfile, HttpContext context) =>
{
    var httpClient = httpClientFactory.CreateClient("AuthService");

    var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);
    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


    // Verify user existence in AuthService
    var response = await httpClient.GetAsync($"users/{userProfile.Id}");

    if (response.IsSuccessStatusCode)
    {
        db.UserProfiles.Add(userProfile);
        await db.SaveChangesAsync();
        return Results.Created($"/userprofiles/{userProfile.Id}", userProfile);
    }

    var errorMessage = await response.Content.ReadAsStringAsync();
    return Results.BadRequest($"User verification failed: {errorMessage}");
}).RequireAuthorization(); // Require authentication

// Update existing user profile
app.MapPut("/userprofiles/{id}", async (UserProfileDbContext db, Guid id, UserProfile updatedUserProfile) =>
{
    var userProfile = await db.UserProfiles.FindAsync(id);

    if (userProfile is null) return Results.NotFound();

    // Update user profile fields
    userProfile.Name = updatedUserProfile.Name;
    userProfile.Address = updatedUserProfile.Address;
    userProfile.Email = updatedUserProfile.Email;
    userProfile.Phone = updatedUserProfile.Phone;

    await db.SaveChangesAsync();
    return Results.Ok(userProfile);
}).RequireAuthorization(); // Require authentication

// Delete user profile
app.MapDelete("/userprofiles/{id}", async (UserProfileDbContext db, Guid id) =>
{
    var userProfile = await db.UserProfiles.FindAsync(id);

    if (userProfile is null) return Results.NotFound();

    db.UserProfiles.Remove(userProfile);
    await db.SaveChangesAsync();

    return Results.NoContent();
}).RequireAuthorization(); // Require authentication

app.Run();
