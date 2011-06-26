using System;
using System.Linq;
using System.Web;
using SampleCommands;
using SampleHandlers;
using Zaz.Server;

namespace SampleEasyRemoteApp
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            Registration.Init(
                new LocalCommandBroker(typeof(SampleHandlersMarker).Assembly), 
                new Conventions
                {
                    CommandResolver = CommandRegistry.ResolveCommand2
                });
        }        
    }
}