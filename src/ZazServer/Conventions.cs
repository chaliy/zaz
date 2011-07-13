namespace Zaz.Server
{
    public class Conventions
    {        
        public ICommandRegistry CommandRegistry { get; set; }
        public ICommandBroker CommandBroker { get; set; }
        public ICommandStateProvider StateProvider { get; set; }
    }
}
