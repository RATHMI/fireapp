using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;
using System.Net.Http.Headers;
using MlkPwgen;
using System.Runtime.CompilerServices;

namespace FireApp.Service.Authentication
{
    /// <summary>
    /// This class is for authentication via tokens.
    /// </summary>
    public static class Token
    {
        /// <summary>
        /// This method generates a new token and 
        /// saves it in the User with the matching UserLogin. 
        /// </summary>
        /// <param name="login">The login data of a user.</param>
        /// <returns>Returns the token if the login worked or null if not.</returns>
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public static string RefreshToken(UserLogin login)
        {
            try
            {
                // Get User with matching username.
                User user = DatabaseOperations.Users.GetById(login.Username);

                // If User was found.
                if (user != null)
                {
                    // Decrypt password for comparison and to avoid encrypting, encrypted password
                    user.Password = Encryption.Encrypt.DecryptString(user.Password);

                    // If password is right.
                    // Compiler tries to optimize this if statement then which causes a bug.
                    if (user.Password == login.Password)
                    {
                        // Generate a new token.
                        user.Token = Authentication.Token.GenerateToken();

                        // Save the changes in the database
                        DatabaseOperations.Users.Upsert(user, user);
                        return user.Token;
                    }
                }

                // If username or password were wrong.
                return null;
            }
            catch (Exception)
            {
                return null;
            }    
        }

        /// <summary>
        /// This method generates a new random token.
        /// </summary>
        /// <param name="hash">An Integer that is used for the first part of the token.</param>
        /// <returns>Returns a new random token.</returns>
        public static string GenerateToken()
        {
            IEnumerable<User> users = DatabaseOperations.Users.GetAll();
            string token = "";

            while (token == "")
            {
                token = PasswordGenerator.Generate(length: 50, allowed: Sets.Alphanumerics);
                foreach (User user in users)
                {
                    if (user.Token == token)
                    {
                        token = "";
                    }
                }
            }

            return token;
        }

        /// <summary>
        /// This method checks if there is a User with this token and if the token is still valid.
        /// </summary>
        /// <param name="token">The token you want to verify.</param>
        /// <returns>Returns the User that is assoziated with the token or null.</returns>
        public static User VerifyToken(string token)
        {
            if (token != null)
            {
                IEnumerable<User> users = DatabaseOperations.Users.GetAll();
                foreach (User u in users)
                {
                    if (u != null && u.Token != null)
                    {
                        // if the token of the User and the given token are matching
                        if (u.Token == token)
                        {
                            // if the token of the User is still valid
                            if (DateTime.Now < u.TokenCreationDate.AddHours(AppData.TokenValidHours(u.UserType)))
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
        /// This method extracts a token from the HttpRequestHeaders.
        /// </summary>
        /// <param name="headers">The headers of a HttpRequest.</param>
        /// <returns>Returns the token or null.</returns>
        public static void GetTokenFromHeader(HttpRequestHeaders headers, out string token)
        {
            IEnumerable<string> key;

            // If the headers contain the key "token".
            if (headers.TryGetValues("token", out key) != false)
            {
                headers.TryGetValues("token", out key);
                token = key.First<string>().Trim('"');
            }
            else
            {
                token = null;
            }
            return;
        }

        /// <summary>
        /// This method extracts a token from the HttpRequestHeaders and fetches the
        /// User. If the token is valid it returns the User.
        /// </summary>
        /// <param name="headers">The headers of a HttpRequest.</param>
        /// <returns>Returns a User if the token is valid.</returns>
        public static void CheckAccess(HttpRequestHeaders headers, out User user)
        {
            string token;
            Authentication.Token.GetTokenFromHeader(headers, out token);
            user = Authentication.Token.VerifyToken(token);
            return;
        }
    }
}