using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireApp.Domain
{
    public class FireAlarmSystem
    {
        public FireAlarmSystem() { }

        public int Id { get; set; }

        public string Company { get; set; }

        public string Description { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public int PostalCode { get; set; }

        public string Address { get; set; }

        public List<int> FireBrigades { get; set; }

        public List<int> ServiceMembers { get; set; }
    }
}
