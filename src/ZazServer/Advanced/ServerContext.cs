using System;
using Microsoft.ApplicationServer.Http;
using Zaz.Server.Advanced.Broker;
using Zaz.Server.Advanced.Registry;
using Zaz.Server.Advanced.State;

namespace Zaz.Server.Advanced
{
    public class ServerContext
    {
        private readonly ICommandRegistry _registry;
        private readonly ICommandBroker _broker;
        private readonly ICommandStateProvider _stateProvider;

        public ServerContext(ICommandRegistry registry = null, 
            ICommandBroker broker = null,
            ICommandStateProvider stateProvider = null)
        {
            _registry = registry ?? Implementations.CommandRegistry;
            _broker = broker ?? Implementations.Broker;
            _stateProvider = stateProvider ?? Implementations.StateProvider;
        }

        public ICommandRegistry Registry { get { return _registry; } }
        public ICommandBroker Broker { get { return _broker; } }
        public ICommandStateProvider StateProvider { get { return _stateProvider; } }        
    }
}