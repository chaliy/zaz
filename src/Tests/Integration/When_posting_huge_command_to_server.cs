using System;
using System.Web.Http.SelfHost;
using FluentAssertions;
using NUnit.Framework;
using SampleCommands;
using Zaz.Client;
using Zaz.Server;
using Zaz.Server.Advanced;
using Zaz.Server.Advanced.Registry;
using Zaz.Tests.Integration.CustomBroker;

namespace Zaz.Tests.Integration
{
    public class When_posting_huge_command_to_server
    {
        private NullCommandBroker _commandBroker;

        private static readonly string URL = "http://" + FortyTwo.LocalHost + ":9303/HugeCommands/";

        [TestFixtureSetUp]
        public void Given_command_server_runnig()
        {
            _commandBroker = new NullCommandBroker();

            var config = ZazServer.ConfigureAsSelfHosted(URL, new ServerConfiguration
            {
                Registry = new ReflectionCommandRegistry(typeof(__SampleCommandsMarker).Assembly),
                Broker = _commandBroker,
            });

            using (var host = new HttpSelfHostServer(config))
            {
                host.OpenAsync().Wait();

                var bus = new ZazClient(URL);
                bus.PostAsync(new HugeFoo
                {
                    Data = new String('a', 2097152)
                }).Wait();
            }
        }

        [Test]
        public void Should_successfully_send_command()
        {
            _commandBroker.CommandsPosted.Should().NotBeEmpty();
        }
    }
}
