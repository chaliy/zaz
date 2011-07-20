using System.Net.Http;
using NUnit.Framework;
using FluentAssertions;
using Zaz.Server;
using Zaz.Server.Advanced.Service;
using Zaz.Tests.Server.Stubs;

namespace Zaz.Tests.Server
{
    public class When_access_to_endpoint
    {        
        private HttpResponseMessage _result;
        
        [TestFixtureSetUp]
        public void Given_service_by_default()
        {
            var service = new CommandsService(new Conventions { Broker = new CommandBrokerStub() });
            _result = service.Get("");            
        }

        [Test]
        public void Should_return_result()
        {
            _result.Should().NotBeNull();
        }

        [Test]
        public void Should_return_message()
        {
            _result.Content.ReadAsString().Should().Contain("commands");
        }
    }
}
