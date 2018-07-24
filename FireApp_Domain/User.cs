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
        private UserTypes userType;

        public UserTypes UserType {
            get { return userType; }
            set {
                userType = value;
                switch (value)
                {
                    case UserTypes.admin: TokenValidDays = 1; break;
                    case UserTypes.firealarmsystem: TokenValidDays = 365; break;
                    case UserTypes.firebrigade: TokenValidDays = 365; break;
                    case UserTypes.servicemember: TokenValidDays = 365; break;
                    default: TokenValidDays = 0; break;
                }
            }
        }

        // This property is a set, because a User can be part of 
        // several institutions of the same type (fire brigade, service group, ...).
        public HashSet<int> AuthorizedObjectIds { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public string Token {
            get { return token; }
            set { this.token = value; TokenCreationDate = DateTime.Now; }
        }
        public DateTime TokenCreationDate { get; set; }
        public int TokenValidDays { get; set; }

        public static string EncryptPassword(string password)
        {
            int sum = 0;
            for(int i = 0; i < password.Length; i++)
            {
                sum += (int)password[i];
            }

            return sum.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns a short description of the User.</returns>
        public string GetUserDescription()
        {
            return this.Id + "(" + this.FirstName + ", " + this.LastName + ")";
        }

        /// <summary>
        /// Creates a deep clone to avoid changes in the original when changing the clone.
        /// </summary>
        /// <returns>Returns a deep clone of this User.</returns>
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
            foreach(int authorizedObjectId in this.AuthorizedObjectIds)
            {
                user.AuthorizedObjectIds.Add(authorizedObjectId);
            }
            user.Token = this.Token;
            user.TokenCreationDate = this.TokenCreationDate;

            return user;
        }

        /// <summary>
        /// Use the return value as headers of a CSV file.
        /// </summary>
        /// <returns>Returns a string with the names of the CSV values.</returns>
        public static string GetCsvHeader()
        {
            return "username;password;user type;authorized object IDs;first name;last name;email;last login";
        }

        /// <summary>
        /// Turns this User into a CSV line.
        /// </summary>
        /// <returns>Returns a CSV line with the values of this User.</returns>
        public string ToCsv()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Id);
            sb.Append(';');
            sb.Append(';');
            sb.Append(UserType);
            sb.Append(';');
            sb.Append(String.Join(",", AuthorizedObjectIds));
            sb.Append(';');
            sb.Append(FirstName);
            sb.Append(';');
            sb.Append(LastName);
            sb.Append(';');
            sb.Append(Email);
            sb.Append(';');
            sb.Append(TokenCreationDate.ToString("yyyy/MM/dd HH:mm:ss"));

            return sb.ToString();
        }

        /// <summary>
        /// This method turns a line of a CSV-File into a new User.
        /// </summary>
        /// <param name="csv">A line of a CSV-File you want to convert.</param>
        /// <returns>Returns a new User or null if an error occures.</returns>
        public static User GetUserFromCsv(string csv) // todo: comment
        {
            string[] values;

            if (csv != null)
            {
                try
                {
                    values = csv.Split(';');
                    User sg = new User(values[0], values[1], values[4], values[5], values[6], UserTypes.unauthorized);
                    string[] date = (values[7].Split(' '))[0].Split('.');
                    string[] time = (values[7].Split(' '))[1].Split(':');
                    sg.TokenCreationDate = new DateTime(
                        Convert.ToInt32(date[0]), 
                        Convert.ToInt32(date[1]), 
                        Convert.ToInt32(date[2]), 
                        Convert.ToInt32(time[0]), 
                        Convert.ToInt32(time[1]), 
                        Convert.ToInt32(time[2]));

                    switch (values[2])
                    {
                        case "0": sg.UserType = UserTypes.admin; break;
                        case "1": sg.UserType = UserTypes.firealarmsystem; break;
                        case "2": sg.UserType = UserTypes.firebrigade; break;
                        case "3": sg.UserType = UserTypes.servicemember; break;
                    }

                    foreach(string s in values[3].Split(','))
                    {
                        sg.AuthorizedObjectIds.Add(Convert.ToInt32(s));
                    }

                    return sg;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return null;
        }
    }

    /// <summary>
    /// Helps to distinguish the different types of users.
    /// </summary>
    public enum UserTypes {unauthorized = -1, admin = 0, firealarmsystem = 1, firebrigade = 2, servicemember = 3 }
}
