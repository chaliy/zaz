using System;
using SampleCommands;

namespace SampleHandlers
{
    public class PrintMessageHandler
    {
        public void Handle(PrintMessage cmd)
        {
            Console.WriteLine(cmd.Message);
        }
    }
}
