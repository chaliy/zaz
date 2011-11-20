using System;
using System.Web.Routing;
using Zaz.Server.Advanced;
using Zaz.Server.Advanced.Service;
namespace Zaz.Server
{
    public static class RoutingExtensions
    {
        public static RouteCollection MapCommandsService(this RouteCollection @this,
            string prefix = "Commands/", ServerConfiguration configuration = null)
        {
            // Add / to the prefix, otherwise UI will not work            
            prefix = prefix ?? "";
            if (!prefix.EndsWith("/"))
            {
                prefix += "/";
            }

            var config = ConfigurationHelper.CreateServiceConfiguration(configuration);
            
            @this.MapServiceRoute<CommandsService>(prefix, config);

            return @this;
        }
    }
}
