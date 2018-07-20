using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;
using System.Net.Http.Headers;
using MlkPwgen;

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
                User user = DatabaseOperations.Users.GetById(login.Username);
                if (user != null)
                {
                    if (user.Password == login.Password)
                    {
                        user.Token = Authentication.Token.GenerateToken(user.Id.GetHashCode());
                        DatabaseOperations.Users.Upsert(user);
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
            System.Random random = new System.Random();
            return hash.ToString().Substring(random.Next(1, 5)) + PasswordGenerator.Generate(length: 50, allowed: Sets.Alphanumerics);
        }

        /// <summary>
        /// This method checks if there is a User with this token and if the token is still valid
        /// </summary>
        /// <param name="token">the token you want to verify</param>
        /// <returns>returns the User that is assoziated with the token or null</returns>
        public static User VerifyToken(string token)
        {
            if (token != null)
            {
                IEnumerable<User> users = DatabaseOperations.Users.GetAll();
                foreach (User u in users)
                {
                    if (u != null && u.Token != null)
                    {
                        // if the toke of the user and the given token are matching
                        if (u.Token == token)
                        {
                            // if the token of the user is still valid
                            if (DateTime.Now < u.TokenCreationDate.AddDays(365))
                            {
                                return u;
                            }
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
        public static void GetTokenFromHeader(HttpRequestHeaders headers, out string token)
        {
            IEnumerable<string> key;
            if (headers.TryGetValues("token", out key) != false)
            {
                headers.TryGetValues("token", out key);
                token = key.First<string>().Trim(new char[] { '"' });
            }
            else
            {
                token = null;
            }
            return;
        }

        /// <summary>
        /// This method extracts a token from the HttpRequestHeaders and fetches the
        /// User. If the token is valid it returns the user.
        /// </summary>
        /// <param name="headers">The headers of a HttpRequest</param>
        /// <returns>returns a User if the token is valid</returns>
        public static void CheckAccess(HttpRequestHeaders headers, out User user)
        {
            string token;
            Authentication.Token.GetTokenFromHeader(headers, out token);
            user = Authentication.Token.VerifyToken(token);
            return;
        }
    }
}