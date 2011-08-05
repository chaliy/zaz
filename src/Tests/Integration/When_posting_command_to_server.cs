using System;
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

namespace Zaz.Tests.Integration
{
    public class When_posting_command_to_server
    {
        private HttpServiceHost _host;

        [TestFixtureSetUp]
        public void Given_command_server_runnig()
        {
            var instance = new CommandsService(new Conventions
            {
                CommandRegistry = new ReflectionCommandRegistry(typeof(__SampleCommandsMarker).Assembly),
                Broker = new ReflectionCommandBroker(typeof(__SampleHandlersMarker).Assembly)
            });            
            var config = HttpHostConfigurationHelper.CreateHostConfigurationBuilder(instance);
            _host = new HttpConfigurableServiceHost<CommandsService>(config, new Uri("http://localhost:9303/Commands/"));
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
            var bus = new CommandBus("http://localhost:9303/Commands/");
            bus.Post(new PrintMessage
            {
                Message = "Hello world"
            });
        }
    }
}
