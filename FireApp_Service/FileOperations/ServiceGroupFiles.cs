using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;
using System.Text;

namespace FireApp.Service.FileOperations
{
    /// <summary>
    /// This class includes methods used for converting ServiceGroups into different file types an vice versa
    /// </summary>
    public static class ServiceGroupFiles
    {
        /// <summary>
        /// converts a csv file into a list of ServiceGroups
        /// </summary>
        /// <param name="bytes">the csv file as a byte array</param>
        /// <returns>returns a list of ServiceGroups</returns>
        public static IEnumerable<ServiceGroup> GetServiceGroupsFromCSV(byte[] bytes)
        {
            string csv = System.Text.Encoding.Default.GetString(bytes);
            List<ServiceGroup> results = new List<ServiceGroup>();
            try
            {
                foreach (string s in csv.Split('\n'))
                {
                    results.Add(ServiceGroup.GetServiceGroupFromCsv(s));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return results;
        }

        /// <summary>
        /// Converts a list of ServiceGroups into a csv file
        /// </summary>
        /// <param name="serviceGroups">a list of ServiceGroups you want to convert</param>
        /// <returns>returns a csv file as a byte array</returns>
        public static byte[] ExportToCSV(IEnumerable<ServiceGroup> serviceGroups)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(ServiceGroup.GetCsvHeader());
            foreach (ServiceGroup sg in serviceGroups)
            {
                sb.AppendLine(sg.ToCsv());
            }

            return Encoding.ASCII.GetBytes(sb.ToString());
        }
    }
}