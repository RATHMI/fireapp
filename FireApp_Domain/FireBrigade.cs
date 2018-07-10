using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireApp.Domain
{
    public class FireBrigade
    {
        public FireBrigade() { }

        //todo: add constructor

        public int Id { get; set; }

        public string Name { get; set; }

        public UserLogin Login { get; set; }
    }
}
