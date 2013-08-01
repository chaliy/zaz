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
    public class When_setting_up_multiple_endpoints_with_auth
    {
        HttpSelfHostServer _host;

        const string Prefix1 = "Commands1";
        const string Prefix2 = "Commands2";

        static readonly string URL = "http://" + FortyTwo.LocalHost + ":9303/Application/";
        static readonly string URL1 = URL + Prefix1 + "/";
        static readonly string URL2 = URL + Prefix2 + "/";

        object _postedCommand;

        [TestFixtureSetUp]
        public void Given_command_server_runnig()
        {
            var endpoint1 = new ServerConfiguration
            {
                Registry = new FooCommandRegistry(),
                Broker = new DelegatingCommandBroker((cmd, ctx) =>
                {
                    _postedCommand = cmd;
                    return Task.Factory.StartNew(() => { });
                }),
            };

            var endpoint2 = new ServerConfiguration
            {
                Registry = new FooCommandRegistry(),
                Broker = new DelegatingCommandBroker((cmd, ctx) => Task.Factory.StartNew(() => { })),
                ConfigureHttp = http => http.SetupBasicAuthentication("supr", "booper", Prefix2)
            };

            var config = new HttpSelfHostConfiguration(URL);

            ZazServer.Configure(config, Prefix1, endpoint1);
            ZazServer.Configure(config, Prefix2, endpoint2);

            _host = new HttpSelfHostServer(config);
            _host.OpenAsync().Wait();
        }

        [TestFixtureTearDown]
        public void Cleanup()
        {
            _host.CloseAsync().Wait();
        }

        [Test]
        public void Should_get_access_denied_error_from_the_endpoint2()
        {
            Action send = () =>
            {
                var bus = new ZazClient(URL2, new ZazConfiguration());
                bus.Post(new FooCommand { Message = "Hello world" });
            };

            send.ShouldThrow<Exception>();
        }

        [Test]
        public void Should_execute_the_command_sent_trough_endpoint1()
        {
            var bus = new ZazClient(URL1, new ZazConfiguration());
            bus.Post(new FooCommand { Message = "Hello world" });

            _postedCommand.Should().NotBeNull();
        }
    }
}