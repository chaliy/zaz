using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using Zaz.Server.Advanced.Broker;

namespace Zaz.Server.Advanced.Service
{
    public class CommandRunner
    {
        private readonly ServerContext _context;

        public CommandRunner(ServerContext context)
        {
            _context = context;
        }

        public HttpResponseMessage RunCommand(string cmdKey, object cmd, string[] tags)
        {
            var broker = (_context.Broker ?? Implementations.Broker);

            var ctx = new CommandHandlingContext(tags ?? new string[0], Thread.CurrentPrincipal);
            var trace = new List<TraceEntry>();
            using (ctx.Trace.Subscribe(trace.Add))
            {
                try
                {
                    broker.Handle(cmd, ctx).Wait();
                }
                catch (AggregateException ex)
                {
                    foreach (var child in ex.Flatten().InnerExceptions)
                    {
                        trace.Error(child.Message);
                    }                    
                }                
            }

            var msg = new StringBuilder("Command " + cmdKey + " accepted.");

            if (trace.Count > 0)
            {
                msg.AppendLine();
                msg.AppendLine("Trace: ");
                foreach (var traceEntry in trace)
                {
                    msg.AppendLine("[" + traceEntry.Serverity + "]" + traceEntry.Message);
                }
            }

            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Accepted,
                Content = new StringContent(msg.ToString())
            };
        }
    }
}
