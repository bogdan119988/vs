using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using SportsStore.WebUI.Infrastructure;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Binders;

namespace SportsStore.WebUI
{
	public class MvcApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
	
			ControllerBuilder.Current.SetControllerFactory(new NinjectControllerFactory());
			ModelBinders.Binders.Add(typeof(Cart), new CartModelBinder());
		}
	}
}
