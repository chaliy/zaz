using Microsoft.ApplicationServer.Http.Description;
using WebApiContrib.Formatters.JsonNet;
using Zaz.Server.Advanced.Service;

namespace Zaz.Server.Advanced
{
    public static class HttpHostConfigurationHelper
    {
        public static IHttpHostConfigurationBuilder CreateHostConfigurationBuilder(CommandsService service)
        {
            var config = HttpHostConfiguration.Create()
                .SetResourceFactory(new CommandsServiceResourceFactory(service))
                ;

            config.Configuration.OperationHandlerFactory.Formatters.Insert(0, new JsonNetFormatter());

            return config;
        }
    }
}
