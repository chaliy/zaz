using System;
using System.Threading.Tasks;
using System.Web.Http.SelfHost;
using FluentAssertions;
using NUnit.Framework;
using Zaz.Client;
using Zaz.Server;
using Zaz.Server.Advanced;
using Zaz.Server.Advanced.Broker;
using Zaz.Tests.Stubs;

namespace Zaz.Tests.Integration.Errors
{
    public class When_posting_command_that_fails
    {
        static readonly string URL = "http://" + FortyTwo.LocalHost + ":9303/FailingCommands/";

        string _resultLog;

        [TestFixtureSetUp]
        public void Given_command_server_runnig()
        {
            var config = ZazServer.ConfigureAsSelfHosted(URL, new ServerConfiguration
            {
                Registry = new FooCommandRegistry(),
                Broker = new DelegatingCommandBroker((cmd, ctx) => Task.Factory.StartNew(() =>
                {
                    throw new InvalidOperationException("Something wrong in this life");
                }))
            });

            using (var host = new HttpSelfHostServer(config))
            {
                host.OpenAsync().Wait();

                // Client side
                var bus = new ZazClient(URL);
                _resultLog = bus.PostAsync(new FooCommand
                {
                    Message = "Hey!, anybody out there?"
                }).Result;
            }
        }

        [Test]
        public void Should_return_error_message()
        {
            _resultLog.Should().Contain("Something wrong in this life");
            _resultLog.Should().Contain("Error");
        }
    }
}
