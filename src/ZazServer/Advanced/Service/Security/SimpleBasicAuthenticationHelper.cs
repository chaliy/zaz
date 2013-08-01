using System;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Zaz.Server.Advanced.Service.Security
{
    // Used this http://www.west-wind.com/weblog/posts/2013/Apr/30/A-WebAPI-Basic-Authentication-MessageHandler
    public static class SimpleBasicAuthenticationHelper
    {
        public static void SetupBasicAuthentications(this HttpConfiguration http, string username, string password)
        {
            http.Filters.Add(new SimpleAuthentication(username, password));
        }

        private class SimpleAuthentication : AuthorizeAttribute
        {
            readonly string _username;
            readonly string _password;

            public SimpleAuthentication(string username, string password)
            {
                _username = username;
                _password = password;
            }

            public override void OnAuthorization(HttpActionContext actionContext)
            {
                var controllerType = actionContext.ControllerContext.Controller.GetType();

                if (controllerType != typeof(CommandsController))
                {
                    HandleUnauthorizedRequest(actionContext);
                    return;
                }

                var identity = ParseAuthorizationHeader(actionContext.Request);

                if (identity == null || !identity.IsAuthenticated)
                {
                    HandleUnauthorizedRequest(actionContext);
                    return;
                }

                var isAuthorized = identity.Name == _username && identity.Password == _password;

                if (!isAuthorized)
                {
                    HandleUnauthorizedRequest(actionContext);
                    return;
                }

                var principal = new GenericPrincipal(identity, new string[] { });

                // Self-hosted mode don't have access to HttpContext instance 
                // which is available only in web-hosted mode.
                if (HttpContext.Current != null)
                    HttpContext.Current.User = principal;

                // But in any case, checking the base class sources some thread-based 
                // principal usage been found.
                Thread.CurrentPrincipal = principal;
            }

            static BasicAuthenticationIdentity ParseAuthorizationHeader(HttpRequestMessage request)
            {
                string authHeader = null;
                var auth = request.Headers.Authorization;
                if (auth != null && auth.Scheme == "Basic")
                    authHeader = auth.Parameter;

                if (string.IsNullOrEmpty(authHeader))
                    return null;

                authHeader = Encoding.Default.GetString(Convert.FromBase64String(authHeader));

                var tokens = authHeader.Split(':');
                if (tokens.Length < 2)
                    return null;

                return new BasicAuthenticationIdentity(tokens[0], tokens[1]);
            }
        }

        private class BasicAuthenticationIdentity : GenericIdentity
        {
            public BasicAuthenticationIdentity(string name, string password)
                : base(name, "Basic")
            {
                Password = password;
            }

            public string Password { get; private set; }
        }
    }
}