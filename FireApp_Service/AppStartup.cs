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
using System.Web;
using System.Threading;
using System.Security.Cryptography.X509Certificates;
using IdentityServer3.Core.Configuration;
using FireApp.Service.Authentication;
using static IdentityServer3.Core.Models.Scope;
using IdentityServer3.Core.Models;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;

[assembly: OwinStartup(typeof(FireApp.Service.AppStartup))]
namespace FireApp.Service {
    public class AppStartup {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();

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
        }
    }
}
