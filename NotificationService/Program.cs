using NotificationService.Models;
using NotificationService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add email configuration
builder.Configuration.AddJsonFile("appsettings.json");

// Register EmailService
builder.Services.AddSingleton<EmailService>();

var app = builder.Build();

// Endpoint to send email
app.MapPost("/send-email", async (EmailMessage emailMessage, EmailService emailService) =>
{
    await emailService.SendEmailAsync(emailMessage);
    return Results.Ok("Email sent successfully.");
});

app.Run();
