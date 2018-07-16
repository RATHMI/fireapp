using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;
using System.Net.Http.Headers;

namespace FireApp.Service.Authentication
{
    /// <summary>
    /// This class is for authentication via tokens
    /// </summary>
    public static class Token
    {
        /// <summary>
        /// This method generates a new token and 
        /// saves it in the User object with the matching UserLogin. 
        /// </summary>
        /// <param name="login">the login data of a user</param>
        /// <returns>returns the token if the login worked or null if not</returns>
        public static string RefreshToken(UserLogin login)
        {
            try
            {
                User user = DatabaseOperations.Users.GetUserById(login.Username).First<User>();
                if (user != null)
                {
                    if (user.Password == login.Password)
                    {
                        user.Token = Authentication.Token.GenerateToken(user.Id.GetHashCode());
                        DatabaseOperations.Users.UpsertUser(user);
                        return user.Token;
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }    
        }

        /// <summary>
        /// This method generates a random token
        /// </summary>
        /// <param name="hash">an Integer that is used for the first part of the token</param>
        /// <returns>returns a new random token</returns>
        public static string GenerateToken(int hash)
        {
            Random random = new Random();
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return hash.ToString() + new string(Enumerable.Repeat(chars, 50).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// This method checks if there is a User with this token and if the token is still valid
        /// </summary>
        /// <param name="token">the token you want to verify</param>
        /// <returns>returns the User that is assoziated with the token or null</returns>
        public static IEnumerable<User> VerifyToken(string token)
        {
            if (token != null)
            {
                List<User> users = DatabaseOperations.Users.GetAllUsers().ToList<User>();
                foreach (User u in users)
                {
                    if (u != null && u.Token != null)
                    {
                        if (u.Token == token && DateTime.Now < u.TokenCreationDate.AddDays(365))
                        {
                            List<User> user = new List<User>();
                            user.Add(u);
                            return (IEnumerable<User>)(user);
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// This method extracts a token from the HttpRequestHeaders
        /// </summary>
        /// <param name="headers">The headers of a HttpRequest</param>
        /// <returns>returns the token or null</returns>
        public static string GetTokenFromHeader(HttpRequestHeaders headers)
        {
            IEnumerable<string> key = new List<string>();
            if (headers.TryGetValues("token", out key) != false)
            {
                headers.TryGetValues("token", out key);
                return key.First<string>();
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// This method extracts a token from the HttpRequestHeaders and fetches the
        /// User. If the token is valid and the UserType of the User is contained in userTypes
        /// it returns the user.
        /// </summary>
        /// <param name="headers">The headers of a HttpRequest</param>
        /// <param name="userTypes"></param>
        /// <returns>returns a User if the token an userType are valid</returns>
        public static IEnumerable<User> CheckAccess(HttpRequestHeaders headers, UserTypes[] userTypes)
        {
            User user = Authentication.Token.VerifyToken(Authentication.Token.GetTokenFromHeader(headers)).First<User>();
            if (user != null)
            {
                if (userTypes.Contains(user.UserType))
                {
                    return (IEnumerable<User>)user;
                }
                else
                {
                    return ((IEnumerable<User>)new User("", "", "", "", "", UserTypes.unauthorized));
                }
            }
            return null;
        }
    }
}