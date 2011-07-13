using System.Net;
using Microsoft.ApplicationServer.Http;
using NUnit.Framework;
using Zaz.Server.Service;
using FluentAssertions;

namespace Zaz.Tests.Integration
{
    public class When_connecting_to_server
    {
        private HttpServiceHost _host;

        [TestFixtureSetUp]
        public void Given_command_server_runnig()
        {
            var instance = new CommandsService();
            _host = new HttpServiceHost(instance, "http://localhost:9303/Commands");            
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
