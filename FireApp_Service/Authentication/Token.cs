using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;

namespace FireApp.Service.Authentication
{
    public static class Token
    {
        public static string GetToken(UserLogin login)
        {
            return null; //todo: implement method
        }

        private static string GenerateToken()
        {
            Random random = new Random();
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 50).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}