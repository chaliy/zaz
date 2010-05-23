using System;
using System.Web;
using System.Web.Routing;
using CommandRouter;

namespace SampleApp.Framework
{

	public static class Registration
	{
		public static Class1 Reg()
		{
			//new CommandRoute();
		}
	}

	public class Class12 : RouteBase
	{
		public override RouteData GetRouteData(HttpContextBase httpContext)
		{
			throw new NotImplementedException();
		}

		public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
		{
			throw new NotImplementedException();
		}
	}

	public class Class1 : Route
	{
		public Class1(string url, IRouteHandler routeHandler) : base(url, routeHandler)
		{
		}

		public Class1(string url, RouteValueDictionary defaults, IRouteHandler routeHandler) : base(url, defaults, routeHandler)
		{
		}

		public Class1(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, IRouteHandler routeHandler) : base(url, defaults, constraints, routeHandler)
		{
		}

		public Class1(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, RouteValueDictionary dataTokens, IRouteHandler routeHandler) : base(url, defaults, constraints, dataTokens, routeHandler)
		{
		}
	}

	public class CommandsRouteHandler : IRouteHandler
	{
		public IHttpHandler GetHttpHandler(RequestContext requestContext)
		{
			throw new NotImplementedException();
		}
	}

	public class CommandsHttpHandler : IHttpHandler
	{
		public void ProcessRequest(HttpContext context)
		{
			throw new NotImplementedException();
		}

		public bool IsReusable
		{
			get { return false; }
		}
	}
}