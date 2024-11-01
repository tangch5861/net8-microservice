using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

// Add memory cache
builder.Services.AddMemoryCache();

// Configure rate limiting
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("RateLimiting"));
builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// Add authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "memoword_issuer",
        ValidAudience = "memoword_audience",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("memo_words_microservices_app_is_my_new_app"))
    };
});

// Add YARP services
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// Enable IP Rate Limiting
app.UseIpRateLimiting();

// Use authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

// Use caching middleware
app.UseResponseCaching();

// Use routing for the reverse proxy
app.MapReverseProxy();

app.Run();
