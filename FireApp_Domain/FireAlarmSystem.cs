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
            ServiceMembers = new HashSet<int>();
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
            ServiceMembers = new HashSet<int>();
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
        /// checks if the id of a ServiceMember is in the list 
        /// of ServiceMembers of this FireAlarmSystem
        /// </summary>
        /// <param name="id">the id of the ServiceMember you want to check</param>
        /// <returns>returns true if the list of ServiceMembers contains the id</returns>
        public bool CheckServiceMember(int id) {
            bool found = false;
            if (this.ServiceMembers != null)
            {
                foreach (int sm in this.ServiceMembers)
                {
                    if (sm == id)
                    {
                        found = true;
                        break;
                    }
                }
            }
            return found;
        }

        public string ToLog()
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append("FireAlarmSystem [");

                sb.Append("id=");
                sb.Append(Id.ToString());

                sb.Append(",company=");
                sb.Append(Company.ToString());

                sb.Append(",description=");
                sb.Append(Description.ToString());

                sb.Append(",country=");
                sb.Append(Country.ToString());

                sb.Append(",city=");
                sb.Append(City.ToString());

                sb.Append(",postalcode=");
                sb.Append(PostalCode.ToString());

                sb.Append(",address=");
                sb.Append(Address.ToString());

                sb.Append(",firebrigades={");
                if (FireBrigades.Count > 1)
                {
                    foreach (int fb in FireBrigades)
                    {
                        sb.Append(fb.ToString());
                        sb.Append(";");
                    }
                    sb.Remove(sb.Length - 1, 1);
                }
                else
                {
                    sb.Append(FireBrigades.First<int>());
                }
                sb.Append("}");

                sb.Append(",servicemembers={");

                if (ServiceMembers.Count > 1)
                {
                    foreach (int sm in ServiceMembers)
                    {
                        sb.Append(sm.ToString());
                        sb.Append(";");
                    }
                    sb.Remove(sb.Length - 1, 1);
                }
                else
                {
                    sb.Append(ServiceMembers.First<int>());
                }
                sb.Append("}");
                sb.Append("]");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return sb.ToString();
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
            foreach (int sm in this.ServiceMembers)
            {
                fas.ServiceMembers.Add(sm);
            }

            return fas;
        }
    }
}
