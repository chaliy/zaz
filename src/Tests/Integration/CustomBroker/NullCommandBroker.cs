using System.Collections.Generic;
using System.Threading.Tasks;
using Zaz.Server.Advanced.Broker;

namespace Zaz.Tests.Integration.CustomBroker
{
    public class NullCommandBroker : ICommandBroker
    {
        public readonly List<object> CommandsPosted = new List<object>();

        public Task Handle(dynamic cmd, CommandHandlingContext ctx)
        {
            CommandsPosted.Add(cmd);
            return Task.Factory.StartNew(() => { });
        }
    }
}
