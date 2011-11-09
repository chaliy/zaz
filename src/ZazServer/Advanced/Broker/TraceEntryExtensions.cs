using System;
using System.Collections.Generic;
using System.Reactive.Subjects;

namespace Zaz.Server.Advanced.Broker
{
    public static class TraceEntryExtensions
    {
        public static void Info(this Subject<TraceEntry> subj, string msg)
        {
            subj.OnNext(new TraceEntry
                            {
                                Message = msg,
                                Serverity = TraceSeverity.Info,
                                Timestamp = DateTime.Now
                            });
        }

        public static void Error(this Subject<TraceEntry> subj, string msg)
        {
            subj.OnNext(new TraceEntry
            {
                Message = msg,
                Serverity = TraceSeverity.Error,
                Timestamp = DateTime.Now
            });
        }

        public static void Error(this ICollection<TraceEntry> subj, string msg)
        {
            subj.Add(new TraceEntry
            {
                Message = msg,
                Serverity = TraceSeverity.Error,
                Timestamp = DateTime.Now
            });
        }
    }
}
