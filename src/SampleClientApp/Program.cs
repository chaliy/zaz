using Zaz.Client;
using SampleCommands;

namespace SampleClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var bus = new CommandBus("http://localhost.fiddler:9302/Commands");            
            bus.Post(new PrintMessage
                        {
                            Message = "Hello world"
                        });
        }
    }    
}
