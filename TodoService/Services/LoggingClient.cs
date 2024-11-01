using TodoService.Models;
using System.Net.Http.Json;

namespace TodoService.Services
{
    public class LoggingClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _loggingServiceUrl;

        public LoggingClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _loggingServiceUrl = configuration["LoggingServiceUrl"];  // Set the Logging Service URL in appsettings.json
        }

        public async Task LogAsync(string message, string logLevel)
        {
            var logEntry = new LogEntry
            {
                Message = message,
                LogLevel = logLevel,
                Timestamp = DateTime.UtcNow
            };

            var response = await _httpClient.PostAsJsonAsync($"{_loggingServiceUrl}/log", logEntry);

            if (!response.IsSuccessStatusCode)
            {
                // Handle failed log entry submission (optional)
                Console.WriteLine($"Failed to send log entry: {response.StatusCode}");
            }
        }
    }
}
