using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;
using System.Text;

namespace FireApp.Service.FileOperations
{
    //todo: comment
    public static class ServiceGroupFiles
    {
        public static IEnumerable<ServiceGroup> GetServiceGroupsFromCSV(object file)
        {
            //todo: implement method
            return null;
        }

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