using System;
using System.Diagnostics;
using SampleCommands;

namespace SampleHandlers
{
    public class PrintMessageHandler
    {
        public void Handle(PrintMessage cmd)
        {
            Console.WriteLine(cmd.Message);            
            EventLog.WriteEntry("Application", "PrintMessage from Zaz Sample Application. Message: " + cmd.Message);
        }
    }
}
