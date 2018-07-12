using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FireApp.Domain;

namespace FireApp.Service.Controllers
{
    [RoutePrefix("user")]
    public class UserController : ApiController
    {
        /// <summary>
        /// inserts a User into the database or updates it if it already exists
        /// </summary>
        /// <param name="user">The User you want to insert</param>
        /// <returns>returns true if User was inserted</returns>
        [HttpPost, Route("upload")]
        public static bool UpsertServiceMember(User user)
        {
            return DatabaseOperations.Users.UpsertUser(user);
        }

        [HttpPost, Route("authenticate")]
        public string Authenticate(string username, string password)
        {
            return null;
        }

    }
}
