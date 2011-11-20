using System;
using System.Threading.Tasks;
namespace Zaz.Server.Advanced.Broker
{
    public class DelegatingCommandBroker : ICommandBroker
    {
        private readonly Func<object, CommandHandlingContext, Task> _delegate;

        public DelegatingCommandBroker(Func<object, CommandHandlingContext, Task> delegatex)
        {
            _delegate = delegatex;
        }

        public Task Handle(object cmd, CommandHandlingContext ctx)
        {
            return _delegate(cmd, ctx);
        }
    }
}
