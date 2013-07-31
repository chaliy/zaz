﻿using System.Linq;
using System.Net;
using System.Net.Http;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Zaz.Server.Advanced;
using Zaz.Server.Advanced.Service;
using Zaz.Server.Advanced.Service.Contract;
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
            var service = new CommandsController(new ServerContext(broker: _broker));
            var cmdKey = typeof(FooCommand).FullName;
            var cmdData = new JObject
            {
                {"Value1", "Foo"}
            };
            _result = service.Default(new PostCommandRequest
            {
                Key = cmdKey,
                Command = cmdData
            });
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
            _broker.HandledCommands.Should().Contain(x => x.GetType() == typeof(FooCommand));
        }

        [Test]
        public void Should_post_correctly_deserialized_command()
        {
            var cmd = _broker.HandledCommands.OfType<FooCommand>().First();
            cmd.Value1.Should().Be("Foo");
        }
    }
}
