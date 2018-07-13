using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Host.SystemWeb;
using Microsoft.Owin.Cors;
using Owin;
using System.Web.Http;
using System.Net.Http.Formatting;
using Newtonsoft.Json.Serialization;
using FireApp.Domain;

[assembly: OwinStartup(typeof(FireApp.Service.AppStartup))]
namespace FireApp.Service {
    public class AppStartup {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            //config.Filters.Add(new AuthorizeAttribute());
            app.UseCors(CorsOptions.AllowAll);

            // Web API configuration and services
            config.Formatters.Clear();
            var jsonFormatter = new JsonMediaTypeFormatter();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.Add(jsonFormatter);
            // Web API routes
            config.MapHttpAttributeRoutes();

            app.UseWebApi(config);
            LocalDatabase.InitializeDatabase();


            //debug: use only in debugging
            User user = new User("admin", "admin", "admin", "admin", "admin@siemens.at", UserTypes.admin, 0);
            user.Token = "1234";
            DatabaseOperations.DbUpserts.UpsertUser(new User("admin", "admin", "admin", "admin", "admin@siemens.at", UserTypes.admin, 0));
        }
    }
}
