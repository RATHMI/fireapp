using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FireApp.Domain;

namespace FireApp.Domain.Extensionmethods
{
    /// <summary>
    /// This class represents a fire alarm system.
    /// </summary>
    public static class FireAlarmSystemExtensions
    {       
        /// <summary>
        /// Checks if the id of a FireBrigade is in the list 
        /// of FireBrigades of this FireAlarmSystem.
        /// </summary>
        /// <param name="id">The id of the FireBrigade you want to check.</param>
        /// <returns>Returns true if the list of FireBrigades contains the id.</returns>
        public static bool CheckFireBrigade(this FireAlarmSystem fas, int id)
        {
            bool found = false;
            if (fas.FireBrigades != null)
            {
                foreach (int fb in fas.FireBrigades)
                {
                    if (fb == id)
                    {
                        found = true;
                        break;
                    }
                }
            }
            return found;
        }

        /// <summary>
        /// Checks if the id of a ServiceGroup is in the list 
        /// of ServiceGroups of this FireAlarmSystem.
        /// </summary>
        /// <param name="id">The id of the ServiceGroup you want to check.</param>
        /// <returns>Returns true if the list of ServiceGroups contains the id.</returns>
        public static bool CheckServiceGroup(this FireAlarmSystem fas, int id) {
            bool found = false;
            if (fas.ServiceGroups != null)
            {
                foreach (int sg in fas.ServiceGroups)
                {
                    if (sg == id)
                    {
                        found = true;
                        break;
                    }
                }
            }
            return found;
        }

        /// <summary>
        /// Creates a deep clone to avoid changes in the original when changing the clone.
        /// </summary>
        /// <returns>Returns a deep clone of this object.</returns>
        public static object Clone(this FireAlarmSystem fas)
        {
            FireAlarmSystem clone =
                new FireAlarmSystem
                (
                    fas.Id,
                    fas.Company,
                    fas.Description,
                    fas.Country,
                    fas.City,
                    fas.PostalCode,
                    fas.Address
                );
            foreach(int fb in fas.FireBrigades)
            {
                clone.FireBrigades.Add(fb);
            }
            foreach (int sg in fas.ServiceGroups)
            {
                clone.ServiceGroups.Add(sg);
            }

            return clone;
        }

        /// <summary>
        /// Use the return value as headers of a CSV file.
        /// </summary>
        /// <returns>Returns a string with the names of the CSV values.</returns>
        public static string GetCsvHeader()
        {
            return "id;company;description;country;city;postal code;address;fire brigades; service groups";
        }

        /// <summary>
        /// Turns this FireAlarmSystem into a CSV line.
        /// </summary>
        /// <returns>Returns a CSV line with the values of the FireAlarmSystem.</returns>
        public static string ToCsv(this FireAlarmSystem fas)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(fas.Id);
            sb.Append(';');
            sb.Append(fas.Company);
            sb.Append(';');
            sb.Append(fas.Description);
            sb.Append(';');
            sb.Append(fas.Country);
            sb.Append(';');
            sb.Append(fas.City);
            sb.Append(';');
            sb.Append(fas.PostalCode);
            sb.Append(';');
            sb.Append(fas.Address);
            sb.Append(';');
            sb.Append(String.Join(",", fas.FireBrigades));
            sb.Append(';');
            sb.Append(String.Join(",", fas.ServiceGroups));

            return sb.ToString();
        } 

        /// <summary>
        /// This method turns a line of a CSV-File into a new FireAlarmSystem.
        /// </summary>
        /// <param name="csv">A line of a CSV-File you want to convert.</param>
        /// <returns>Returns a new FireAlarmSystem or null if an error occures.</returns>
        public static FireAlarmSystem GetFromCsv(string csv)
        {
            string[] values;

            if (csv != null)
            {
                try
                {
                    values = csv.Split(';');
                    int id = Convert.ToInt32(values[0]);
                    string company = values[1];
                    string description = values[2];
                    string country = values[3];
                    string city = values[4];
                    string postalCode = values[5];
                    string address = values[6];

                    HashSet<int> fireBrigades = new HashSet<int>();
                    foreach(string s in values[7].Split(','))
                    {
                        fireBrigades.Add(Convert.ToInt32(s));
                    }

                    HashSet<int> serviceGroups = new HashSet<int>();
                    foreach (string s in values[8].Split(','))
                    {
                        serviceGroups.Add(Convert.ToInt32(s));
                    }

                    FireAlarmSystem fas = new FireAlarmSystem(
                            id,
                            company,
                            description,
                            country,
                            city,
                            postalCode,
                            address);

                    fas.FireBrigades = fireBrigades;
                    fas.ServiceGroups = serviceGroups;

                    return fas;
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
            else
            {
                return null;
            }            
        }

        public static bool Equals(this FireAlarmSystem fas, FireAlarmSystem other)
        {
            if(fas.Id == other.Id)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static int GetHashCode(this FireAlarmSystem fas)
        {
            return fas.Id.GetHashCode();
        }
    }
}
