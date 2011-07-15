using System;
using System.Net.Http;
using System.ServiceModel;
using System.Web.Routing;
using Microsoft.ApplicationServer.Http.Activation;
using Microsoft.ApplicationServer.Http.Description;
using Zaz.Server.Advanced;

namespace Zaz.Server
{
    public class ZazServer
    {
        private class CommandBusFactory : IResourceFactory
        {            
            private readonly Conventions _conventions;

            public CommandBusFactory(Conventions conventions)
            {
                _conventions = conventions;
            }

            public object GetInstance(Type serviceType, InstanceContext instanceContext, HttpRequestMessage request)
            {
                return new CommandsService(_conventions);
            }

            public void ReleaseInstance(InstanceContext instanceContext, object service)
            {               
            }
        }

        public static void Init(string prefix = "Commands", Conventions conventions = null)
        {
            var config = HttpHostConfiguration.Create()
                .SetResourceFactory(new CommandBusFactory(conventions));
            RouteTable.Routes.MapServiceRoute<CommandsService>(prefix, config);
        }
    }
}
