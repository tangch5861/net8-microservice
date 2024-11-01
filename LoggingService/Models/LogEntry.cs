namespace LoggingService.Models
{
    public class LogEntry
    {
        public int Id { get; set; }
        public string Level { get; set; } // e.g., Information, Error, Warning
        public string Message { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string ApplicationName { get; set; }
        public string Details { get; set; } // Optional for detailed logs
    }
}
