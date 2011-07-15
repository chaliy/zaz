using System.ComponentModel;

namespace SampleCommands
{
    [Description("Prints message to internal console, unfortunatelly, only IIS will be able to see it.")]
    public class PrintMessage
    {
        [Description("Message to write to the console.")]
        public string Message { get; set; }
    }
}
