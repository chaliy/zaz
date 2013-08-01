using System.Web.Http;
using System.Web.Http.Controllers;

namespace Zaz.Server.Advanced.Service.Security
{
    abstract class ZazAuthenticaion : AuthorizeAttribute
    {
        readonly string _prefix;

        protected ZazAuthenticaion(string prefix)
        {
            _prefix = ZazServer.NormalizePrefix(prefix);
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var controllerType = actionContext.ControllerContext.Controller.GetType();

            if (controllerType != typeof(CommandsController))
                return;

            var request = actionContext.Request;

            if (ZazServer.PrefixMatches(request, _prefix) == false)
                return;

            DoAuthorize(actionContext);
        }

        protected abstract void DoAuthorize(HttpActionContext actionContext);
    }
}