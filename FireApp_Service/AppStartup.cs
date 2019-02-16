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
            app.UseCors(CorsOptions.AllowAll);

            // Web API configuration and services
            config.Formatters.Clear();
            var jsonFormatter = new JsonMediaTypeFormatter();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.Add(jsonFormatter);
            // Web API routes
            config.MapHttpAttributeRoutes();

            app.UseWebApi(config);
            //LocalDatabase.InitializeDatabase();

            // Create admin accounts

            // Caution security risk!
            // Password should be reset when restarting the service.
            User admin = new User("admin", "Password1!", "Max", "Mustermann", "fro.diplomarbeit@gmail.com", UserTypes.admin);
            DatabaseOperations.Users.Upsert(admin, admin);
            admin = new User("admin2", "Password1!", "Max", "Mustermann", "test@example.org", UserTypes.admin);
            DatabaseOperations.Users.Upsert(admin, admin);

            //debug: use only when debugging
            #region debugging
#if DEBUG
            User user = new User("admin2", "admin", "Max", "Mustermann", "admin2@siemens.at", UserTypes.admin);
            DatabaseOperations.Users.Upsert(user, admin);
            user = new User("admin3", "admin", "John", "Doe", "admin3@siemens.at", UserTypes.admin);
            DatabaseOperations.Users.Upsert(user, admin);

            user = new User("fb1", "test", "Mom's", "spaghetti", "test1@siemens.at", UserTypes.fireFighter);
            user.AuthorizedObjectIds.Add(0);
            user.AuthorizedObjectIds.Add(1);
            user.AuthorizedObjectIds.Add(3);
            user.AuthorizedObjectIds.Add(6);
            user.AuthorizedObjectIds.Add(8);
            DatabaseOperations.Users.Upsert(user, admin);

            user = new User("sm1", "test", "David", "Grasser", "test2@siemens.at", UserTypes.servicemember);
            user.AuthorizedObjectIds.Add(2);
            user.AuthorizedObjectIds.Add(9);
            user.AuthorizedObjectIds.Add(4);
            DatabaseOperations.Users.Upsert(user, admin);

            user = new User("fas1", "test", "Jürgen", "Schulz", "test3@siemens.at", UserTypes.fireSafetyEngineer);
            user.AuthorizedObjectIds.Add(7);
            user.AuthorizedObjectIds.Add(5);
            user.AuthorizedObjectIds.Add(2);
            user.AuthorizedObjectIds.Add(10);
            DatabaseOperations.Users.Upsert(user, admin);

            user = new User("fb2", "test", "Maria", "Gusenbauer", "test4@siemens.at", UserTypes.fireFighter);
            user.AuthorizedObjectIds.Add(1);
            user.AuthorizedObjectIds.Add(8);
            user.AuthorizedObjectIds.Add(3);
            DatabaseOperations.Users.Upsert(user, admin);

            user = new User("sm2", "test", "Martha", "Schuster", "test5@siemens.at", UserTypes.servicemember);
            user.AuthorizedObjectIds.Add(7);
            user.AuthorizedObjectIds.Add(2);
            DatabaseOperations.Users.Upsert(user, admin);

            user = new User("fas2", "test", "Stanislaus", "Klein", "test6@siemens.at", UserTypes.fireSafetyEngineer);
            user.AuthorizedObjectIds.Add(6);
            user.AuthorizedObjectIds.Add(4);
            user.AuthorizedObjectIds.Add(1);
            DatabaseOperations.Users.Upsert(user, admin);
            
            user = new User("fb3", "test", "Dorothea", "Wildt", "test7@siemens.at", UserTypes.fireFighter);
            user.AuthorizedObjectIds.Add(2);
            user.AuthorizedObjectIds.Add(3);
            user.AuthorizedObjectIds.Add(1);
            DatabaseOperations.Users.Upsert(user, admin);

            user = new User("sm3", "test", "Friedrich", "Mann", "test8@siemens.at", UserTypes.servicemember);
            user.AuthorizedObjectIds.Add(2);
            user.AuthorizedObjectIds.Add(7);
            user.AuthorizedObjectIds.Add(8);
            DatabaseOperations.Users.Upsert(user, admin); 
               
            user = new User("fas3", "test", "Beathe", "Grün", "test9@siemens.at", UserTypes.fireSafetyEngineer);
            user.AuthorizedObjectIds.Add(9);
            user.AuthorizedObjectIds.Add(5);
            user.AuthorizedObjectIds.Add(1);
            DatabaseOperations.Users.Upsert(user, admin);

            user = new User("unauthorized1", "test", "Oskar", "Schindler", "test90@siemens.at", UserTypes.unauthorized);
            DatabaseOperations.Users.Upsert(user, admin);

            user = new User("unauthorized2", "test", "Cordula", "Grün", "test91@siemens.at", UserTypes.unauthorized);
            DatabaseOperations.Users.Upsert(user, admin);

            user = new User("unauthorized3", "test", "Herbert", "Kurz", "test92@siemens.at", UserTypes.unauthorized);
            DatabaseOperations.Users.Upsert(user, admin);
#endif
            #endregion
        }
    }
}
