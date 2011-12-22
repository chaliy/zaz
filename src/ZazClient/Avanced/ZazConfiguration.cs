using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Zaz.Client.Avanced
{
    public class ZazConfiguration
    {
        public Action<WebRequestHandler> ConfigureHttp { get; set; }
        public Action<HttpRequestHeaders> ConfigureDefaultHeaders { get; set; }
    }
}
