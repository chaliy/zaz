using System.Net;
using System.Web.Http.SelfHost;
using FluentAssertions;
using NUnit.Framework;
using Zaz.Server;

namespace Zaz.Tests.Integration
{
    public class When_connecting_to_server
    {
        private HttpSelfHostServer _host;

        private static readonly string URL = "http://" + FortyTwo.LocalHost + ":9303/OtherCommands/";

        [TestFixtureSetUp]
        public void Given_command_server_runnig()
        {
            var config = ZazServer.ConfigureAsSelfHosted(URL);

            _host = new HttpSelfHostServer(config);
            _host.OpenAsync().Wait();
        }

        [TestFixtureTearDown]
        public void Cleanup()
        {
            _host.CloseAsync().Wait();
        }

        [Test]
        public void Should_return_the_home_page_by_default_url()
        {
            var page = new WebClient().DownloadString(URL);
            page.Should().Contain("<!-- Zaz Command Bus Portal -->");
        }

        [Test]
        public void Should_return_the_resource_by_url()
        {
            var page = new WebClient().DownloadString(URL + "js/app.js");
            page.Should().Contain("/* Zaz Server Portal */");
        }
    }
}
