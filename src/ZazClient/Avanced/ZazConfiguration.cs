using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Zaz.Client.Avanced
{
    public class ZazConfiguration
    {
        public Action<HttpClientHandler> ConfigureHttp { get; set; }
        public Action<HttpRequestHeaders> ConfigureDefaultHeaders { get; set; }
    }

    public static class SimpleBasicAuthenticationHelper
    {
        public static void SetupSimpleBasicAuthentication(this ZazConfiguration config, string username, string password)
        {
            var configurator = config.ConfigureDefaultHeaders;

            config.ConfigureDefaultHeaders = headers =>
            {
                if (configurator != null)
                    configurator(headers);

                var data = Convert.ToBase64String(Encoding.Default.GetBytes(String.Format("{0}:{1}", username, password)));
                headers.Authorization = new AuthenticationHeaderValue("Basic", data);
            };
        }
    }
}
