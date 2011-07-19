using System.Web.Routing;
using Microsoft.ApplicationServer.Http.Activation;
using Microsoft.ApplicationServer.Http.Description;
using WebApiContrib.Formatters.JsonNet;
using Zaz.Server.Advanced;
using Zaz.Server.Advanced.Service;

namespace Zaz.Server
{
    public class ZazServer
    {
        public static void Init(string prefix = "Commands/", Conventions conventions = null)
        {
            // Add / to the prefix, otherwise UI will not work            
            prefix = prefix ?? "";
            if (!prefix.EndsWith("/"))
            {
                prefix += "/";
            }
            
            RouteTable.Routes.MapServiceRoute<CommandsService>(prefix, 
                HttpHostConfigurationHelper.CreateHostConfigurationBuilder(new CommandsService(conventions)));
        }
    }
}
