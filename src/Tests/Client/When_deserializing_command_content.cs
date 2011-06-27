using NUnit.Framework;
using Zaz.Client.Avanced;
using FluentAssertions;

namespace Zaz.Tests.Client
{    
    public class When_deserializing_command_content
    {
        private CommandContent _content;

        public class SomeCommand
        {
            public string Value1 { get; set; }
        }

        [TestFixtureSetUp]
        public void Given_command()
        {
            var envelope = new CommandEnvelope
            {
                Key = "Bar.SomeCommand",
                Command = new SomeCommand
                {
                    Value1 = "Foo"
                }
            };
            _content = new CommandContent(envelope);
        }

        [Test]
        public void Should_serialize_command_data()
        {
            _content.ReadAsString().Should().Contain("Value1");
            _content.ReadAsString().Should().Contain("Foo");
        }

        [Test]
        public void Should_add_key_of_the_command()
        {
            _content.ReadAsString().Should().Contain("Bar.SomeCommand");
        }
    }
}
