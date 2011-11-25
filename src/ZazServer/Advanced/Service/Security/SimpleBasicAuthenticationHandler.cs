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
    public class SimpleBasicAuthenticationHandler : AbstractBasicAuthenticationHandler
    {                
        private readonly Func<NetworkCredential, bool> _checkCredentials;

        public SimpleBasicAuthenticationHandler(Func<NetworkCredential, bool> checkCredentials, string realm = "Zaz Command Bus")
            : base(realm)
        {
            _checkCredentials = checkCredentials;
        }

        protected override bool AuthenticateUser(HttpRequestMessage input, NetworkCredential cred)
        {
            return _checkCredentials(cred);
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