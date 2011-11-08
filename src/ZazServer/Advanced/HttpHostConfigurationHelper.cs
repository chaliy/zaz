using Microsoft.ApplicationServer.Http;
using Microsoft.ApplicationServer.Http.Description;
using WebApiContrib.Formatters.JsonNet;
using Zaz.Server.Advanced.Service;

namespace Zaz.Server.Advanced
{
    public static class HttpHostConfigurationHelper
    {
        public static HttpConfiguration CreateHostConfigurationBuilder(CommandsService service)
        {
           
        	var config = new HttpConfiguration();
        	config.CreateInstance = (t, c, m) => service;        	

            config.Formatters.Insert(0, new JsonNetFormatter());            

            return config;
        }
    }
}
