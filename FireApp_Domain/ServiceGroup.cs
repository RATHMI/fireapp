using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireApp.Domain
{
    /// <summary>
    /// This class is used as a link between Users and FireAlarmSystems.
    /// </summary>
    public class ServiceGroup : UserGroup
    {
        protected static new UserTypes userType = UserTypes.servicemember;

        public ServiceGroup(int id, string name) : base(id, name)
        {
        }            
    }
}
