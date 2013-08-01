using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Zaz.Client;
using Zaz.Server.Advanced;
using Zaz.Server.Advanced.Broker;
using Zaz.Server.Advanced.Service;
using Zaz.Tests.Stubs;

namespace Zaz.Tests.Integration.Errors
{
    public class When_posting_command_that_not_exists
    {
        static readonly string URL = "http://" + FortyTwo.LocalHost + ":9303/NotExistingCommands/";

        Exception _resultEx;

        [TestFixtureSetUp]
        public void Given_command_server_runnig()
        {
            var instance = new CommandsController(new ServerContext
            (
                registry: new FooCommandRegistry(),
                broker: new DelegatingCommandBroker((cmd, ctx) =>
                {
                    return Task.Factory.StartNew(() => { });
                })
            ));
            using (instance.OpenConfiguredServiceHost(URL))
            {

                // Client side
                var bus = new ZazClient(URL);
                try
                {
                    bus.Post(new NotExistingCommand
                    {
                    });
                }
                catch (Exception ex)
                {
                    _resultEx = ex;
                }
            }
        }

        [Test]
        public void Should_throw_exception()
        {
            _resultEx.Should().NotBeNull();
        }
    }
}
