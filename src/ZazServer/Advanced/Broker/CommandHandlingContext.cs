using System;
using System.Reactive.Subjects;
using System.Security.Principal;
using System.Threading;

namespace Zaz.Server.Advanced.Broker
{
    public class CommandHandlingContext
    {
        private readonly string[] _tags;
        private readonly IObserver<LogEntry> _log;
        private readonly IPrincipal _principal; 
                
        public CommandHandlingContext(string[] tags = null, IPrincipal principal = null, IObserver<LogEntry> log = null)
        {
            _tags = tags ?? new string[0];
            _log = log ?? new Subject<LogEntry>();
            _principal = principal ?? Thread.CurrentPrincipal;
        }

        public string[] Tags { get { return _tags; } }
        public IObserver<LogEntry> Log { get { return _log; } }
        public IPrincipal Principal { get { return _principal; } }
    }
}
