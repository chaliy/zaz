namespace Zaz.Server
{
    public class CommandHandlingContext
    {
        public string[] Tags { get; set; }

        public CommandHandlingContext()
        {
            Tags = new string[0];
        }
    }
}
