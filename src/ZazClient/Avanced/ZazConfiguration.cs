using System;
using System.Net.Http;
namespace Zaz.Client.Avanced
{
    public class ZazConfiguration
    {
        public Action<WebRequestHandler> ConfigureHttp { get; set; }        
    }
}
