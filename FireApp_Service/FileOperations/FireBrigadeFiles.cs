using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;
using System.Text;

namespace FireApp.Service.FileOperations
{
    /// <summary>
    /// This class includes methods used for converting FireBrigades into different file types an vice versa
    /// </summary>
    public static class FireBrigadeFiles
    {
        /// <summary>
        /// converts a csv file into a list of FireBrigades
        /// </summary>
        /// <param name="bytes">the csv file as a byte array</param>
        /// <returns>returns a list of FireBrigades</returns>
        public static IEnumerable<FireBrigade> GetFireBrigadesFromCSV(string csv)
        {
            List<FireBrigade> results = new List<FireBrigade>();
            try
            {
                foreach (string s in csv.Split('\n'))
                {
                    results.Add(FireBrigade.GetFireBrigadeFromCsv(s));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return results;
        }

        /// <summary>
        /// Converts a list of FireBrigades into a csv file
        /// </summary>
        /// <param name="fireBrigades">a list of FireBrigades you want to convert</param>
        /// <returns>returns a csv file as a byte array</returns>
        public static byte[] ExportToCSV(IEnumerable<FireBrigade> fireBrigades)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(FireBrigade.GetCsvHeader());
            foreach (FireBrigade fb in fireBrigades)
            {
                sb.AppendLine(fb.ToCsv());
            }

            return Encoding.ASCII.GetBytes(sb.ToString());
        }
    }
}