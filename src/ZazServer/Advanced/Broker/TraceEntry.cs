using System;

namespace Zaz.Server.Advanced.Broker
{
    public class TraceEntry 
    {
        public TraceSeverity Serverity {get;set;}
        public DateTime Timestamp {get;set;}        
        public string Message {get;set;}
        public string[] Tags { get; set; }
    }
}