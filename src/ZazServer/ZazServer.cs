using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.SelfHost;
using WebApiContrib.Formatters.JsonNet;
using Zaz.Server.Advanced;
using Zaz.Server.Advanced.Service;

namespace Zaz.Server
{
    public static class ZazServer
    {
        public static HttpConfiguration ConfigureAsWebHost(string prefix = "Commands/", ServerConfiguration configuration = null)
        {
            var config = GlobalConfiguration.Configuration;

            Configure(config, prefix, configuration);

            return config;
        }

        public static HttpSelfHostConfiguration ConfigureAsSelfHosted(string url, ServerConfiguration configuration = null)
        {
            var config = new HttpSelfHostConfiguration(url);

            Configure(config, "", configuration);

            return config;
        }


        // Yep, all these templates could be moved to the controller itself
        // to describe them more conventionaly in some way. 
        // But for now this solution id good enough. 
        // Just had to focus on the other things.
        static void Configure(HttpConfiguration httpConfig, string prefix = "Commands/", ServerConfiguration serverConfig = null)
        {
            prefix = prefix ?? "";
            var configuration = serverConfig ?? new ServerConfiguration();

            if (!prefix.EndsWith("/"))
                prefix += "/";

            if (prefix.StartsWith("/"))
                prefix = prefix.Substring(1, prefix.Length - 1);

            var config = httpConfig;

            // POST

            config.Routes.MapHttpRoute(
                name: "ZazCommands/PostLegacy",
                routeTemplate: prefix + "Legacy",
                defaults: new { controller = "Commands", action = "PostLegacy" }
            );

            config.Routes.MapHttpRoute(
                name: "ZazCommands/PostScheduled",
                routeTemplate: prefix + "Scheduled",
                defaults: new { controller = "Commands", action = "PostScheduled" }
            );

            // GET

            config.Routes.MapHttpRoute(
                name: "ZazCommands/GetScheduled",
                routeTemplate: prefix + "Scheduled/{id}",
                defaults: new { controller = "Commands", action = "GetScheduled" }
            );

            config.Routes.MapHttpRoute(
                name: "ZazCommands/GetScheduledLog",
                routeTemplate: prefix + "Scheduled/Log/{id}",
                defaults: new { controller = "Commands", action = "GetScheduledLog" }
            );

            // DEFAULTS

            config.Routes.MapHttpRoute(
                name: "ZazCommands/Commands",
                routeTemplate: prefix + "{action}",
                defaults: new { controller = "Commands" }
            );

            config.Routes.MapHttpRoute(
                name: "ZazCommands/Default",
                routeTemplate: prefix + "{*path}",
                defaults: new { controller = "Commands", action = "Default", path = RouteParameter.Optional }
            );

            var activator = (IHttpControllerActivator)config.Services.GetService(typeof(IHttpControllerActivator));
            config.Services.Replace(typeof(IHttpControllerActivator), new SimpleControllerActicator(activator, new ServerContext(configuration.Registry, configuration.Broker, configuration.StateProvider)));

            if (configuration.ConfigureHttp != null)
            {
                configuration.ConfigureHttp(httpConfig);
            }

            SetUp(config);
        }

        static void SetUp(HttpConfiguration config)
        {
            // Replace JsonFormatter
            config.Formatters.Remove(config.Formatters.JsonFormatter);
            config.Formatters.Add(new JsonNetFormatter2());

            var selfHosted = config as HttpSelfHostConfiguration;

            if (selfHosted != null)
            {
                selfHosted.MaxReceivedMessageSize = 16777216;
                selfHosted.MaxBufferSize = 16777216;
            }

            // For web hosted services set these parameters within web.config file
            //    <configuration>
            //        <system.web>
            //        <httpRuntime maxRequestLength="16384" requestLengthDiskThreshold="16384"/>
            //        </system.web>
            //    </configuration>
        }

        class SimpleControllerActicator : IHttpControllerActivator
        {
            readonly IHttpControllerActivator _activator;
            readonly ServerContext _context;

            public SimpleControllerActicator(IHttpControllerActivator activator, ServerContext context)
            {
                _activator = activator;
                _context = context;
            }

            public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
            {
                if (controllerType == typeof(CommandsController))
                    return new CommandsController(_context);

                return _activator.Create(request, controllerDescriptor, controllerType);
            }
        }
    }
}
