﻿using System;
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

        private const string URL = "http://localhost.fiddler:9303/Commands/";

        [TestFixtureSetUp]
        public void Given_command_server_runnig()
        {
            var instance = new CommandsService(new ServerContext
            {
                Registry = new ReflectionCommandRegistry(typeof(__SampleCommandsMarker).Assembly),
                Broker = new ReflectionCommandBroker(typeof(__SampleHandlersMarker).Assembly)
            });            
            var config = HttpHostConfigurationHelper.CreateConfiguration(instance);
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
            var bus = new CommandBus(URL);
            bus.Post(new PrintMessage
            {
                Message = "Hello world"
            });
        }
    }
}
