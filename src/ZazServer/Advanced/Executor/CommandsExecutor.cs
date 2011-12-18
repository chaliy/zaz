using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Subjects;
using System.Security.Principal;
using System.Threading.Tasks;
using Zaz.Server.Advanced.Broker;
using Zaz.Server.Advanced.State;

namespace Zaz.Server.Advanced.Executor
{
    public class CommandsExecutor
    {
        private readonly ServerContext _context;

        public CommandsExecutor(ServerContext context)
        {
            _context = context;
        }

        public ExecutionId Submit(object cmd, string[] tags, IPrincipal principal)
        {            
            var stateProvider = _context.StateProvider;

            var id = ExecutionId.New();
            stateProvider.Start(id, DateTime.UtcNow);
            var log = new Subject<LogEntry>();
            
            var traceSubscription = log
                .Subscribe(e => stateProvider.WriteTrace(id, DateTime.UtcNow, e.Severity, e.Message, e.Tags));

            Execute(cmd, tags, principal, log)            
                .ContinueWith(t =>
                {
                    var execResult = t.Result;
                    if (execResult.IsSucces)
                    {
                        stateProvider.CompleteSuccess(id, DateTime.UtcNow);
                    }
                    else
                    {
                        stateProvider.CompleteFailure(id, DateTime.UtcNow, execResult.FailureReason);
                    }                   
                    traceSubscription.Dispose();
                });

            return id;
        }

        public ExecutionStats GetExecutionStats(ExecutionId id, DateTime? token)
        {

            var stateProvider = (_context.StateProvider ?? Implementations.StateProvider);
            var lastTrace = stateProvider
                .QueryEntries(id)
                .OrderBy(x => x.Timestamp)
                .LastOrDefault();

            var status = ExecutionStatus.Pending;
            if (lastTrace != null)
            {
                switch (lastTrace.Kind)
                {
                    case ProgressEntryKind.Failure:
                        status = ExecutionStatus.Failure;
                        break;

                    case ProgressEntryKind.Success:
                        status = ExecutionStatus.Success;
                        break;

                    case ProgressEntryKind.Trace:
                    case ProgressEntryKind.Start:
                        status = ExecutionStatus.InProgress;
                        break;
                }
            }

            var log = stateProvider
                .QueryEntries(id)
                .Where(x => token.HasValue && x.Timestamp > token)
                .Where(x => x.Kind == ProgressEntryKind.Trace)       
                .Select(ConvertToLogEntry())
                .ToList();

            return new ExecutionStats
            {
                Id = id,
                Status = status,
                Log = log
            };
        }
        
        private Task<CommandExecutionResult> Execute(object cmd, string[] tags, IPrincipal principal, IObserver<LogEntry> log)
        {
            var broker = _context.Broker;
                                            
            var ctx = new CommandHandlingContext(tags, principal, log);
           
            return broker.Handle(cmd, ctx)
                .ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        if (t.Exception != null)
                        {
                            var ex = t.Exception.Flatten();
                            if (ex.InnerExceptions.Count == 0)
                            {
                                log.Error(ex.GetBaseException().ToString());
                            }
                            else
                            {
                                foreach (var child in ex.InnerExceptions)
                                {
                                    log.Error(child.ToString());
                                }
                            }
                            return CommandExecutionResult.Failure(ex.GetBaseException().Message);
                        }
                        else
                        {
                            return CommandExecutionResult.Failure();
                        }
                    }
                    // TODO: Handle cancelled
                    return CommandExecutionResult.Success();                 
                });            
        }

        public static Expression<Func<ProgressEntry, LogEntry>> ConvertToLogEntry()
        {
            return e => new LogEntry
            {
                Message = e.Message,
                Severity = e.Severity,
                Tags = e.Tags,
                Timestamp = e.Timestamp
            };
        }
    }
}
