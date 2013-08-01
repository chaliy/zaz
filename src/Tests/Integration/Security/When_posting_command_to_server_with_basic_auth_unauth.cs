using System;
using System.Threading.Tasks;
using System.Web.Http.SelfHost;
using FluentAssertions;
using NUnit.Framework;
using Zaz.Client;
using Zaz.Client.Avanced;
using Zaz.Server;
using Zaz.Server.Advanced;
using Zaz.Server.Advanced.Broker;
using Zaz.Server.Advanced.Service.Security;
using Zaz.Tests.Stubs;

namespace Zaz.Tests.Integration.Security
{
    public class When_posting_command_to_server_with_basic_auth_unauth
    {
        private HttpSelfHostServer _host;

        private static readonly string URL = "http://" + FortyTwo.LocalHost + ":9303/BasicUnAuthCommands/";

        [TestFixtureSetUp]
        public void Given_command_server_runnig()
        {
            var serverConfiguration = new ServerConfiguration
            {
                Registry = new FooCommandRegistry(),
                Broker = new DelegatingCommandBroker((cmd, ctx) =>
                {
                    return Task.Factory.StartNew(() => { });
                }),
                ConfigureHttp = http => http.SetupBasicAuthentication("supr", "booper", "")
            };

            var config = ZazServer.ConfigureAsSelfHosted(URL, serverConfiguration);

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
            Action send = () =>
            {
                var configuration = new ZazConfiguration();
                configuration.SetupSimpleBasicAuthentication("user", "unknown");
                var bus = new ZazClient(URL, configuration);

                bus.Post(new FooCommand
                {
                    Message = "Hello world"
                });
            };

            send.ShouldThrow<Exception>();
        }
    }
}
