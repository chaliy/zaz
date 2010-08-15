using System;
using CommandRouter;
using CommandRouter.Remote;
using SampleCommands;
using SampleHandlers;

namespace SampleRemoteApp
{
    public class SampleCommandBusService : Server.CommandBusService
    {
        public override ICommandBus CreateCommandBus()
        {
            return new LocalCommandBus(CommandHandlerResolvers.FromAssembly(typeof(SampleHandlersMarker).Assembly));            
        }

        public override Type ResolveCommand(string key)
        {
            return CommandRegistry.GetCommand(key);
        }
    }
}
