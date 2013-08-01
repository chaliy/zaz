using System;
using System.Threading.Tasks;
using System.Web.Http.SelfHost;
using Zaz.Server;
using Zaz.Server.Advanced;
using Zaz.Server.Advanced.Broker;
using Zaz.Server.Advanced.Service;
using Zaz.Tests.Stubs;

namespace Zaz.Tests.Integration
{
    static class __CommandsServiceExt
    {
        public static HttpSelfHostServer OpenConfiguredServiceHost(this CommandsController @this, string url)
        {
            var config = ZazServer.ConfigureAsSelfHosted(url);
            var host = new HttpSelfHostServer(config);
            host.OpenAsync().Wait();
            return host;
        }
    }

    static class Create
    {
        public static CommandsController FooCommandsService(Func<object, CommandHandlingContext, Task> delegatex)
        {
            var instance = new CommandsController(new ServerContext
            (
                registry: new FooCommandRegistry(),
                broker: new DelegatingCommandBroker(delegatex)
            ));
            return instance;
        }

    }
}
