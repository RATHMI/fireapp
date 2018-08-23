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
    public class FireAlarmSystem
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

        // List of the identifiers of FireBrigades
        // that should have access to certain information.
        public HashSet<int> FireBrigades { get; set; }

        // List of the identifiers of ServiceGroups 
        // that should have access to certain information.
        public HashSet<int> ServiceGroups { get; set; }     
    }
}
