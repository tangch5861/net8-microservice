using Serilog;
using App.Metrics;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog for logging
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

// Register Serilog globally
builder.Host.UseSerilog();

// Add App.Metrics services (correct way)
//builder.Services.AddMetrics(); // No need for explicit namespace
builder.Services.AddMetricsTrackingMiddleware();
builder.Services.AddMetricsEndpoints(); // Enables metrics endpoints like /metrics

// Add support for controllers
builder.Services.AddControllers();

var app = builder.Build();

// Middleware to log requests using Serilog
app.UseSerilogRequestLogging();

// Enable App Metrics middleware
app.UseMetricsAllMiddleware(); // Adds metrics tracking to all middleware (requests, errors, etc.)

// Optionally enable metrics endpoints (e.g., for Prometheus scraping)
app.UseMetricsEndpoint();

// Map controllers
app.MapControllers();

// Run the application
app.Run();
