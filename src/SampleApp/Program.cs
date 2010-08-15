using CommandRouter;
using CommandRouter.Remote.Client;
using SampleCommands;

namespace SampleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //ICommandBus bus = new LocalCommandBus(CommandHandlerResolvers.FromAssembly(typeof(SampleHandlersMarker).Assembly));
            ICommandBus bus = new RemoteCommandBus("http://localhost:9301/CommandBus.svc");
            bus.Post(new PrintMessage
                        {
                            Message = "Hello world"
                        });
        }
    }    
}
