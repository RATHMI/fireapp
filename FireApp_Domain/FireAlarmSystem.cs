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
    public class FireAlarmSystem : ICloneable
    {
        private FireAlarmSystem() {}

        public FireAlarmSystem(int id)
        {
            this.Id = id;
            this.Company = "";
            this.Description = "";
            this.Country = "";
            this.City = "";
            this.PostalCode = 0;
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
            FireBrigades = new HashSet<int>();
            ServiceGroups = new HashSet<int>();
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

        // list of the identifiers of the ServiceGroups 
        // that should have access to certain information
        public HashSet<int> ServiceGroups { get; set; }

        /// <summary>
        /// checks if the id of a FireBrigade is in the list 
        /// of FireBrigades of this FireAlarmSystem
        /// </summary>
        /// <param name="id">the id of the FireBrigade you want to check</param>
        /// <returns>returns true if the list of FireBrigades contains the id</returns>
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
        /// checks if the id of a ServiceGroup is in the list 
        /// of ServiceGroups of this FireAlarmSystem
        /// </summary>
        /// <param name="id">the id of the ServiceGroup you want to check</param>
        /// <returns>returns true if the list of ServiceGroups contains the id</returns>
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
    }
}
