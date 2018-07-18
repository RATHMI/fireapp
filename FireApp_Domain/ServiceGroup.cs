﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireApp.Domain
{
    public class ServiceGroup
    {
        private ServiceGroup() { }

        public ServiceGroup(int id, string groupName)
        {
            this.Id = id;
            this.GroupName = groupName;
        }

        public int Id { get; set; }

        public string GroupName { get; set; }
    }
}