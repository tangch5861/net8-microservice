namespace TodoService.Models
{
    public class LogEntry
    {
        public string Message { get; set; }
        public string LogLevel { get; set; }  // e.g., "Information", "Error", "Warning"
        public DateTime Timestamp { get; set; }
    }
}
