using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FireApp.Domain.User;

namespace FireApp.Domain.Extensionmethods
{
    /// <summary>
    /// This class represents a user of this application.
    /// </summary>
    public static class UserExtensions
    {       
        /// <summary>
        /// Get a short descriptions of the User.
        /// </summary>
        /// <returns>Returns a short description of the User.</returns>
        public static string GetUserDescription(this User user)
        {
            return user.Id + "(" + user.FirstName + ", " + user.LastName + ")";
        }

        /// <summary>
        /// Creates a deep clone to avoid changes in the original when changing the clone.
        /// </summary>
        /// <returns>Returns a deep clone of this User.</returns>
        public static object Clone(this User user)
        {
            User clone = new User();
            clone.Id = user.Id;
            clone.Password = user.Password;
            clone.FirstName = user.FirstName;
            clone.LastName = user.LastName;
            clone.Email = user.Email;
            clone.UserType = user.UserType;
            clone.AuthorizedObjectIds = new HashSet<int>();
            foreach(int authorizedObjectId in user.AuthorizedObjectIds)
            {
                clone.AuthorizedObjectIds.Add(authorizedObjectId);
            }
            clone.Token = user.Token;
            clone.TokenCreationDate = user.TokenCreationDate;

            return clone;
        }

        /// <summary>
        /// Returns a clone of this User with only the most nessesary data.
        /// </summary>
        /// <returns>Returns a clone of this User.</returns>
        public static User SafeClone(this User user)
        {
            User clone = new User();
            clone.Id = user.Id;
            clone.FirstName = user.FirstName;
            clone.LastName = user.LastName;
            clone.Email = user.Email;
            clone.UserType = user.UserType;
            clone.AuthorizedObjectIds = new HashSet<int>();
            foreach(int authobject in user.AuthorizedObjectIds)
            {
                clone.AuthorizedObjectIds.Add(authobject);
            }

            return clone;
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
        public static string ToCsv(this User user)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(user.Id);
            sb.Append(';');
            sb.Append(user.Password);
            sb.Append(';');
            sb.Append(user.UserType);
            sb.Append(';');
            sb.Append(String.Join(",", user.AuthorizedObjectIds));
            sb.Append(';');
            sb.Append(user.FirstName);
            sb.Append(';');
            sb.Append(user.LastName);
            sb.Append(';');
            sb.Append(user.Email);
            sb.Append(';');
            sb.Append(user.TokenCreationDate.ToString("yyyy/MM/dd HH:mm:ss"));

            return sb.ToString();
        }

        /// <summary>
        /// This method turns a line of a CSV-File into a new User.
        /// </summary>
        /// <param name="csv">A line of a CSV-File you want to convert.</param>
        /// <returns>Returns a new User or null if an error occures.</returns>
        public static User GetFromCsv(string csv)
        {
            string[] values;

            if (csv != null)
            {
                try
                {
                    values = csv.Split(';');
                    User u = new User(values[0], values[1], values[4], values[5], values[6], UserTypes.unauthorized);
                    string[] date = (values[7].Split(' '))[0].Split('.');
                    string[] time = (values[7].Split(' '))[1].Split(':');

                    u.TokenCreationDate = new DateTime(
                        Convert.ToInt32(date[0]), 
                        Convert.ToInt32(date[1]), 
                        Convert.ToInt32(date[2]), 
                        Convert.ToInt32(time[0]), 
                        Convert.ToInt32(time[1]), 
                        Convert.ToInt32(time[2]));


                    // Turn the UserType into the enum.
                    switch (values[2])
                    {
                        case "0": u.UserType = UserTypes.admin; break;
                        case "1": u.UserType = UserTypes.fireSafetyEngineer; break;
                        case "2": u.UserType = UserTypes.fireFighter; break;
                        case "3": u.UserType = UserTypes.servicemember; break;
                    }

                    foreach(string s in values[3].Split(','))
                    {
                        u.AuthorizedObjectIds.Add(Convert.ToInt32(s));
                    }

                    return u;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return null;
        }

        public static bool Equals(this User user, User other)
        {
            if (user.Id == other.Id)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static int GetHashCode(this User user)
        {
            return user.Id.GetHashCode();
        }
    }
}
