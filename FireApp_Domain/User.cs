using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireApp.Domain
{
    /// <summary>
    /// This class represents a user of this application.
    /// </summary>
    public class User
    {
        public User() { }

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

        // This property is a set, because a User can be part of 
        // several institutions of the same type (fire brigade, service group, ...).
        public HashSet<int> AuthorizedObjectIds { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        // Is used to identify the user when sending an request to the API.
        public string Token
        {
            get { return token; }
            set { this.token = value; TokenCreationDate = DateTime.Now; }
        }

        public DateTime TokenCreationDate { get; set; }        
    }

    /// <summary>
    /// Helps to distinguish the different types of users.
    /// </summary>
    public enum UserTypes
    {
        unauthorized = -1,
        admin = 0,
        fireSafetyEngineer = 1,
        fireFighter = 2,
        servicemember = 3
    }
}
