using System;

namespace Zaz.Server.Advanced.State
{
    public class TraceEntry 
    {
        public TraceKind Kind {get;set;}
        public DateTime Timestamp {get;set;}
        public TraceSeverity Severity {get;set;}
        public string Message {get;set;}
        public string[] Tags { get; set; }
    }
}