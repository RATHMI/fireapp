using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireApp.Domain
{
    /// <summary>
    /// This class represents a fire alarm system.
    /// </summary>
    public class FireAlarmSystem : IEquatable<FireAlarmSystem>, ICloneable
    {
        private FireAlarmSystem() {}

        public FireAlarmSystem(int id)
        {
            this.Id = id;
            this.Company = "";
            this.Description = "";
            this.Country = "";
            this.City = "";
            this.PostalCode = "0";
            this.Address = "";
            FireBrigades = new HashSet<int>();
            ServiceGroups = new HashSet<int>();
        }

        public FireAlarmSystem(
            int id,
            string company,
            string description,
            string country,
            string city,
            string postalCode,
            string address)
        {
            this.Id = id;
            this.Company = company;
            this.Description = description;
            this.Country = country;
            this.City = city;
            this.PostalCode = postalCode;
            this.Address = address;
            FireBrigades = new HashSet<int>();
            ServiceGroups = new HashSet<int>();
        }

        // Identifier of the FireAlarmSystem.
        public int Id { get; set; }                         

        // Name of the company that owns the FireAlarmSystem.
        public string Company { get; set; }

        // Short description of the FireAlarmSystem.
        public string Description { get; set; }

        // Country where the FireAlarmSystem is installed.
        public string Country { get; set; }

        // City where the FireAlarmSystem is installed.
        public string City { get; set; }

        // Postal code of the city where the FireAlarmSystem is installed.
        // Not an integer becaus it may contain characters (e.g. A-4020).
        public string PostalCode { get; set; }

        // Address of the building where the FireAlarmSystem is installed.
        public string Address { get; set; }

        // List of the identifiers of the FireBrigades
        // that should have access to certain information.
        public HashSet<int> FireBrigades { get; set; }

        // List of the identifiers of the ServiceGroups 
        // that should have access to certain information.
        public HashSet<int> ServiceGroups { get; set; }

        /// <summary>
        /// Checks if the id of a FireBrigade is in the list 
        /// of FireBrigades of this FireAlarmSystem.
        /// </summary>
        /// <param name="id">The id of the FireBrigade you want to check.</param>
        /// <returns>Returns true if the list of FireBrigades contains the id.</returns>
        public bool CheckFireBrigade(int id)
        {
            bool found = false;
            if (this.FireBrigades != null)
            {
                foreach (int fb in this.FireBrigades)
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
        public bool CheckServiceGroup(int id) {
            bool found = false;
            if (this.ServiceGroups != null)
            {
                foreach (int sg in this.ServiceGroups)
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
        public object Clone()
        {
            FireAlarmSystem fas =
                new FireAlarmSystem
                (
                    this.Id,
                    this.Company,
                    this.Description,
                    this.Country,
                    this.City,
                    this.PostalCode,
                    this.Address
                );
            foreach(int fb in this.FireBrigades)
            {
                fas.FireBrigades.Add(fb);
            }
            foreach (int sg in this.ServiceGroups)
            {
                fas.ServiceGroups.Add(sg);
            }

            return fas;
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
        public string ToCsv()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Id);
            sb.Append(';');
            sb.Append(Company);
            sb.Append(';');
            sb.Append(Description);
            sb.Append(';');
            sb.Append(Country);
            sb.Append(';');
            sb.Append(City);
            sb.Append(';');
            sb.Append(PostalCode);
            sb.Append(';');
            sb.Append(Address);
            sb.Append(';');
            sb.Append(String.Join(",", FireBrigades));
            sb.Append(';');
            sb.Append(String.Join(",", ServiceGroups));

            return sb.ToString();
        } 

        /// <summary>
        /// This method turns a line of a CSV-File into a new FireAlarmSystem.
        /// </summary>
        /// <param name="csv">A line of a CSV-File you want to convert.</param>
        /// <returns>Returns a new FireAlarmSystem or null if an error occures.</returns>
        public static FireAlarmSystem GetFireAlarmSystemFromCsv(string csv)
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

        public bool Equals(FireAlarmSystem other)
        {
            if(this.Id == other.Id)
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
