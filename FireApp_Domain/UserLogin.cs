using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireApp.Domain
{

    /// <summary>
    /// This class is used to transfer the login credentials between two systems.
    /// </summary>
    public class UserLogin
    {
        private UserLogin() { }
        public UserLogin(string username, string password)
        {
            this.Username = username;
            this.Password = password;
        }

        public string Username { get; set; }
        public string Password { get; set; }
    }
}
