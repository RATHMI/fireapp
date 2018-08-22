using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireApp.Domain
{
    public class UserGroup
    {
        protected UserGroup() { }

        public UserGroup(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        // public virtual UserTypes UserType { get; }       
    }
}
