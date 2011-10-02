using System;
using System.Net;
using Microsoft.ApplicationServer.Http;
using Microsoft.ApplicationServer.Http.Activation;
using NUnit.Framework;
using FluentAssertions;
using Zaz.Server.Advanced;
using Zaz.Server.Advanced.Service;

namespace Zaz.Tests.Integration
{
    public class When_connecting_to_server
    {
        private HttpServiceHost _host;

        [TestFixtureSetUp]
        public void Given_command_server_runnig()
        {
            var instance = new CommandsService();           
            var config = HttpHostConfigurationHelper.CreateHostConfigurationBuilder(instance);
            _host = new HttpServiceHost(typeof(CommandsService), config, new Uri("http://localhost:9303/Commands"));
            _host.Open();
        }

        [TestFixtureTearDown]
        public void Cleanup()
        {            
            _host.Close();
        }

        [Test]
        public void Should_return_home_page()
        {
            var client = new WebClient();
            client.DownloadString("http://localhost:9303/Commands").Should().Contain("Zaz");
        }
    }
}
