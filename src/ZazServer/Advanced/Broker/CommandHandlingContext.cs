using System.Reactive.Subjects;

namespace Zaz.Server.Advanced.Broker
{
    public class CommandHandlingContext
    {
        public readonly string[] Tags;
        public readonly Subject<TraceEntry> Trace
            = new Subject<TraceEntry>();

        public CommandHandlingContext(string[] tags)
        {
            Tags = tags;
        }
    }
}
