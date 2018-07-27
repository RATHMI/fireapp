using FireApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FireApp.Service.Controllers
{
    [RoutePrefix("email")]
    public class EmailController : ApiController
    {
        /// <summary>
        /// Sends a email to the admin with a message of the user.
        /// </summary>
        /// <param name="message">The text the user entered.</param>
        /// <returns>
        /// Returns 1 if the email was sent.
        /// 0 : User is not logged in.
        /// -1 : an error occured.
        /// </returns>
        [HttpPost, Route("help")]
        public Int32 Help([FromBody] string message)
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    User admin = DatabaseOperations.Users.GetFirstAdmin();
                    if (admin != null) {
                        Email.Email.HelpEmail(user, admin.Email, message);
                        return 1;
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }
    }
}
