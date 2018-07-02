using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireApp.Domain {
    public class FireEvent {

        public int Id { get; set; }
        public DateTime At { get; set; }

        public string By { get; set; }

        public string Name { get; set; }
    }
}
