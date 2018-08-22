using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FireApp.Domain;

namespace FireApp.Domain.Extensionmethods
{
    public static class UserGroupExtensions
    {
        /// <summary>
        /// Use the return value as headers of a CSV file.
        /// </summary>
        /// <returns>Returns a string with the names of the CSV values.</returns>
        public static string GetCsvHeader()
        {
            return "id;name";
        }

        /// <summary>
        /// Turns this ServiceGroup into a CSV line.
        /// </summary>
        /// <returns>Returns a CSV line with the values of the ServiceGroup.</returns>
        public static string ToCsv(this UserGroup ug)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ug.Id);
            sb.Append(';');
            sb.Append(ug.Name);

            return sb.ToString();
        }

        /// <summary>
        /// This method turns a line of a CSV-File into a new ServiceGroup.
        /// </summary>
        /// <param name="csv">A line of a CSV-File you want to convert.</param>
        /// <returns>Returns a new ServiceGroup or null if an error occures.</returns>
        public static UserGroup GetFromCsv(string csv)
        {
            string[] values;

            if (csv != null)
            {
                try
                {
                    values = csv.Split(';');
                    return new UserGroup(Convert.ToInt32(values[0]), values[1]);
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return null;
        }

        public static bool Equals(this UserGroup ug, UserGroup other)
        {
            if (ug.Id == other.Id)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static int GetHashCode(this UserGroup ug)
        {
            return ug.Id.GetHashCode();
        }
    }
}
