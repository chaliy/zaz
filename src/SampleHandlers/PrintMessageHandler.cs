using System;
using System.Diagnostics;
using System.Threading.Tasks;
using SampleCommands;

namespace SampleHandlers
{
    public class PrintMessageHandler
    {
        public Task Handle(PrintMessage cmd)
        {
            return Task.Factory.StartNew(() =>
            {
                Console.WriteLine(cmd.Message);
                EventLog.WriteEntry("Application", "PrintMessage from Zaz Sample Application. Message: " + cmd.Message);
            });
        }
    }
}
