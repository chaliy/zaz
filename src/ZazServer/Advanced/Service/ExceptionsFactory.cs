using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Zaz.Server.Advanced.Service
{
    public static class ExceptionsFactory
    {
        public static HttpResponseException CreateApiException(string message)
        {
            var resp = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(message)
            };
            return new HttpResponseException(resp);
        }
    }
}
