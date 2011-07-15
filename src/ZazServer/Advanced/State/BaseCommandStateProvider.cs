using System;
using System.Linq;

namespace Zaz.Server.Advanced.State
{
    public abstract class BaseCommandStateProvider : ICommandStateProvider
    {				
        public void Start(string key, DateTime timestamp)
        {			
            WriteEntry(key, new TraceEntry
                                {
                                    Kind = TraceKind.Start,
                                    Timestamp = timestamp
                                });
        }
		
        public void WriteTrace(string key, DateTime timestamp, TraceSeverity severity, string message)
        {
            WriteEntry(key, new TraceEntry
                                {
                                    Kind = TraceKind.Trace,
                                    Timestamp = timestamp,
                                    Severity = severity,
                                    Message = message
                                });
        }
		
        public void CompleteSuccess(string key, DateTime timestamp)
        {
            WriteEntry(key, new TraceEntry
                                {
                                    Kind = TraceKind.Success,
                                    Timestamp = timestamp													
                                });
        }
		
        public void CompleteFailure(string key, DateTime timestamp, string reason)
        {
            WriteEntry(key, new TraceEntry
                                {
                                    Kind = TraceKind.Failure,
                                    Timestamp = timestamp													
                                });
        }
		
        public abstract IQueryable<TraceEntry> QueryEntries(string key);
		
        protected abstract void WriteEntry(string key, TraceEntry entry);
    }
}