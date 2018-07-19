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
    public static class FireEventsFiles
    {
        public static byte[] ExportToCSV(IEnumerable<FireEvent> events)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(FireEvent.GetCsvHeader());
            foreach(FireEvent fe in events)
            {
                sb.AppendLine(fe.ToCsv());
            }

            return Encoding.ASCII.GetBytes(sb.ToString());
        }
    }
}