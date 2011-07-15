namespace Zaz.Server.Advanced.Broker
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
