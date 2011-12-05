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
    public class When_posting_command_to_non_existing_server
    {
        HttpServiceHost _host;

        static readonly string URL = "http://" + FortyTwo.LocalHost + ":9303/NonExistingCommands/";        

        object _postedCommand;
        CommandHandlingContext _ctx;
        Exception _resultEx;

        [TestFixtureSetUp]
        public void Given_command_server_runnig()
        {           
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
        
        [Test]
        public void Should_throw_exception()
        {
            _resultEx.Should().NotBeNull();
        }

        [Test]
        public void Should_be_zaz_exception()
        {
            _resultEx.Should().BeAssignableTo<ZazException>();
        }        
    }
}
