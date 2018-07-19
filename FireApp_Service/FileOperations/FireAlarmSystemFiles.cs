using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;
using System.Text;

namespace FireApp.Service.FileOperations
{
    //todo: comment
    public static class FireAlarmSystemFiles
    {
        public static IEnumerable<FireAlarmSystem> GetFireAlarmSystemsFromCSV(object file)
        {
            //todo: implement method
            return null;
        }

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