using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireApp.Domain
{
    public class User : ICloneable
    {
        private User() { }

        public User(string userName, string password, string firstName, string lastName, string email, UserTypes userType)
        {
            this.Id = userName;
            this.Password = password;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Email = email;
            this.UserType = userType;
            this.AuthorizedObjectIds = new HashSet<int>();
        }

        private string token;
        public string Id { get; set; }
        public string Password { get; set; }

        public UserTypes UserType { get; set; }
        public HashSet<int> AuthorizedObjectIds { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public string Token {
            get { return token; }
            set { this.token = value; TokenCreationDate = DateTime.Now; }
        }
        public DateTime TokenCreationDate { get; set; }

        public object Clone()
        {
            User user = new User();
            user.Id = this.Id;
            user.Password = this.Password;
            user.FirstName = this.FirstName;
            user.LastName = this.LastName;
            user.Email = this.Email;
            user.UserType = this.UserType;
            user.AuthorizedObjectIds = new HashSet<int>();
            user.AuthorizedObjectIds.ToList<int>().AddRange(this.AuthorizedObjectIds);
            user.Token = this.Token;
            user.TokenCreationDate = this.TokenCreationDate;

            return user;
        }
    }

    public enum UserTypes {unauthorized = -1, admin = 0, firealarmsystem = 1, firebrigade = 2, servicemember = 3 }
}
