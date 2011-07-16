namespace Zaz.Server.Advanced.Service
{
    public class CommandEnvelope
    {
        public string Key { get; set; }
        public object Command { get; set; }
        public string[] Tags { get; set; }
    }
}
