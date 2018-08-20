using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;
using System.Text;

namespace FireApp.Service.FileOperations
{
    /// <summary>
    /// This class includes methods used for converting UserGroups into different file types and vice versa.
    /// </summary>
    public static class UserGroupFiles
    {
        /// <summary>
        /// Converts a csv file into a list of UserGroups.
        /// </summary>
        /// <param name="bytes">The csv file as a byte array.</param>
        /// <returns>Returns a list of UserGroups.</returns>
        public static IEnumerable<UserGroup> GetFromCSV(byte[] bytes)
        {
            string csv = System.Text.Encoding.Default.GetString(bytes);
            List<UserGroup> results = new List<UserGroup>();
            try
            {
                foreach (string s in csv.Split('\n'))
                {
                    try
                    {
                        results.Add(UserGroup.GetFromCsv(s));
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return results;
        }

        /// <summary>
        /// Converts a list of UserGroups into a csv file.
        /// </summary>
        /// <param name="userGroups">A list of UserGroups you want to convert.</param>
        /// <returns>Returns a csv file as a byte array.</returns>
        public static byte[] ExportToCSV(IEnumerable<UserGroup> userGroups)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(UserGroup.GetCsvHeader());
            foreach (UserGroup ug in userGroups)
            {
                sb.AppendLine(ug.ToCsv());
            }

            return Encoding.ASCII.GetBytes(sb.ToString());
        }
    }
}