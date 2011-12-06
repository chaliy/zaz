using System;
using System.Net.Http;
using System.Reactive.Subjects;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zaz.Server.Advanced.Broker;
using Zaz.Server.Advanced.Logging;
using Zaz.Server.Advanced.Service.Contract;

namespace Zaz.Server.Advanced.Executor
{
    public class CommandsExecutor
    {
        private readonly ServerContext _context;

        public CommandsExecutor(ServerContext context)
        {
            _context = context;
        }
        
        public string SubmitScheduled(object cmd, string[] tags, IPrincipal principal)
        {            
            var stateProvider = _context.StateProvider;

            var id = Guid.NewGuid().ToString("n");
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
    }
}
