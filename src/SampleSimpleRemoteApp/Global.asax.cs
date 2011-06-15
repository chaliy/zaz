using System;
using System.Linq;
using System.Web;
using SampleCommands;
using SampleHandlers;
using Zaz.EasyRemote.Server;
using Zaz.Local;
using Zaz.Remote.Server;

namespace SampleEasyRemoteApp
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            var bus = DefaultBuses.LocalBus(typeof(SampleHandlersMarker).Assembly, Activator.CreateInstance);            
            var broker = new CommandBusBroker(bus);
            Registration.Register(key => CommandRegistry.ResolveCommand(key).FirstOrDefault(), broker);
        }        
    }
}