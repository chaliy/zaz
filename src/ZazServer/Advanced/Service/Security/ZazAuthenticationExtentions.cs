using System;
using System.Net.Http;
using System.Web.Http;

namespace Zaz.Server.Advanced.Service.Security
{
    // This 'prefix' parameter looks ugly. It will be fixed in next builds
    public static class ZazAuthenticationExtentions
    {
        public static void SetupBasicAuthentication(this HttpConfiguration http, string username, string password, string prefix)
        {
            http.Filters.Add(new SimpleBasicAuthenticationFilter(username, password, prefix));
        }

        public static void SetupCustomAuthentication(this HttpConfiguration http, Func<HttpRequestMessage, bool> isValid, string prefix)
        {
            http.Filters.Add(new CustomAuthenticationFilter(isValid, prefix));
        }
    }
}