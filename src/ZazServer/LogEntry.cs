using System;
using Zaz.Server.Advanced;

namespace Zaz.Server
{
    public class LogEntry 
    {
        public LogEntrySeverity Severity {get;set;}
        public DateTime Timestamp {get;set;}        
        public string Message {get;set;}
        public string[] Tags { get; set; }
    }
}