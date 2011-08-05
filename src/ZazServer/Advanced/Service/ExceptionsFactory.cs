using System.Net;
using System.Net.Http;
using Microsoft.ApplicationServer.Http.Dispatcher;

namespace Zaz.Server.Advanced.Service
{
    public static class ExceptionsFactory
    {
        public static HttpResponseException CreateApiException(string message)
        {
            var resp = new HttpResponseMessage(HttpStatusCode.BadRequest, message)
            {
                Content = new StringContent(message)
            };
            return new HttpResponseException(resp);
        }
    }
}
