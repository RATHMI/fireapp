using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;
using System.Text;

namespace FireApp.Service.FileOperations
{
    //todo: comment
    public static class FireBrigadeFiles
    {
        public static IEnumerable<FireBrigade> GetFireBrigadesFromCSV(object file)
        {
            //todo: implement method
            return null;
        }

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