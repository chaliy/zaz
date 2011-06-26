using Zaz.Client;
using SampleCommands;

namespace SampleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var bus = new CommandBus("http://localhost:9302/");            
            bus.Post(new PrintMessage
                        {
                            Message = "Hello world"
                        });
        }
    }    
}
