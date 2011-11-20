using System;
using System.Net;
using Microsoft.ApplicationServer.Http;
using Microsoft.ApplicationServer.Http.Activation;
using NUnit.Framework;
using SampleCommands;
using SampleHandlers;
using Zaz.Client;
using Zaz.Server;
using Zaz.Server.Advanced;
using Zaz.Server.Advanced.Broker;
using Zaz.Server.Advanced.Registry;
using Zaz.Server.Advanced.Service;
using Zaz.Server.Advanced.Service.Security;
using FluentAssertions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using Zaz.Tests.Stubs;

namespace Zaz.Tests.Integration.Security
{
    public class When_posting_command_to_server_with_basic_auth_unauth
    {
        private HttpServiceHost _host;

        private const string URL = "http://localhost.fiddler:9303/BasicUnAuthCommands/";        

        [TestFixtureSetUp]
        public void Given_command_server_runnig()
        {
            var instance = new CommandsService(new ServerContext(
                registry: new FooCommandRegistry(),
                broker: new DelegatingCommandBroker((cmd, ctx) => {                    
                    return Task.Factory.StartNew(() =>
                    {                        
                    });
                })));
            
            var config = ConfigurationHelper.CreateConfiguration(instance);

            SimpleBasicAuthenticationHandler.Configure(config, cred => false);

            _host = new HttpServiceHost(typeof(CommandsService), config, new Uri(URL));
            _host.Open();            
        }

        [TestFixtureTearDown]
        public void Cleanup()
        {            
            _host.Close();
        }

        [Test]
        public void Should_successfully_send_command()
        {
            Action send = () =>
            {
                var bus = new CommandBus(URL, new Client.Avanced.ZazConfiguration
                {
                    ConfigureHttp = h =>
                    {
                        h.Credentials = new NetworkCredential("supr", "booper");
                    }
                });
                bus.Post(new FooCommand
                {
                    Message = "Hello world"
                });
            };

            send.ShouldThrow<Exception>();
        }
    }
}
