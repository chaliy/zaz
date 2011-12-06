using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Zaz.Client;
using Zaz.Server.Advanced.Broker;
using Zaz.Tests.Stubs;

namespace Zaz.Tests.Integration
{
    public class When_posting_long_command_to_server
    {        
        private object _postedCommand;
        private CommandHandlingContext _ctx;

        private static readonly string URL = "http://" + FortyTwo.LocalHost + ":9303/LongCommands/";

        [TestFixtureSetUp]
        public void Given_command_server_runnig()
        {
            var instance = Create.FooCommandsService((cmd, ctx) =>
            {
                _postedCommand = cmd;
                _ctx = ctx;

                return Task.Factory.StartNew(() =>
                {
                    ctx.Log.Info("Hello word! #1");
                    Thread.Sleep(1000);
                    ctx.Log.Info("Hello word! #2");
                    Thread.Sleep(1000);
                    ctx.Log.Info("Hello word! #3");
                    Thread.Sleep(1000);
                    ctx.Log.Info("Hello word! #4");
                });                
            });
            using(instance.OpenConfiguredServiceHost(URL))
            {
                var bus = new ZazClient(URL);
                bus.Post(new FooCommand
                {
                    Message = "Heeeeelllllloooooo!"
                });
            }
        }

        [Test]
        public void Should_accept_command()
        {
            _postedCommand.Should().NotBeNull();
        }
    }
}
