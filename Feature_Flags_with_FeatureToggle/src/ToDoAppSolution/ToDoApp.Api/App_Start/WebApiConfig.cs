#pragma warning disable 1591
namespace ToDoApp.Api
{
    using System.Web.Http;

    internal static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new
                          {
                              id = RouteParameter.Optional
                          });
        }
    }
}