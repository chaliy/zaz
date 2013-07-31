using System.Threading.Tasks;
using System.Web.Http.SelfHost;
using FluentAssertions;
using NUnit.Framework;
using Zaz.Client;
using Zaz.Server;
using Zaz.Server.Advanced;
using Zaz.Server.Advanced.Broker;
using Zaz.Tests.Stubs;

namespace Zaz.Tests.Integration
{
    public class When_configuring_multiple_endpoints_within_1_webhost
    {
        const string Prefix1 = "Commands1";
        const string Prefix2 = "Commands2";

        private static readonly string URL = "http://" + FortyTwo.LocalHost + ":9303/Application/";
        private static readonly string URL1 = URL + Prefix1 + "/";
        private static readonly string URL2 = URL + Prefix2 + "/";

        HttpSelfHostServer _host;

        FooCommand _api1Command;
        FooCommand _api2Command;

        [TestFixtureSetUp]
        public void SetUp()
        {
            var config = new HttpSelfHostConfiguration(URL);

            ZazServer.Configure(config, Prefix1, new ServerConfiguration
            {
                Registry = new FooCommandRegistry(),
                Broker = new DelegatingCommandBroker((o, context) =>
                {
                    _api1Command = (FooCommand)o;
                    return Task.Factory.StartNew(() => { });
                }),
            });
            ZazServer.Configure(config, Prefix2, new ServerConfiguration
            {
                Registry = new FooCommandRegistry(),
                Broker = new DelegatingCommandBroker((o, context) =>
                {
                    _api2Command = (FooCommand)o;
                    return Task.Factory.StartNew(() => { });
                }),
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
        public void Should_despatch_commands_between_controllers_by_prefixes()
        {
            var client1 = new ZazClient(URL1);
            var client2 = new ZazClient(URL2);

            var cmd1 = new FooCommand
            {
                Message = "Command #1"
            };
            var cmd2 = new FooCommand
            {
                Message = "Command #2"
            };

            client1.Post(cmd1);
            client2.Post(cmd2);

            _api1Command.Message.Should().Be(cmd1.Message);
            _api2Command.Message.Should().Be(cmd2.Message);
        }
    }
}