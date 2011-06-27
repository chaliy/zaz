using System;
using System.Net.Http;
using System.ServiceModel;
using System.Web.Routing;
using Microsoft.ApplicationServer.Http.Activation;
using Microsoft.ApplicationServer.Http.Description;

namespace Zaz.Server
{
    public class Registration
    {
        private class CommandBusFactory : IResourceFactory
        {
            private readonly ICommandBroker _broker;
            private readonly Conventions _conventions;

            public CommandBusFactory(ICommandBroker broker, Conventions conventions)
            {
                _broker = broker;
                _conventions = conventions;
            }

            public object GetInstance(Type serviceType, InstanceContext instanceContext, HttpRequestMessage request)
            {
                return new CommandsService(_broker, _conventions);
            }

            public void ReleaseInstance(InstanceContext instanceContext, object service)
            {               
            }
        }

        public static void Init(ICommandBroker broker, Conventions conventions)
        {
            var config = HttpHostConfiguration.Create()
                .SetResourceFactory(new CommandBusFactory(broker, conventions));
            RouteTable.Routes.MapServiceRoute<CommandsService>("Commands", config);
        }
    }
}
