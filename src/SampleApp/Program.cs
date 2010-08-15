using CommandRouter;
using CommandRouter.Remote;
using SampleCommands;
using SampleHandlers;

namespace SampleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //ICommandBus bus = new LocalCommandBus(CommandHandlerResolvers.FromAssembly(typeof(SampleHandlersMarker).Assembly));
            ICommandBus bus = new Client.RemoteCommandBus("http://localhost:9301/CommandBus.svc");
            ICommandClient client = new CommandClient(bus);
            client.Post(new PrintMessage
                            {
                                Message = "Hello world"
                            });
        }
    }    
}
