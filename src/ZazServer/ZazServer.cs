using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.Routing;
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
        public static void Configure(HttpConfiguration httpConfig, string prefix = "Commands/", ServerConfiguration serverConfig = null)
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
                name: prefix + "ZazCommands/PostLegacy",
                routeTemplate: prefix + "Legacy",
                defaults: new { x_zaz_prefx = prefix, controller = "Commands", action = "PostLegacy" }
            );

            config.Routes.MapHttpRoute(
                name: prefix + "ZazCommands/PostScheduled",
                routeTemplate: prefix + "Scheduled",
                defaults: new { x_zaz_prefx = prefix, controller = "Commands", action = "PostScheduled" }
            );

            // GET

            config.Routes.MapHttpRoute(
                name: prefix + "ZazCommands/GetScheduled",
                routeTemplate: prefix + "Scheduled/{id}",
                defaults: new { x_zaz_prefx = prefix, controller = "Commands", action = "GetScheduled" }
            );

            config.Routes.MapHttpRoute(
                name: prefix + "ZazCommands/GetScheduledLog",
                routeTemplate: prefix + "Scheduled/Log/{id}",
                defaults: new { x_zaz_prefx = prefix, controller = "Commands", action = "GetScheduledLog" }
            );

            // DEFAULTS

            config.Routes.MapHttpRoute(
                name: prefix + "ZazCommands/Commands",
                routeTemplate: prefix + "{action}",
                defaults: new { x_zaz_prefx = prefix, controller = "Commands" }
            );

            config.Routes.MapHttpRoute(
                name: prefix + "ZazCommands/Default",
                routeTemplate: prefix + "{*path}",
                defaults: new { x_zaz_prefx = prefix, controller = "Commands", action = "Default", path = RouteParameter.Optional }
            );

            var nestedActivator = (IHttpControllerActivator)config.Services.GetService(typeof(IHttpControllerActivator));

            var serverContext = new ServerContext(
                configuration.Registry,
                configuration.Broker,
                configuration.StateProvider
            );

            var controllerActivator = new SimpleControllerActicator(prefix, nestedActivator, serverContext);
            config.Services.Replace(typeof(IHttpControllerActivator), controllerActivator);


            if (configuration.ConfigureHttp != null)
                configuration.ConfigureHttp(httpConfig);

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
            readonly string _prefix;
            readonly IHttpControllerActivator _nestedActivator;
            readonly ServerContext _context;

            public SimpleControllerActicator(string prefix, IHttpControllerActivator nestedActivator, ServerContext context)
            {
                _prefix = prefix;
                _nestedActivator = nestedActivator;
                _context = context;
            }

            public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
            {
                if (controllerType == typeof(CommandsController) && PrefixMatches(request))
                    return new CommandsController(_context);

                return _nestedActivator.Create(request, controllerDescriptor, controllerType);
            }

            bool PrefixMatches(HttpRequestMessage request)
            {
                if (!request.Properties.ContainsKey("MS_HttpRouteData"))
                    return false;

                var route = request.Properties["MS_HttpRouteData"] as IHttpRouteData;

                if (route == null)
                    return false;

                if (!route.Values.ContainsKey("x_zaz_prefx"))
                    return false;

                var prefix = (string)route.Values["x_zaz_prefx"];

                return prefix == _prefix;
            }
        }
    }
}
