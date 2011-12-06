using System;

namespace Zaz.Client
{
    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public LogEntrySeverity Severity { get; set; }
        public string Message { get; set; }
        public string[] Tags { get; set; }
    }
}
