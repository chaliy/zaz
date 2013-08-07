


using SampleCommands;
using Zaz.Client;

namespace SampleClientApp
{
    class Program
    {
        static void Main()
        {
            var bus = new ZazClient("http://localhost.fiddler:9302/Commands");
            bus.PostAsync(new PrintMessage
            {
                Message = "Hello world"
            }).Wait();
        }
    }
}
