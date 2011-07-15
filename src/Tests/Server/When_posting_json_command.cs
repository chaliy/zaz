using System.Linq;
using System.Net;
using System.Net.Http;
using NUnit.Framework;
using FluentAssertions;
using Zaz.Server;
using Zaz.Server.Advanced;
using Zaz.Server.Advanced.Service;
using Zaz.Tests.Server.Stubs;

namespace Zaz.Tests.Server
{
    public class When_posting_json_command
    {        
        private HttpResponseMessage _result;
        private CommandBrokerStub _broker;
        
        [TestFixtureSetUp]
        public void Given_service_by_default()
        {
            _broker = new CommandBrokerStub();
            var service = new CommandsService(new Conventions { CommandBroker = _broker });
            var cmdKey = typeof (FooCommand).FullName;            
            var cmdMessage = new HttpRequestMessage
                {
                    Content = new StringContent(@"
                    { 
	                    'Key' : '" + cmdKey + @"',			
	                    'Command' : {
		                    'Value1' : 'Foo'
	                    }
                    }")
                };
            _result = service.Post(cmdMessage);
        }

        [Test]
        public void Should_accept_command()
        {
            _result.Should().NotBeNull();
            _result.StatusCode.Should().Be(HttpStatusCode.Accepted);
        }

        [Test]
        public void Should_post_command()
        {
            _broker.HandledCommands.Should().Contain(x => x.GetType() == typeof (FooCommand));
        }

        [Test]
        public void Should_post_correctly_deserialized_command()
        {
            var cmd = _broker.HandledCommands.OfType<FooCommand>().First();
            cmd.Value1.Should().Be("Foo");
        }
    }
}
