using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;
using System.Net.Http.Headers;

namespace FireApp.Service.Authentication
{
    public static class Token
    {
        public static string RefreshToken(UserLogin login)
        {
            User user = DatabaseOperations.Users.GetUserById(login.Username).First<User>();
            if (user != null)
            {
                if (user.Password == login.Password)
                {
                    user.Token = Authentication.Token.GenerateToken(user.Id.GetHashCode());
                    user.TokenCreationDate = DateTime.Now;
                    DatabaseOperations.Users.UpsertUser(user);
                    return user.Token;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public static string GenerateToken(int hash)
        {
            Random random = new Random();
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return hash.ToString() + new string(Enumerable.Repeat(chars, 50).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static User VerifyToken(string token)
        {
            List<User> users = DatabaseOperations.Users.GetAllUsers().ToList<User>();
            foreach(User u in users)
            {
                if(u.Token == token && DateTime.Now < u.TokenCreationDate.AddDays(365))
                {
                    return u;
                }
            }

            return null;
        }

        public static string GetTokenFromHeader(HttpRequestHeaders header)
        {
            IEnumerable<string> key = new List<string>();
            if (header.TryGetValues("token", out key) != false)
            {                
                return key.First<string>();
            }
            else
            {
                return null;
            }

        }
    }
}