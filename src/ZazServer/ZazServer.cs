using System;
using System.Net.Http;
using System.ServiceModel;
using System.Web.Routing;
using Microsoft.ApplicationServer.Http.Activation;
using Microsoft.ApplicationServer.Http.Description;
using Zaz.Server.Advanced.Service;

namespace Zaz.Server
{
    public class ZazServer
    {
        private class CommandBusFactory : IResourceFactory
        {            
            private readonly CommandsService _instance;

            public CommandBusFactory(CommandsService instance)
            {
                _instance = instance;
            }

            public object GetInstance(Type serviceType, InstanceContext instanceContext, HttpRequestMessage request)
            {                
                return _instance;
            }

            public void ReleaseInstance(InstanceContext instanceContext, object service)
            {                
            }
        }

        public static void Init(string prefix = "Commands/", Conventions conventions = null)
        {
            // Add / to the prefix, otherwise UI will not work            
            prefix = prefix ?? "";
            if (!prefix.EndsWith("/"))
            {
                prefix += "/";
            }
            var config = HttpHostConfiguration.Create()
                .SetResourceFactory(new CommandBusFactory(new CommandsService(conventions)));
            RouteTable.Routes.MapServiceRoute<CommandsService>(prefix, config);
        }
    }
}
