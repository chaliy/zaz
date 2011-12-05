using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.ApplicationServer.Http;
using Microsoft.ApplicationServer.Http.Activation;
using NUnit.Framework;
using SampleCommands;
using SampleHandlers;
using Zaz.Client;
using Zaz.Server;
using Zaz.Server.Advanced;
using Zaz.Server.Advanced.Broker;
using Zaz.Server.Advanced.Registry;
using Zaz.Server.Advanced.Service;
using Zaz.Server.Advanced.Service.Security;
using Zaz.Tests.Stubs;
using FluentAssertions;

namespace Zaz.Tests.Integration.Errors
{
    public class When_posting_command_that_fails
    {
        HttpServiceHost _host;

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
            var config = ConfigurationHelper.CreateConfiguration(instance);

            using (_host = new HttpServiceHost(typeof(CommandsService), config, new Uri(URL)))
            {
                _host.Open();


                // Client side
                var bus = new CommandBus(URL);
                _resultLog = bus.Post(new FooCommand
                {
                    Message = "Hey!, anybody out there?"
                });                
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
