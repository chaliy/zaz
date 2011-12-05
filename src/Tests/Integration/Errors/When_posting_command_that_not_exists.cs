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
    public class When_posting_command_that_not_exists
    {
        HttpServiceHost _host;

        static readonly string URL = "http://" + FortyTwo.LocalHost + ":9303/NotExistingCommands/";        
        
        Exception _resultEx;

        [TestFixtureSetUp]
        public void Given_command_server_runnig()
        {
            var instance = new CommandsService(new ServerContext
            (
                registry: new FooCommandRegistry(),
                broker: new DelegatingCommandBroker((cmd, ctx) =>
                {
                    return Task.Factory.StartNew(() => {});
                })
            ));
            var config = ConfigurationHelper.CreateConfiguration(instance);

            using (_host = new HttpServiceHost(typeof(CommandsService), config, new Uri(URL)))
            {
                _host.Open();


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
