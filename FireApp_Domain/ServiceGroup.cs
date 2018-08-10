using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireApp.Domain
{
    /// <summary>
    /// This class is used as a link between Users and FireAlarmSystems.
    /// </summary>
    public class ServiceGroup : IEquatable<ServiceGroup>
    {
        private ServiceGroup() { }

        public ServiceGroup(int id, string groupName)
        {
            this.Id = id;
            this.GroupName = groupName;
        }

        public int Id { get; set; }

        public string GroupName { get; set; }

        /// <summary>
        /// Use the return value as headers of a CSV file.
        /// </summary>
        /// <returns>Returns a string with the names of the CSV values.</returns>
        public static string GetCsvHeader()
        {
            return "id;group name";
        }

        /// <summary>
        /// Turns this ServiceGroup into a CSV line.
        /// </summary>
        /// <returns>Returns a CSV line with the values of the ServiceGroup.</returns>
        public string ToCsv()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Id);
            sb.Append(';');
            sb.Append(GroupName);

            return sb.ToString();
        }

        /// <summary>
        /// This method turns a line of a CSV-File into a new ServiceGroup.
        /// </summary>
        /// <param name="csv">A line of a CSV-File you want to convert.</param>
        /// <returns>Returns a new ServiceGroup or null if an error occures.</returns>
        public static ServiceGroup GetServiceGroupFromCsv(string csv)
        {
            string[] values;

            if (csv != null)
            {
                try
                {
                    values = csv.Split(';');
                    ServiceGroup sg = new ServiceGroup(Convert.ToInt32(values[0]), values[1]);
                    return sg;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return null;
        }

        public bool Equals(ServiceGroup other)
        {
            if (this.Id == other.Id)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}
