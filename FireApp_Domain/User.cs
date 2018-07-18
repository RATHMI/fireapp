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
            foreach(int authorizedObjectId in this.AuthorizedObjectIds)
            {
                user.AuthorizedObjectIds.Add(authorizedObjectId);
            }
            user.Token = this.Token;
            user.TokenCreationDate = this.TokenCreationDate;

            return user;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns a string with the names of the CSV values</returns>
        public static string GetCsvHeader()
        {
            return "username;password;user type;authorized object IDs;first name;last name;email;last login";
        }

        /// <summary>
        /// Turns this User into a CSV line
        /// </summary>
        /// <returns>returns a CSV line with the values of the User</returns>
        public string ToCsv()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Id);
            sb.Append(';');
            sb.Append(';');
            sb.Append(UserType);
            sb.Append(';');
            foreach (int authorizedOject in AuthorizedObjectIds)
            {
                sb.Append(authorizedOject);
                sb.Append(',');
            }
            sb.Remove(sb.Length - 1, 1);
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
        /// This method turns a line of a CSV-File into a new User
        /// </summary>
        /// <param name="csv">a line of a CSV-File you want to convert</param>
        /// <returns>returns a new User or null if an error occures</returns>
        public static User GetUserFromCsv(string csv)
        {
            string[] values;

            if (csv != null)
            {
                try
                {
                    values = csv.Split(';');
                    //"username;password;user type;authorized object IDs;first name;last name;email;last login";
                    //User(string userName, string password, string firstName, string lastName, string email, UserTypes userType)
                    User sg = new User(values[0], values[1], values[4], values[5], values[6], UserTypes.unauthorized);
                    string[] date = (values[7].Split(' '))[0].Split('/');
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

    public enum UserTypes {unauthorized = -1, admin = 0, firealarmsystem = 1, firebrigade = 2, servicemember = 3 }
}
