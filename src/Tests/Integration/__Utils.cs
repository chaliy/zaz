using System;
using System.Threading.Tasks;
using Microsoft.ApplicationServer.Http;
using Zaz.Server.Advanced;
using Zaz.Server.Advanced.Broker;
using Zaz.Server.Advanced.Service;
using Zaz.Tests.Stubs;

namespace Zaz.Tests.Integration
{
    static class __CommandsServiceExt
    {
        public static HttpServiceHost OpenConfiguredServiceHost(this CommandsService @this, string url)
        {
            var config = ConfigurationHelper.CreateConfiguration(@this);            
            var host = new HttpServiceHost(typeof(CommandsService), config, new Uri(url));
            host.Open();
            return host;
        }
    }

    static class Create
    {
        public static CommandsService FooCommandsService(Func<object, CommandHandlingContext, Task> delegatex)
        {
            var instance = new CommandsService(new ServerContext
            (
                registry: new FooCommandRegistry(),
                broker: new DelegatingCommandBroker(delegatex)
            ));
            return instance;
        }

    }
}
