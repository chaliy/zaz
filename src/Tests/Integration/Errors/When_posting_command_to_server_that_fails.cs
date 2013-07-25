using System;
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
    public class When_posting_command_to_server_that_fails
    {

        static readonly string URL = "http://" + FortyTwo.LocalHost + ":9303/FailingServerCommands/";

        Exception _resultEx;

        [TestFixtureSetUp]
        public void Given_command_server_runnig()
        {
            var config = ZazServer.ConfigureAsSelfHosted(URL, new ServerConfiguration
            {
                Registry = new FooCommandRegistry(),
                Broker = new DelegatingCommandBroker((cmd, ctx) =>
                {
                    throw new InvalidOperationException("Server failed...");
                    // return Task.Factory.StartNew(() => { });
                })
            });

            using (var host = new HttpSelfHostServer(config))
            {
                host.OpenAsync().Wait();

                // Client side
                var bus = new ZazClient(URL);
                try
                {
                    bus.Post(new FooCommand
                    {
                        Message = "Hello world"
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

        [Test]
        public void Should_throw_with_details()
        {
            _resultEx.ToString().Should().Contain("Server failed...");
        }

        [Test]
        public void Should_be_zaz_exception()
        {
            _resultEx.Should().BeAssignableTo<ZazException>();
        }
    }
}
