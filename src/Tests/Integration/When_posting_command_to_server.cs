using System.Web.Http.SelfHost;
using NUnit.Framework;
using SampleCommands;
using SampleHandlers;
using Zaz.Client;
using Zaz.Server;
using Zaz.Server.Advanced;
using Zaz.Server.Advanced.Broker;
using Zaz.Server.Advanced.Registry;

namespace Zaz.Tests.Integration
{
    public class When_posting_command_to_server
    {
        private HttpSelfHostServer _host;

        private static readonly string URL = "http://" + FortyTwo.LocalHost + ":9303/SomeCommands/";

        [TestFixtureSetUp]
        public void Given_command_server_runnig()
        {
            var config = ZazServer.ConfigureAsSelfHosted(URL, new ServerConfiguration
            {
                Registry = new ReflectionCommandRegistry(typeof(__SampleCommandsMarker).Assembly),
                Broker = new ReflectionCommandBroker(typeof(__SampleHandlersMarker).Assembly),
            });

            _host = new HttpSelfHostServer(config);

            _host.OpenAsync().Wait();
        }

        [TestFixtureTearDown]
        public void Cleanup()
        {
            _host.CloseAsync().Wait();
        }

        [Test]
        public void Should_successfully_send_command()
        {
            var bus = new ZazClient(URL);
            bus.Post(new PrintMessage
            {
                Message = "Hello world"
            });
        }
    }
}
