using System;

namespace Zaz.Client.Avanced.Contract
{
    public class TraceEntry
    {
        public DateTime Timestamp { get; set; }
        public TraceSeverity Severity { get; set; }
        public string Message { get; set; }
        public string[] Tags { get; set; }
    }
}
