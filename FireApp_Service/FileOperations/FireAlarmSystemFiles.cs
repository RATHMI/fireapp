using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;
using System.Text;

namespace FireApp.Service.FileOperations
{
    /// <summary>
    /// This class includes methods used for converting FireAlarmSystems into different file types and vice versa.
    /// </summary>
    public static class FireAlarmSystemFiles
    {
        /// <summary>
        /// Converts a csv file into a list of FireAlarmSystems.
        /// </summary>
        /// <param name="bytes">The csv file as a byte array.</param>
        /// <returns>Returns a list of FireAlarmSystems.</returns>
        public static IEnumerable<FireAlarmSystem> GetFireAlarmSystemsFromCSV(byte[] bytes)
        {
            string csv = System.Text.Encoding.Default.GetString(bytes);
            List<FireAlarmSystem> results = new List<FireAlarmSystem>();
            FireAlarmSystem fas;
            try
            {
                foreach (string s in csv.Split('\n'))
                {
                    fas = FireAlarmSystem.GetFromCsv(s);
                    if (fas != null)
                    {
                        results.Add(fas);
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
        /// Converts a list of FireAlarmSystems into a csv file.
        /// </summary>
        /// <param name="fireAlarmSystems">A list of FireAlarmSystems you want to convert.</param>
        /// <returns>Returns a csv file as a byte array.</returns>
        public static byte[] ExportToCSV(IEnumerable<FireAlarmSystem> fireAlarmSystems)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(FireAlarmSystem.GetCsvHeader());
            foreach (FireAlarmSystem fas in fireAlarmSystems)
            {
                sb.AppendLine(fas.ToCsv());
            }

            return Encoding.ASCII.GetBytes(sb.ToString());
        }
    }
}