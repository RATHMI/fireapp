using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireApp.Domain
{
    public class ServiceMember
    {
        public ServiceMember() { }

        public int Id { get; set; }

        public string ForeName { get; set; }

        public string LastName { get; set; }

        public UserLogin Login { get; set; }
    }
}
