using System;
using Microsoft.ApplicationServer.Http.Dispatcher;
using NUnit.Framework;
using Zaz.Server;
using Zaz.Server.Advanced.Registry;
using Zaz.Server.Advanced.Service;
using Zaz.Tests.Server.Stubs;
using FluentAssertions;

namespace Zaz.Tests.Server.Service
{
    public class When_resolving_command
    {
        private CommandResolver _commandResolver;

        [TestFixtureSetUp]
        public void Given_test_registry_with_some_items()
        {
            var registry = new SimpleCommandRegistry
                               {
                                   new CommandInfo
                                       {
                                           Key = "Some.Key",
                                           Aliases = new[] {"Key1", "Key2"},
                                           Type = new TypeStub("Key", "Some")
                                       },

                                    new CommandInfo
                                       {
                                           Key = "Some.AnotherKey",
                                           Aliases = new[] {"AnKey1"},
                                           Type = new TypeStub("AnotherKey", "Some")
                                       },

                                    new CommandInfo
                                       {
                                           Key = "Some.FooKey",
                                           Aliases = new[] {"FooKey"},
                                           Type = new TypeStub("FooKey", "Some")
                                       },
                               };

            _commandResolver = new CommandResolver(new Conventions
                                                       {
                                                           Registry = registry
                                                       });
        }

        [Test]
        public void Should_resolve_by_key()
        {
            var type = _commandResolver.ResolveCommandType("Some.Key");
            type.FullName.Should().Be("Some.Key");
        }

        [Test]
        public void Should_resolve_by_key_and_igonore_case()
        {
            var type = _commandResolver.ResolveCommandType("SOME.KEY");
            type.FullName.Should().Be("Some.Key");
        }

        [Test]
        public void Should_resolve_by_alias()
        {
            var type = _commandResolver.ResolveCommandType("AnKey1");
            type.FullName.Should().Be("Some.AnotherKey");
        }

        [Test]
        public void Should_resolve_by_part_of_the_key()
        {
            var type = _commandResolver.ResolveCommandType("AnotherKey");
            type.FullName.Should().Be("Some.AnotherKey");
        }

        [Test]
        public void Should_resolve_by_part_of_the_key_and_ignore_case()
        {
            var type = _commandResolver.ResolveCommandType("ANOTHERKEY");
            type.FullName.Should().Be("Some.AnotherKey");
        }

        [Test]
        public void Should_fail_to_resolve_if_more_then_one()
        {
            Action resolve = () => {
                var type = _commandResolver.ResolveCommandType("Key");                
            };

            resolve.ShouldThrow<HttpResponseException>();
        }

        [Test]
        public void Should_fail_to_resolve_if_nothing_is_matched()
        {
            Action resolve = () =>
            {
                var type = _commandResolver.ResolveCommandType("NotToBeFoundEver");
            };

            resolve.ShouldThrow<HttpResponseException>();
        }

        [Test]
        public void Should_resolve_when_both_alias_and_key_matches()
        {
            var type = _commandResolver.ResolveCommandType("FooKey");
            type.FullName.Should().Be("Some.FooKey");
        }        
    }
}
