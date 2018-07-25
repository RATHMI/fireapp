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
using System.Threading;

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
            #region debugging
            #if DEBUG
            User user = new User("admin", "admin", "admin", "admin", "admin@siemens.at", UserTypes.admin);
            DatabaseOperations.BasicOperations.Users.Upsert(user);
            user = new User("admin2", "admin", "admin", "admin", "admin@siemens.at", UserTypes.admin);
            DatabaseOperations.BasicOperations.Users.Upsert(user);
            user = new User("admin3", "admin", "admin", "admin", "admin@siemens.at", UserTypes.admin);
            DatabaseOperations.BasicOperations.Users.Upsert(user);

            user = new User("fb1", "test", "test", "test", "test@siemens.at", UserTypes.firebrigade);
            user.AuthorizedObjectIds.Add(0);
            DatabaseOperations.BasicOperations.Users.Upsert(user);
            user = new User("sm1", "test", "test", "test", "test@siemens.at", UserTypes.servicemember);
            user.AuthorizedObjectIds.Add(0);
            DatabaseOperations.BasicOperations.Users.Upsert(user);
            user = new User("fas1", "test", "test", "test", "test@siemens.at", UserTypes.firealarmsystem);
            user.AuthorizedObjectIds.Add(0);
            DatabaseOperations.BasicOperations.Users.Upsert(user);

            user = new User("fb2", "test", "test", "test", "test@siemens.at", UserTypes.firebrigade);
            user.AuthorizedObjectIds.Add(1);
            DatabaseOperations.BasicOperations.Users.Upsert(user);
            user = new User("sm2", "test", "test", "test", "test@siemens.at", UserTypes.servicemember);
            user.AuthorizedObjectIds.Add(1);
            DatabaseOperations.BasicOperations.Users.Upsert(user);
            user = new User("fas2", "test", "test", "test", "test@siemens.at", UserTypes.firealarmsystem);
            user.AuthorizedObjectIds.Add(1);
            DatabaseOperations.BasicOperations.Users.Upsert(user);
            
            user = new User("fb3", "test", "test", "test", "test@siemens.at", UserTypes.firebrigade);
            user.AuthorizedObjectIds.Add(2);
            DatabaseOperations.BasicOperations.Users.Upsert(user);
            user = new User("sm3", "test", "test", "test", "test@siemens.at", UserTypes.servicemember);
            user.AuthorizedObjectIds.Add(2);
            DatabaseOperations.BasicOperations.Users.Upsert(user);    
            user = new User("fas3", "test", "test", "test", "test@siemens.at", UserTypes.firealarmsystem);
            user.AuthorizedObjectIds.Add(2);
            DatabaseOperations.BasicOperations.Users.Upsert(user);
            #endif
            #endregion
        }
    }
}
