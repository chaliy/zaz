using CommandRouter;
using SampleCommands;
using SampleHandlers;

namespace SampleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ICommandBus bus = new LocalCommandBus(CommandHandlerResolvers.FromAssembly(typeof(SampleHandlersMarker).Assembly));
            ICommandClient client = new CommandClient(bus);
            client.Post(new PrintMessage
                            {
                                Message = "Hello world"
                            });
        }
    }    
}
