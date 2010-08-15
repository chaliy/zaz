using System;
using CommandRouter;
using CommandRouter.Local;
using CommandRouter.Remote.Server;
using SampleCommands;
using SampleHandlers;

namespace SampleRemoteApp
{
    public class SampleCommandBusService : CommandBusService
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
