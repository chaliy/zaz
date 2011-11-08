using System;
using System.Web;
using SampleCommands;
using SampleHandlers;
using Zaz.Server;
using Zaz.Server.Advanced.Broker;
using Zaz.Server.Advanced.Registry;

namespace SampleServerApp
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {            
            ZazServer.Init("Commands",                
                new ServerContext
                {
                    Registry = new ReflectionCommandRegistry(typeof(__SampleCommandsMarker).Assembly),
                    Broker = new ReflectionCommandBroker(typeof(__SampleHandlersMarker).Assembly)
                });
        }        
    }
}