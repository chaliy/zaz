using System;
using Microsoft.ApplicationServer.Http;
using Microsoft.ApplicationServer.Http.Activation;
using NUnit.Framework;
using SampleCommands;
using Zaz.Client.Avanced;
using Zaz.Server;
using Zaz.Server.Advanced;
using Zaz.Server.Advanced.Registry;
using Zaz.Server.Advanced.Service;

namespace Zaz.Tests.Integration.CustomBroker
{
    public class When_posting_long_command_to_server
    {
        private HttpServiceHost _host;

        [TestFixtureSetUp]
        public void Given_command_server_runnig()
        {
            var instance = new CommandsService(new Conventions
            {
                Registry = new ReflectionCommandRegistry(typeof(__SampleCommandsMarker).Assembly),
                Broker = new LongCommandBroker()
            });            
            var config = HttpHostConfigurationHelper.CreateHostConfigurationBuilder(instance);            
            _host = new HttpServiceHost(typeof(CommandsService), config, new Uri("http://localhost:9303/LongCommands"));
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
            var bus = new AdvancedCommandBus("http://localhost:9303/LongCommands/");
            bus.PostScheduled(new CommandEnvelope
                          {
                              Key = "SampleCommands.PrintMessage"
                          }).Wait();            
        }
    }
}
