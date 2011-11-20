using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.ApplicationServer.Http;
using Microsoft.ApplicationServer.Http.Dispatcher;

namespace Zaz.Server.Advanced.Service.Security
{
    // Authentication in WCF API are ... Well, not yet done
    // This code is compiled from all sources below
    // http://haacked.com/archive/2011/10/19/implementing-an-authorization-attribute-for-wcf-web-api.aspx
    // http://codebetter.com/howarddierking/2011/10/11/oauth-2-0-in-web-api/
    // http://webapicontrib.codeplex.com/
    public class SimpleBasicAuthenticationHandler : HttpOperationHandler<HttpRequestMessage, HttpRequestMessage>
    {        
        private const string Scheme = "Basic";

        private readonly string _realm;
        private readonly Func<NetworkCredential, bool> _checkCredentials;

        public SimpleBasicAuthenticationHandler(Func<NetworkCredential, bool> checkCredentials, string realm = "Zaz Command Bus")
            : base("response")
        {
            _checkCredentials = checkCredentials;
            _realm = realm;
        }
                        
        private static NetworkCredential ExtractCredentials(HttpRequestMessage request)
        {
            if (request.Headers.Authorization != null && request.Headers.Authorization.Scheme.StartsWith(Scheme))
            {
                string encodedUserPass = request.Headers.Authorization.Parameter.Trim();

                try
                {
                    var encoding = Encoding.GetEncoding("iso-8859-1");
                    var userPass = encoding.GetString(Convert.FromBase64String(encodedUserPass));
                    var separator = userPass.IndexOf(':');

                    var cred = new NetworkCredential(userPass.Substring(0, separator), userPass.Substring(separator + 1));

                    return cred;
                }
                catch (FormatException)
                {           
                }
            }

            return null;
        }

        private bool AuthenticateUser(NetworkCredential cred)
        {
            return _checkCredentials(cred);
        }

        private void Challenge()
        {
            var resp = new HttpResponseMessage(HttpStatusCode.Unauthorized);            
            resp.Content = new StringContent("Access denied");
            resp.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue(Scheme, "realm=" + _realm));

            throw new HttpResponseException(resp);            
        }

        protected override HttpRequestMessage OnHandle(HttpRequestMessage input)
        {
            var cred = ExtractCredentials(input);
            if (cred == null)
            {
                Challenge();
            }

            if (!AuthenticateUser(cred))
            {
                Challenge();                
            }
                        
            Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(cred.UserName), new string[] { });            

            return input;
        }


        public static void Configure(HttpConfiguration config, 
            Func<NetworkCredential, bool> checkCredentials,
            string realm = "Zaz Command Bus")
        {
            var requestHandlers = config.RequestHandlers;
            config.RequestHandlers = (c, e, od) =>
            {
                if (requestHandlers != null)
                {
                    requestHandlers(c, e, od); // Original request handler
                }

                c.Add(new SimpleBasicAuthenticationHandler(checkCredentials, realm));
            };
        }
    }
}