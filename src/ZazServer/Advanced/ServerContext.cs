using Zaz.Server.Advanced.Broker;
using Zaz.Server.Advanced.Registry;
using Zaz.Server.Advanced.State;

namespace Zaz.Server.Advanced
{
    public class ServerContext
    {
        readonly ICommandRegistry _registry;
        readonly ICommandBroker _broker;
        readonly ICommandStateProvider _stateProvider;

        public ServerContext(
            ICommandRegistry registry = null,
            ICommandBroker broker = null,
            ICommandStateProvider stateProvider = null)
        {
            _registry = registry ?? Implementations.CommandRegistry.Value;
            _broker = broker ?? Implementations.Broker.Value;
            _stateProvider = stateProvider ?? Implementations.StateProvider.Value;
        }

        public ICommandRegistry Registry { get { return _registry; } }
        public ICommandBroker Broker { get { return _broker; } }
        public ICommandStateProvider StateProvider { get { return _stateProvider; } }
    }
}