using Zaz.Server.Advanced.Broker;
using Zaz.Server.Advanced.Registry;
using Zaz.Server.Advanced.State;

namespace Zaz.Server
{
    public class Conventions
    {        
        public ICommandRegistry CommandRegistry { get; set; }
        public ICommandBroker Broker { get; set; }
        public ICommandStateProvider StateProvider { get; set; }
    }
}
