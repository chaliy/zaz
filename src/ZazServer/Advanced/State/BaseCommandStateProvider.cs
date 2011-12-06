using System;
using System.Linq;

namespace Zaz.Server.Advanced.State
{
    public abstract class BaseCommandStateProvider : ICommandStateProvider
    {				
        public void Start(string key, DateTime timestamp)
        {			
            WriteEntry(key, new LogEntry
                                {
                                    Kind = LogEntryKind.Start,
                                    Timestamp = timestamp
                                });
        }
		
        public void WriteTrace(string key, DateTime timestamp, LogEntrySeverity severity, string message, string[] tags)
        {
            WriteEntry(key, new LogEntry
                                {
                                    Kind = LogEntryKind.Trace,
                                    Timestamp = timestamp,
                                    Severity = severity,
                                    Message = message
                                });
        }
		
        public void CompleteSuccess(string key, DateTime timestamp)
        {
            WriteEntry(key, new LogEntry
                                {
                                    Kind = LogEntryKind.Success,
                                    Timestamp = timestamp													
                                });
        }
		
        public void CompleteFailure(string key, DateTime timestamp, string reason)
        {
            WriteEntry(key, new LogEntry
                                {
                                    Kind = LogEntryKind.Failure,
                                    Timestamp = timestamp													
                                });
        }
		
        public abstract IQueryable<LogEntry> QueryEntries(string key);
		
        protected abstract void WriteEntry(string key, LogEntry entry);
    }
}