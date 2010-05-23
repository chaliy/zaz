using System.Web.Mvc;
using System.Web.Routing;
using CommandRouter;
using SampleApp.Framework;

namespace SampleApp
{	
	public class SampleApplication : System.Web.HttpApplication
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.Add(new CommandRoute(new CommandRouteBuilder()
				.Url("Commands")
				.CommandsResolveStrategy()));			

			//routes.Add(Registration.Reg()
			//        .Url("Commands")
			//        .CommandsResolveStrategy()
			//        .CommandHandlersResolveStrategy()
			//        .CommandHandlersCreateStrategy());

			routes.MapRoute(
				"Default", // Route name
				"{controller}/{action}/{id}", // URL with parameters
				new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
			);

		}

		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();

			RegisterRoutes(RouteTable.Routes);
		}
	}
}