using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IdentityModel.Tokens.Jwt;
using FireApp.Service.Authentication;

namespace FireApp.Service.Controllers
{
    public class TokenController : ApiController
    {
        [HttpPost, Route("authenticate"), AllowAnonymous]
        public string Get(string username, string password)
        {
            if (CheckUser(username, password))
            {
                return Authentication.JwtManager.GenerateToken(username, 365);
            }

            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        [JwtAuthentication]
        public string Test()
        {
            return "value";
        }

        public bool CheckUser(string username, string password)
        {
            // todo: check in the database
            return true;
        }
    }
}
