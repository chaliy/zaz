using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using Zaz.Server.Advanced.Broker;
using Zaz.Server.Advanced.Logging;

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

            var log = new ZazLogToStringAdapter();
            var ctx = new CommandHandlingContext(tags, Thread.CurrentPrincipal, log);
            try
            {
                broker.Handle(cmd, ctx).Wait();
            }
            catch (AggregateException ex)
            {
                foreach (var child in ex.Flatten().InnerExceptions)
                {                    
                    log.Error(child.Message);
                }
            }

            var msg = new StringBuilder("Command " + cmdKey + " accepted.");

            if (log.HasSomething())
            {
                msg.AppendLine();
                msg.Append(log);
            }

            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Accepted,
                Content = new StringContent(msg.ToString())
            };
        }
    }
}
