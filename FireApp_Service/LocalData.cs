using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;

namespace FireApp.Service
{
    public class LocalData
    {
        public List<FireEvent> allFireEvents;
        public List<FireEvent> activeFireEvents;

        // May use instead of DB in case of performance issues

        public LocalData()
        {
            allFireEvents = DatabaseOperations.GetAllFireEvents().ToList<FireEvent>();
        }

        public void AddFireEvent(FireEvent fe)
        {
            allFireEvents.Add(fe);
            //todo: check active FireEvents
        }
    }
}