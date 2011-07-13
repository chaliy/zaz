using System;
using System.Collections.Generic;
using Zaz;
using Zaz.Local;
using Zaz.Remote.Server;
using SampleCommands;
using SampleHandlers;

namespace SampleRemoteApp
{
    public class SimpleCommandBusService : CommandBusService
    {
        
        public override ICommandBroker CreateCommandBroker()
        {
            var bus = DefaultBuses.LocalBus(typeof(__SampleHandlersMarker).Assembly, Activator.CreateInstance);
            return new CommandBusBroker(bus);
        }

        public override IEnumerable<Type> ResolveCommand(string key)
        {
            return CommandRegistry.ResolveCommand(key);
        }
    }
}
