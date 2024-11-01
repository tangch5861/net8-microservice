using LoggingService.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Sinks.PostgreSQL;

var builder = WebApplication.CreateBuilder(args);

// Configure PostgreSQL with Entity Framework Core
builder.Services.AddDbContext<LoggingDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));

// Configure Serilog for logging to PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("PostgresConnection");

builder.Host.UseSerilog((context, services, configuration) => configuration
    .WriteTo.Console()  // Logs to console
    .WriteTo.PostgreSQL(
        connectionString,
        "LogEntries",  // PostgreSQL table for logs
        needAutoCreateTable: true  // Automatically create table if not exists
    ));

// Build the app
var app = builder.Build();

// Manual logging endpoint
app.MapPost("/log", async (LogEntry logEntry, LoggingDbContext db) =>
{
    logEntry.Timestamp = DateTime.UtcNow;  // Set log timestamp
    db.LogEntries.Add(logEntry);
    await db.SaveChangesAsync();
    return Results.Ok(logEntry);
});

// Default GET endpoint to check if the service is running
app.MapGet("/", () => "Logging Service is up and running.");

// Run the app
app.Run();
