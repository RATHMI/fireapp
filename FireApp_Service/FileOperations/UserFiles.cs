using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;
using System.IO;
using System.Text;

namespace FireApp.Service.FileOperations
{
    //todo: comment
    public static class UserFiles
    {
        public static IEnumerable<User> GetUsersFromCSV(object file)
        {
            //todo: implement method
            return null;
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