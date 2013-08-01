
namespace Zaz.Server.Advanced
{

    // See ZazServer for details

    //    static class ConfigurationHelper
    //    {
    //        public static HttpSelfHostConfiguration CreateHttpSelfHostConfiguration(CommandsService service, string url)
    //        {
    //            var config = new HttpSelfHostConfiguration(url);
    //            SetUp(config, service);
    //            return config;
    //        }
    //
    //        //[Obsolete("Use CreateServiceConfiguration")]
    //        public static HttpConfiguration CreateConfiguration(CommandsService service)
    //        {
    //            var config = new HttpConfiguration();
    //            SetUp(config, service);
    //            return config;
    //        }
    //
    //        static void SetUp(HttpConfiguration config, CommandsService service)
    //        {
    //            //config.CreateInstance = (t, c, m) => service;
    //
    //            // Replace JsonFormatter
    //            config.Formatters.Remove(config.Formatters.JsonFormatter);
    //            config.Formatters.Add(new JsonNetFormatter2());
    //
    //            //            config.MaxReceivedMessageSize = 16777216;
    //            //            config.MaxBufferSize = 16777216;
    //        }
    //
    //        public static HttpConfiguration CreateServiceConfiguration(ServerConfiguration configuration)
    //        {
    //            var service = CreateService(configuration);
    //            var httpConfig = CreateConfiguration(service);
    //
    //            if (configuration.ConfigureHttp != null)
    //            {
    //                configuration.ConfigureHttp(httpConfig);
    //            }
    //
    //            return httpConfig;
    //        }
    //
    //        public static CommandsService CreateService(ServerConfiguration configuration)
    //        {
    //            var context = new ServerContext(configuration.Registry, configuration.Broker, configuration.StateProvider);
    //
    //            var service = new CommandsService(context);
    //            return service;
    //        }
    //    }
}
