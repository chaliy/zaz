using System;
using System.Linq;
using System.Web;
using SampleCommands;
using SampleHandlers;
using Zaz.Server;
using Zaz.Server.Advanced.Broker;
using Zaz.Server.Advanced.Registry;

namespace SampleEasyRemoteApp
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            ZazServer.Init("Commands/",                
                new Conventions
                {
                    Registry = new ReflectionCommandRegistry(typeof(__SampleCommandsMarker).Assembly),
                    Broker = new ReflectionCommandBroker(typeof(__SampleHandlersMarker).Assembly)
                });
        }        
    }
}