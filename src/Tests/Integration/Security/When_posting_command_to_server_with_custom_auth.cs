using System;
using System.Linq;
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
    public class When_posting_command_to_server_with_custom_auth
    {
        HttpSelfHostServer _host;

        const string Prefix = "Commands1";

        static readonly string URL = "http://" + FortyTwo.LocalHost + ":9303/Application/";
        static readonly string URL1 = URL + Prefix + "/";

        object _postedCommand;

        [TestFixtureSetUp]
        public void Given_command_server_runnig()
        {
            var config = new HttpSelfHostConfiguration(URL);

            ZazServer.Configure(config, Prefix, new ServerConfiguration
            {
                Registry = new FooCommandRegistry(),
                Broker = new DelegatingCommandBroker((cmd, ctx) =>
                {
                    _postedCommand = cmd;
                    return Task.Factory.StartNew(() => { });
                }),
                ConfigureHttp = c => c.SetupCustomAuthentication(request =>
                {
                    var apiToken = request.Headers.GetValues("X-Api-Token").FirstOrDefault();
                    return apiToken == "ValidToken";

                }, Prefix)
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
        public void Should_execute_the_command_with_valid_tokent_attached()
        {
            var bus = new ZazClient(URL1, new ZazConfiguration
            {
                ConfigureDefaultHeaders = c => c.Add("X-Api-Token", "ValidToken")
            });
            bus.PostAsync(new FooCommand { Message = "Hello world" }).Wait();

            _postedCommand.Should().NotBeNull();
        }

        [Test]
        public void Should_fail_when_token_is_not_attached()
        {
            var bus = new ZazClient(URL1, new ZazConfiguration
            {
                ConfigureDefaultHeaders = c => c.Add("X-Api-Token", "InvalidToken")
            });

            Action res = () => bus.PostAsync(new FooCommand { Message = "Hello world" }).Wait();

            res.ShouldThrow<Exception>();
        }
    }
}