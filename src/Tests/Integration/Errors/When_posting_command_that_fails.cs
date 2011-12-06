using System;
using System.Threading.Tasks;
using Microsoft.ApplicationServer.Http;
using NUnit.Framework;
using Zaz.Client;
using Zaz.Client.Avanced.Logging;
using Zaz.Server.Advanced;
using Zaz.Server.Advanced.Broker;
using Zaz.Server.Advanced.Service;
using Zaz.Tests.Stubs;
using FluentAssertions;

namespace Zaz.Tests.Integration.Errors
{
    public class When_posting_command_that_fails
    {        
        static readonly string URL = "http://" + FortyTwo.LocalHost + ":9303/FailingCommands/";        
        
        string _resultLog;

        [TestFixtureSetUp]
        public void Given_command_server_runnig()
        {
            var instance = new CommandsService(new ServerContext
            (
                registry: new FooCommandRegistry(),
                broker: new DelegatingCommandBroker((cmd, ctx) =>
                {
                    return Task.Factory.StartNew(() =>
                    {
                        throw new InvalidOperationException("Something wrong in this life");
                    });
                })
            ));
            
            using (instance.OpenConfiguredServiceHost(URL))
            {

                // Client side
                var bus = new ZazClient(URL);
                var log = new ZazLogToStringAdapter();
                bus.Post(new FooCommand
                {
                    Message = "Hey!, anybody out there?"
                }, log: log);
                _resultLog = log.ToString();
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
