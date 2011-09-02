using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web;
using Zaz.Server.Advanced.Broker;
using Zaz.Server.Advanced.Service.Contract;
using Zaz.Server.Advanced.State;
using StateTraceEntry = Zaz.Server.Advanced.State.TraceEntry;
using ContractTraceEntry = Zaz.Server.Advanced.Service.Contract.TraceEntry;
using Zaz.Server.Advanced.Ui;

namespace Zaz.Server.Advanced.Service
{
    [ServiceContract]
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]    
    public class CommandsService
    {
        private readonly Conventions _conventions;
        private readonly CommandResolver _resolver;
        private readonly CommandRunner _runner;
        
        public CommandsService(Conventions conventions = null)
        {            
            _conventions = conventions ?? new Conventions();            
            _resolver = new CommandResolver(_conventions);
            _runner = new CommandRunner(_conventions);
        }
        
        [WebGet(UriTemplate = "/{*path}")]
        public HttpResponseMessage Get(string path = "index.html")
        {
            return new HttpResponseMessage
                       {
                           Content = UiContent.Get(path)
                       };
        }

        [WebGet(UriTemplate = "MetaList")]
        public IQueryable<CommandMeta> MetaList()
        {
            var registry = (_conventions.Registry ?? DefaultConventions.CommandRegistry);
            return registry
                .Query()
                .Select(x => new CommandMeta
                            {
                                Key = x.Key,
                                Description = x.Description,
                                Aliases = x.Aliases,
                                Tags = x.Tags,
                                Parameters = x.Parameters
                                              .Select(xx => new CommandMetaParameter
                                                                {
                                                                    Name = xx.Name,
                                                                    Description = xx.Description,
                                                                    Type = xx.Type
                                                                })
                                              .ToArray()
                            });
        }      

        private static object BindFormToCommand(IDictionary<string, string> form, Type cmdType)
        {
            try
            {
                var binder = new CommandBinder();
                return binder.Build(cmdType, form);
            }
            catch (InvalidOperationException ex)
            {
                throw ExceptionsFactory.CreateApiException("Problems with binding command data. " + ex.Message);
            }
        }

        [WebInvoke(Method = "POST", UriTemplate = "Legacy")]
        public HttpResponseMessage PostLegacy(HttpRequestMessage req)
        {
            //if (req.Content.Headers
            //    .Any(x => x.Key == "Content-Type"
            //        && x.Value.Any(xx => xx == "application/x-www-form-urlencoded")))
            //{
            // Old legacy version

            var body = req.Content.ReadAsString();
            var form = ParseQueryString(body);

            if (!form.ContainsKey("Zaz-Command-Id"))
            {
                throw ExceptionsFactory.CreateApiException("Required value 'Zaz-Command-Id' was not found.");
            }

            var cmdKey = form["Zaz-Command-Id"];
            var cmdType = _resolver.ResolveCommandType(cmdKey);

            var cmd = BindFormToCommand(form, cmdType);

            return _runner.RunCommand(cmdKey, cmd, new string[0]);
        }

        private static Dictionary<string, string> ParseQueryString(string query)
        {
            var nvc = HttpUtility.ParseQueryString(query);
            return nvc.Keys
                .Cast<string>()
                .ToDictionary(x => x, x => nvc[x]);
        }


        [WebInvoke(Method = "POST", UriTemplate = "")]
        public HttpResponseMessage Post(PostScheduledCommandRequest env)
        {            
            var cmdKey = env.Key;
            var cmd = _resolver.ResoveCommand(env, cmdKey);
            return _runner.RunCommand(cmdKey, cmd, env.Tags);
        }
        
        [WebInvoke(Method = "POST", UriTemplate = "Scheduled")]
        public PostScheduledCommandResponse PostScheduled(PostScheduledCommandRequest req)
        {
            var cmdKey = req.Key;
            var cmd = _resolver.ResoveCommand(req, cmdKey);

            var broker = (_conventions.Broker ?? DefaultConventions.Broker);
            var stateProvider = (_conventions.StateProvider ?? DefaultConventions.StateProvider);

            var id = Guid.NewGuid().ToString("n");
            stateProvider.Start(id, DateTime.UtcNow);
            var ctx = new CommandHandlingContext(req.Tags ?? new string[0]);
            var traceSubscription = ctx.Trace
                .Subscribe(e => stateProvider.WriteTrace(id, DateTime.UtcNow, e.Serverity, e.Message, e.Tags));

            broker.Handle(cmd, ctx)
                .ContinueWith(t =>
                                  {
                                      traceSubscription.Dispose();
                                      stateProvider.CompleteSuccess(id, DateTime.UtcNow);
                                  });

            return new PostScheduledCommandResponse
                       {
                           Id = id
                       };
        }

        [WebGet(UriTemplate = "Scheduled/{id}?token={token}")]
        public GetScheduledCommandResponse GetScheduled(string id, DateTime? token)
        {

            var stateProvider = (_conventions.StateProvider ?? DefaultConventions.StateProvider);
            var lastTrace = stateProvider
                .QueryEntries(id)
                .OrderBy(x => x.Timestamp)
                .LastOrDefault();

            var status = ScheduledCommandStatus.Pending;
            if (lastTrace != null)
            {
                switch (lastTrace.Kind)
                {
                    case TraceKind.Failure:
                        status = ScheduledCommandStatus.Failure;
                        break;
                        
                    case TraceKind.Success:
                        status = ScheduledCommandStatus.Success;
                        break;

                    case TraceKind.Trace:
                    case TraceKind.Start:
                        status = ScheduledCommandStatus.InProgress;
                        break;
                }
            }

            var trace = stateProvider
                .QueryEntries(id)
                .Where(x => token.HasValue && x.Timestamp > token)
                .Where(x => x.Kind == TraceKind.Trace)
                .Select(ConvertTraceEntry())
                .ToArray();

            return new GetScheduledCommandResponse
                           {
                               Id = id,
                               Status = status,
                               Trace = trace
                           };
        }

        [WebGet(UriTemplate = "Scheduled/Trace/{id}")]
        public IQueryable<ContractTraceEntry> GetScheduledTrace(string id)
        {            
            var stateProvider = (_conventions.StateProvider ?? DefaultConventions.StateProvider);
            return stateProvider
                .QueryEntries(id)
                .Where(x => x.Kind == TraceKind.Trace)
                .Select(ConvertTraceEntry());
        }
       
        public static Expression<Func<StateTraceEntry, ContractTraceEntry>> ConvertTraceEntry()
        {
            return e =>  new ContractTraceEntry
                       {
                           Message = e.Message,
                           Severity = e.Severity,
                           Tags = e.Tags,
                           Timestamp = e.Timestamp
                       };
        }
    }
}
