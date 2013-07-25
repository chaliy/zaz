using System.Security.Principal;
using System.Threading;
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
    public class When_posting_command_to_server_with_basic_auth
    {
        private HttpSelfHostServer _host;

        private static readonly string URL = "http://" + FortyTwo.LocalHost + ":9303/BasicAuthCommands/";

        private object _postedCommand;
        private CommandHandlingContext _ctx;
        IPrincipal _principal;

        [TestFixtureSetUp]
        public void Given_command_server_runnig()
        {
            var serverConfiguration = new ServerConfiguration
            {
                Registry = new FooCommandRegistry(),
                Broker = new DelegatingCommandBroker((cmd, ctx) =>
                {
                    _postedCommand = cmd;
                    _ctx = ctx;

                    _principal = Thread.CurrentPrincipal;

                    return Task.Factory.StartNew(() => { });
                }),
                ConfigureHttp = http => http.SetupBasicAuthentications("supr", "booper")
            };

            var config = ZazServer.ConfigureAsSelfHosted(URL, serverConfiguration);
            _host = new HttpSelfHostServer(config);

            //SimpleBasicAuthenticationHandler.Configure(config, cred => cred.UserName == "supr" && cred.Password == "booper");

            using (new HttpSelfHostServer(config))
            {
                _host.OpenAsync().Wait();

                // Client side
                var configuration = new ZazConfiguration();
                configuration.SetupSimpleBasicAuthentication("supr", "booper");

                var client = new ZazClient(URL, configuration);

                client.Post(new FooCommand
                {
                    Message = "Hello world"
                });
            }
        }

        [Test]
        public void Should_successfully_send_command()
        {
            _postedCommand.Should().NotBeNull();
        }

        [Test]
        public void Should_be_authenticated()
        {
            _principal.Identity.IsAuthenticated.Should().BeTrue();
        }

        [Test]
        public void Should_be_authenticated_with_correct_user()
        {
            _principal.Identity.Name.Should().Be("supr");
        }
    }
}
