#pragma warning disable 1591
namespace ToDoApp.Api
{
    using System.Web;
    using System.Web.Http;
    using System.Web.Mvc;

    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
        }
    }
}