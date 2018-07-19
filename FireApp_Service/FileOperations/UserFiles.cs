using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;

namespace FireApp.Service.FileOperations
{
    //todo: comment
    public static class UserFiles
    {
        public static IEnumerable<User> GetUsersFromCSV(byte[] bytes)
        {
            string csv = System.Text.Encoding.Default.GetString(bytes);
            List<User> results = new List<User>();
            try
            {
                foreach (string s in csv.Split('\n'))
                {
                    results.Add(User.GetUserFromCsv(s));               
                }                
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return results;
        }

        public static byte[] ExportToCSV(IEnumerable<User> users)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(User.GetCsvHeader());
            foreach (User u in users)
            {
                sb.AppendLine(u.ToCsv());
            }

            return Encoding.ASCII.GetBytes(sb.ToString());
        }
    }
}