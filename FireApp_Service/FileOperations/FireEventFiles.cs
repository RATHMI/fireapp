using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;
using System.IO;
using System.Text;
using FireApp.Domain.Extensionmethods;

namespace FireApp.Service.FileOperations
{
    /// <summary>
    /// This class includes methods used for converting FireEvents into different file types.
    /// </summary>
    public static class FireEventsFiles 
    {
        /// <summary>
        /// Converts a list of FireEvents into a csv file.
        /// </summary>
        /// <param name="events">A list of FireEvents you want to convert.</param>
        /// <returns>Returns a csv file as a byte array.</returns>
        public static byte[] ExportToCSV(IEnumerable<FireEvent> events)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(FireEventExtensions.GetCsvHeader());
            foreach(FireEvent fe in events)
            {
                sb.AppendLine(fe.ToCsv());
            }

            return Encoding.ASCII.GetBytes(sb.ToString());
        }

        //No methods to import FireEvents because they should only be inserted by a FireAlarmSystem via the controller.
    }
}