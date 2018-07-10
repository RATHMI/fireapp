using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireApp.Domain
{
    /// <summary>
    /// represents a fire alarm system
    /// </summary>
    public class FireAlarmSystem
    {
        public FireAlarmSystem() {}

        public FireAlarmSystem(
            int id,
            string company,
            string description,
            string country,
            string city,
            int postalCode,
            string address)
        {
            this.Id = id;
            this.Company = company;
            this.Description = description;
            this.Country = country;
            this.City = city;
            this.PostalCode = postalCode;
            this.Address = address;
        }

        // identifier of the FireAlarmSystem
        public int Id { get; set; }                         

        // name of the company that owns the FireAlarmSystem
        public string Company { get; set; }

        // short description of the FireAlarmSystem
        public string Description { get; set; }

        // country where the FireAlarmSystem is installed
        public string Country { get; set; }

        // city where the FireAlarmSystem is installed
        public string City { get; set; }

        // postal code of the city where the FireAlarmSystem is installed
        public int PostalCode { get; set; }

        // address of the building where the FireAlarmSystem is installed
        public string Address { get; set; }

        // list of the identifiers of the FireBrigades 
        // that should have access to certain information
        public HashSet<int> FireBrigades { get; set; }

        // list of the identifiers of the ServiceMembers 
        // that should have access to certain information
        public HashSet<int> ServiceMembers { get; set; }

        /// <summary>
        /// checks if the id of a FireBrigade is in the list 
        /// of FireBrigades of this FireAlarmSystem
        /// </summary>
        /// <param name="id">the id of the FireBrigade you want to check</param>
        /// <returns>returns true if the list of FireBrigades contains the id</returns>
        public bool CheckFireBrigade(int id)
        {
            bool found = false;
            foreach(int fb in FireBrigades)
            {
                if(fb == id)
                {
                    found = true;
                    break;
                }
            }
            return found;
        }

        /// <summary>
        /// checks if the id of a ServiceMember is in the list 
        /// of ServiceMembers of this FireAlarmSystem
        /// </summary>
        /// <param name="id">the id of the ServiceMember you want to check</param>
        /// <returns>returns true if the list of ServiceMembers contains the id</returns>
        public bool CheckServiceMember(int id) {
            bool found = false;
            foreach (int sm in ServiceMembers)
            {
                if (sm == id)
                {
                    found = true;
                    break;
                }
            }
            return found;
        }
    }
}
