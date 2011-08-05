using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using Zaz.Server.Advanced.Broker;

namespace Zaz.Server.Advanced.Service
{
    public class CommandRunner
    {
        private readonly Conventions _conventions;

        public CommandRunner(Conventions conventions)
        {
            _conventions = conventions;
        }

        public HttpResponseMessage RunCommand(string cmdKey, object cmd, string[] tags)
        {
            var broker = (_conventions.Broker ?? DefaultConventions.Broker);

            var ctx = new CommandHandlingContext(tags ?? new string[0]);
            var trace = new List<TraceEntry>();
            using (ctx.Trace.Subscribe(trace.Add))
            {
                try
                {
                    broker.Handle(cmd, ctx)
                        .Wait();
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

            foreach (var traceEntry in trace)
            {
                msg.AppendLine("[" + traceEntry.Serverity + "]" + traceEntry.Message);
            }

            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Accepted,
                Content = new StringContent(msg.ToString())
            };
        }
    }
}
