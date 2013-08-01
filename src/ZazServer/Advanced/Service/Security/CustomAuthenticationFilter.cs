using System;
using System.Net.Http;
using System.Web.Http.Controllers;

namespace Zaz.Server.Advanced.Service.Security
{
    class CustomAuthenticationFilter : ZazAuthenticaion
    {
        readonly Func<HttpRequestMessage, bool> _isValid;

        public CustomAuthenticationFilter(Func<HttpRequestMessage, bool> isValid, string prefix)
            : base(prefix)
        {
            _isValid = isValid;
        }

        protected override void DoAuthorize(HttpActionContext actionContext)
        {
            if (_isValid(actionContext.Request))
                return;

            HandleUnauthorizedRequest(actionContext);
        }
    }
}