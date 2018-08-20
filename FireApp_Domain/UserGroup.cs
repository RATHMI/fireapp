using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireApp.Domain
{
    public class UserGroup : IEquatable<UserGroup>
    {
        protected UserGroup() { }

        protected UserGroup(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public virtual UserTypes UserType { get; }

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
        public string ToCsv()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Id);
            sb.Append(';');
            sb.Append(Name);

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
                    UserGroup ug = new UserGroup(Convert.ToInt32(values[0]), values[1]);
                    return ug;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return null;
        }

        public bool Equals(UserGroup other)
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
