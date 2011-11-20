using System;
using Microsoft.ApplicationServer.Http;
using Microsoft.ApplicationServer.Http.Description;
using WebApiContrib.Formatters.JsonNet;
using Zaz.Server.Advanced.Service;

namespace Zaz.Server.Advanced
{
    public static class ConfigurationHelper
    {
        //[Obsolete("Use CreateServiceConfiguration")]
        public static HttpConfiguration CreateConfiguration(CommandsService service)
        {
           
        	var config = new HttpConfiguration();
        	config.CreateInstance = (t, c, m) => service;        	

            config.Formatters.Insert(0, new JsonNetFormatter());
            config.MaxReceivedMessageSize = 16777216;
            config.MaxBufferSize = 16777216;

            return config;
        }                
        
        public static HttpConfiguration CreateServiceConfiguration(ServerConfiguration configuration)
        {
            var service = CreateService(configuration);

            var httpConfig = CreateConfiguration(service);

            if (configuration.ConfigureHttp != null)
            {
                configuration.ConfigureHttp(httpConfig);
            }

            return httpConfig;
        }

        public static CommandsService CreateService(ServerConfiguration configuration)
        {
            var context = new ServerContext(configuration.Registry, configuration.Broker, configuration.StateProvider);

            var service = new CommandsService(context);
            return service;
        }
    }    
}
