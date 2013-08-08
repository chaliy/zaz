using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.SelfHost;
using FluentAssertions;
using NUnit.Framework;
using Zaz.Client;
using Zaz.Server;
using Zaz.Server.Advanced;
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
            var config = ZazServer.ConfigureAsSelfHosted(URL, new ServerConfiguration
            {
                Registry = new FooCommandRegistry(),
                Broker = new DelegatingCommandBroker((cmd, ctx) =>
                {
                    _postedCommand = cmd;
                    _ctx = ctx;

                    return Task.Factory.StartNew(() =>
                    {
                        ctx.Log.Info("Hello word! #1");
                        Thread.Sleep(TimeSpan.FromSeconds(1));

                        ctx.Log.Info("Hello word! #2");
                        Thread.Sleep(TimeSpan.FromSeconds(1));

                        ctx.Log.Info("Hello word! #3");
                        Thread.Sleep(TimeSpan.FromSeconds(1));

                        ctx.Log.Info("Hello word! #4");
                    });
                })
            });

            using (var host = new HttpSelfHostServer(config))
            {
                host.OpenAsync().Wait();

                var bus = new ZazClient(URL);
                bus.PostAsync(new FooCommand
                {
                    Message = "Heeeeelllllloooooo!"
                }).Wait();
            }
        }

        [Test]
        public void Should_accept_command()
        {
            _postedCommand.Should().NotBeNull();
        }
    }
}
