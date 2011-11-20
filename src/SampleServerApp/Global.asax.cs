using System;
using System.Web;
using System.Web.Routing;
using SampleCommands;
using SampleHandlers;
using SampleServerApp.App;
using Zaz.Server;
using Zaz.Server.Advanced;
using Zaz.Server.Advanced.Broker;
using Zaz.Server.Advanced.Registry;

namespace SampleServerApp
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {            

            RouteTable.Routes.MapCommandsService("Commands",
            new ServerConfiguration
            {
                Registry = new ReflectionCommandRegistry(typeof(__SampleCommandsMarker).Assembly),
                Broker = new ReflectionCommandBroker(typeof(__SampleHandlersMarker).Assembly)
                //Broker = new DumbCommandBroker()
            });
        }        
    }
}