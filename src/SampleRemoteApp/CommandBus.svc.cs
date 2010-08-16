using System;
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

        public override Type ResolveCommand(string key)
        {
            return CommandRegistry.GetCommand(key);
        }
    }
}
