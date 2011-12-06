using System;

namespace Zaz.Server.Advanced.State
{
    public class LogEntry 
    {
        public LogEntryKind Kind {get;set;}
        public DateTime Timestamp {get;set;}
        public LogEntrySeverity Severity {get;set;}
        public string Message {get;set;}
        public string[] Tags { get; set; }
    }
}