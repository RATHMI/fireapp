using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireApp.Domain
{
    public class User
    {
        private User() { }

        public User(string userName, string password, string firstName, string lastName, string email, UserTypes userType, int authorizedObjectId)
        {
            this.Id = userName;
            this.Password = password;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Email = email;
            this.UserType = userType;
            this.AuthorizedObjectId = authorizedObjectId;
        }

        public string Id { get; set; }
        public string Password { get; set; }

        public UserTypes UserType { get; set; }
        public int AuthorizedObjectId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public string Token { get; set; }
        public DateTime TokenCreationDate { get; set; }
    }

    public enum UserTypes {unauthorized = -1, admin = 0, firealarmsystem = 1, firebrigade = 2, servicemember = 3 }
}
