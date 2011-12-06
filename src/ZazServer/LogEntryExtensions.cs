using System;
using System.Collections.Generic;
using Zaz.Server.Advanced;

namespace Zaz.Server
{
    public static class LogEntryExtensions
    {
        public static void Info(this IObserver<LogEntry> subj, string msg)
        {
            subj.OnNext(new LogEntry
                            {
                                Message = msg,
                                Severity = LogEntrySeverity.Info,
                                Timestamp = DateTime.Now
                            });
        }

        public static void Error(this IObserver<LogEntry> subj, string msg)
        {
            subj.OnNext(new LogEntry
            {
                Message = msg,
                Severity = LogEntrySeverity.Error,
                Timestamp = DateTime.Now
            });
        }

        public static void Error(this ICollection<LogEntry> subj, string msg)
        {
            subj.Add(new LogEntry
            {
                Message = msg,
                Severity = LogEntrySeverity.Error,
                Timestamp = DateTime.Now
            });
        }
    }
}
