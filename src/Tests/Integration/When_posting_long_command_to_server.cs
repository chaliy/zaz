using System;
using Microsoft.ApplicationServer.Http;
using NUnit.Framework;
using SampleCommands;
using Zaz.Client.Avanced;
using Zaz.Server;
using Zaz.Server.Advanced;
using Zaz.Server.Advanced.Registry;
using Zaz.Server.Advanced.Service;
using Zaz.Tests.Integration.CustomBroker;

namespace Zaz.Tests.Integration
{
    public class When_posting_long_command_to_server
    {
        private HttpServiceHost _host;

        private static readonly string URL = "http://" + FortyTwo.LocalHost + ":9303/LongCommands/";

        [TestFixtureSetUp]
        public void Given_command_server_runnig()
        {
            var instance = new CommandsService(new ServerContext
            (
                registry: new ReflectionCommandRegistry(typeof(__SampleCommandsMarker).Assembly),
                broker: new LongCommandBroker()
            ));            
            var config = ConfigurationHelper.CreateConfiguration(instance);            
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
            var bus = new AdvancedCommandBus(URL);
            bus.PostScheduled(new CommandEnvelope
                          {
                              Key = "SampleCommands.PrintMessage"
                          }).Wait();            
        }
    }
}
