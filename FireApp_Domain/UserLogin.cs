using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireApp.Domain
{
    public class UserLogin
    {
        const int tokenlength = 50;

        private string token;

        public UserLogin() { }

        public string UserId { get; set; }

        public string Password { get; set; }

        public bool CheckToken(string token)
        {
            bool matching = true;

            if(this.token == null || token == null || token.Length != tokenlength)
            {
                matching = false;
            }
            else
            {
                for(int i = 0; i < tokenlength; i++)
                {
                    if(this.token[i] != token[i])
                    {
                        matching = false;
                        break;
                    }
                }
            }

            return matching;           
        }

        public string GetToken()
        {     
            Random random = new Random();
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            token = new string(Enumerable.Repeat(chars, tokenlength) .Select(s => s[random.Next(s.Length)]).ToArray());

            return token;
        }
    }
}
