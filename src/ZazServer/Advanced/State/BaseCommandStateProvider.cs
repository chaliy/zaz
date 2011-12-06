using System;
using System.Linq;

namespace Zaz.Server.Advanced.State
{
    public abstract class BaseCommandStateProvider : ICommandStateProvider
    {				
        public void Start(string key, DateTime timestamp)
        {			
            WriteEntry(key, new ProgressEntry
                                {
                                    Kind = ProgressEntryKind.Start,
                                    Timestamp = timestamp
                                });
        }
		
        public void WriteTrace(string key, DateTime timestamp, LogEntrySeverity severity, string message, string[] tags)
        {
            WriteEntry(key, new ProgressEntry
                                {
                                    Kind = ProgressEntryKind.Trace,
                                    Timestamp = timestamp,
                                    Severity = severity,
                                    Message = message
                                });
        }
		
        public void CompleteSuccess(string key, DateTime timestamp)
        {
            WriteEntry(key, new ProgressEntry
                                {
                                    Kind = ProgressEntryKind.Success,
                                    Timestamp = timestamp													
                                });
        }
		
        public void CompleteFailure(string key, DateTime timestamp, string reason)
        {
            WriteEntry(key, new ProgressEntry
                                {
                                    Kind = ProgressEntryKind.Failure,
                                    Timestamp = timestamp													
                                });
        }
		
        public abstract IQueryable<ProgressEntry> QueryEntries(string key);
		
        protected abstract void WriteEntry(string key, ProgressEntry entry);
    }
}