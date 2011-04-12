using System;
using System.Linq;
using System.Web;
using SampleCommands;
using SampleHandlers;
using Zaz.EasyRemote.Server;
using Zaz.Local;

namespace SampleSimpleRemoteApp
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            var bus = DefaultBuses.LocalBus(typeof(SampleHandlersMarker).Assembly, Activator.CreateInstance);
            Registration.Register(key => CommandRegistry.ResolveCommand(key).FirstOrDefault(), bus);
        }        
    }
}