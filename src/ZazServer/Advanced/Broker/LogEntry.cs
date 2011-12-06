using System;

namespace Zaz.Server.Advanced.Broker
{
    public class LogEntry 
    {
        public LogEntrySeverity Severity {get;set;}
        public DateTime Timestamp {get;set;}        
        public string Message {get;set;}
        public string[] Tags { get; set; }
    }
}