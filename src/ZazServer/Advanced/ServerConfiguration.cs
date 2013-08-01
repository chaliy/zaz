using System;
using System.Web.Http;
using Zaz.Server.Advanced.Broker;
using Zaz.Server.Advanced.Registry;
using Zaz.Server.Advanced.State;

namespace Zaz.Server.Advanced
{
    public class ServerConfiguration
    {
        public Action<HttpConfiguration> ConfigureHttp { get; set; }

        public ICommandRegistry Registry { get; set; }
        public ICommandBroker Broker { get; set; }
        public ICommandStateProvider StateProvider { get; set; }
    }
}
