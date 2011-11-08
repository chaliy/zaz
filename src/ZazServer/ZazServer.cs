using System.Web.Routing;
using Microsoft.ApplicationServer.Http.Activation;
using Zaz.Server.Advanced;
using Zaz.Server.Advanced.Service;

namespace Zaz.Server
{
    public static class ZazServer
    {
        public static void Init(string prefix = "Commands/", ServerContext context = null)
        {
            RouteTable.Routes.MapCommandsService(prefix, context);
        }

        public static RouteCollection MapCommandsService(this RouteCollection @this, 
            string prefix = "Commands/", ServerContext context = null)
        {
            // Add / to the prefix, otherwise UI will not work            
            prefix = prefix ?? "";
            if (!prefix.EndsWith("/"))
            {
                prefix += "/";
            }

            @this.MapServiceRoute<CommandsService>(prefix,
                HttpHostConfigurationHelper.CreateHostConfigurationBuilder(new CommandsService(context)));

            return @this;
        }
    }
}
