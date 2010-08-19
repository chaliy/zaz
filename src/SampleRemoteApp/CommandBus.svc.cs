using System;
using System.Collections.Generic;
using Zaz;
using Zaz.Local;
using Zaz.Remote.Server;
using SampleCommands;
using SampleHandlers;

namespace SampleRemoteApp
{
    public class SampleCommandBusService : CommandBusService
    {
        public override ICommandBus CreateCommandBus()
        {
			return DefaultBuses.LocalBus(typeof(SampleHandlersMarker).Assembly);            
        }

        public override IEnumerable<Type> ResolveCommand(string key)
        {
            return CommandRegistry.ResolveCommand(key);
        }
    }
}
