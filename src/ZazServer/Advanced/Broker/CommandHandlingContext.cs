using System.Reactive.Subjects;
using System.Security.Principal;

namespace Zaz.Server.Advanced.Broker
{
    public class CommandHandlingContext
    {
        private readonly string[] _tags;
        private readonly Subject<TraceEntry> _trace;
        private readonly IPrincipal _principal; 
                
        public CommandHandlingContext(string[] tags, IPrincipal principal)
        {
            _tags = tags ?? new string[0];
            _trace = new Subject<TraceEntry>();
            _principal = principal;
        }

        public string[] Tags { get { return _tags; } }
        public Subject<TraceEntry> Trace { get { return _trace; } }
        public IPrincipal Principal { get { return _principal; } }
    }
}
