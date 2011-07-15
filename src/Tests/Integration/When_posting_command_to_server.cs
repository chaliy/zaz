using System.Net;
using Microsoft.ApplicationServer.Http;
using NUnit.Framework;
using SampleCommands;
using SampleHandlers;
using Zaz.Client;
using Zaz.Server;
using FluentAssertions;
using Zaz.Server.Advanced;
using Zaz.Server.Advanced.Broker;
using Zaz.Server.Advanced.Registry;

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
                CommandBroker = new ReflectionCommandBroker(typeof(__SampleHandlersMarker).Assembly)
            });
            _host = new HttpServiceHost(instance, "http://localhost:9303/Commands");            
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
            var bus = new CommandBus("http://localhost:9303/Commands");
            bus.Post(new PrintMessage
            {
                Message = "Hello world"
            });
        }
    }
}
