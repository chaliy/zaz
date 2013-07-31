using System.Net.Http;
using FluentAssertions;
using NUnit.Framework;
using Zaz.Server.Advanced;
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
            var controller = new CommandsController(new ServerContext(broker: new CommandBrokerStub()));
            _result = controller.Default("");
        }

        [Test]
        public void Should_return_result()
        {
            _result.Should().NotBeNull();
        }

        [Test]
        public void Should_return_message()
        {
            var content = _result.Content.ReadAsStringAsync().Result;
            content.Should().Contain("<!-- Zaz Command Bus Portal -->");
        }
    }
}
