using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;

namespace FireApp.Service
{
    public class LocalDatabase
    {
        private List<FireEvent> allFireEvents;
        private List<FireEvent> activeFireEvents;

        public LocalDatabase()
        {
            allFireEvents = DatabaseOperations.QueryFireEvents().ToList<FireEvent>();
            activeFireEvents = DatabaseOperations.QueryActiveFireEvents().ToList<FireEvent>();
        }

        public List<FireEvent> GetAllFireEvents()
        {
            return allFireEvents;
        }

        public void UpsertActiveFireEvent(FireEvent fe)
        {
            FireEvent old = activeFireEvents.FirstOrDefault(x => x.Id.SourceId == fe.Id.SourceId && x.TargetId == fe.TargetId);
            if (old != null)
            {
                old = fe;
            }
            else
            {
                activeFireEvents.Add(fe);
            }
        }

        public List<FireEvent> GetActiveFireEvents()
        {
            return activeFireEvents;
        }

        public void DeleteActiveFireEvent(FireEvent fe)
        {
            FireEvent old = activeFireEvents.FirstOrDefault(x => x.Id.SourceId == fe.Id.SourceId && x.TargetId == fe.TargetId);
            activeFireEvents.Remove(fe);
        }
    }
}