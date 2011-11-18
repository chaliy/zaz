using System;
using System.Web.Routing;
using Microsoft.ApplicationServer.Http;
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
            string prefix = "Commands/", ServerContext context = null, Action<HttpConfiguration> configure = null)
        {
            // Add / to the prefix, otherwise UI will not work            
            prefix = prefix ?? "";
            if (!prefix.EndsWith("/"))
            {
                prefix += "/";
            }

            var config = HttpHostConfigurationHelper.CreateConfiguration(new CommandsService(context));

            if (configure != null)
            {
                configure(config);
            }

            @this.MapServiceRoute<CommandsService>(prefix, config);

            return @this;
        }
    }
}
