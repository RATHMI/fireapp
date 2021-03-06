﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using FireApp.Domain.Extensionmethods;

namespace FireApp.Service.FileOperations
{
    /// <summary>
    /// This class includes methods used for converting Users into different file types and vice versa.
    /// </summary>
    public static class UserFiles
    {
        /// <summary>
        /// Converts a csv file into a list of Users.
        /// </summary>
        /// <param name="bytes">The csv file as a byte array.</param>
        /// <returns>Returns a list of Users.</returns>
        public static IEnumerable<User> GetUsersFromCSV(byte[] bytes)
        {
            string csv = System.Text.Encoding.Default.GetString(bytes);
            List<User> results = new List<User>();
            try
            {
                foreach (string s in csv.Split('\n'))
                {
                    results.Add(UserExtensions.GetFromCsv(s));               
                }                
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return results;
        }

        /// <summary>
        /// Converts a list of Users into a csv file.
        /// </summary>
        /// <param name="users">A list of Users you want to convert.</param>
        /// <returns>Returns a csv file as a byte array.</returns>
        public static byte[] ExportToCSV(IEnumerable<User> users)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(UserExtensions.GetCsvHeader());
            foreach (User u in users)
            {
                sb.AppendLine(u.ToCsv());
            }

            return Encoding.ASCII.GetBytes(sb.ToString());
        }
    }
}