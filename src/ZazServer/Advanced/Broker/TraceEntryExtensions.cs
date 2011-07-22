using System;
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
    }
}
