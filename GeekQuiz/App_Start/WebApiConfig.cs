using Autofac;
using Autofac.Integration.WebApi;
using GeekQuiz.Models;
using GeekQuiz.WorkerServices;
using Newtonsoft.Json.Serialization;
using System.Data.Entity;
using System.Reflection;
using System.Web.Http;

namespace GeekQuiz
{
  public static class WebApiConfig
  {
    public static void Register(HttpConfiguration config)
    {
      // Web API configuration and services

      // Use camel case for JSON data.
      config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

      // Web API routes
      config.MapHttpAttributeRoutes();

      config.Routes.MapHttpRoute(
          name: "DefaultApi",
          routeTemplate: "api/{controller}/{id}",
          defaults: new { id = RouteParameter.Optional }
      );

      var builder = new ContainerBuilder();

      // Register your Web API controllers.
      builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

      builder.RegisterType<TriviaContext>().InstancePerRequest();
      builder.RegisterType<TriviaService>().As<ITriviaService>();

      // Set the dependency resolver to be Autofac.
      var container = builder.Build();
      config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
    }
  }
}
