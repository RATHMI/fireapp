using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;
using FireApp.Service.Cache;

namespace FireApp.Service
{
    public static class LocalDatabase
    {
        static string allFireEventsString = "allFireEvents";
        static string activeFireEventsString = "activeFireEvents";

        static LocalDatabase(){}

        public static List<FireEvent> GetAllFireEvents()
        {
            return (List<FireEvent>)GlobalCachingProvider.Instance.GetItem(allFireEventsString, false);
        }

        public static void UpsertActiveFireEvent(FireEvent fe)
        {
            List<FireEvent> activeFireEvents = GetActiveFireEvents();
            FireEvent old = activeFireEvents.FirstOrDefault(x => x.Id.SourceId == fe.Id.SourceId && x.TargetId == fe.TargetId);
            if (old != null)
            {
                old = fe;
            }
            else
            {
                activeFireEvents.Add(fe);
            }
            GlobalCachingProvider.Instance.RemoveItem(activeFireEventsString);
            GlobalCachingProvider.Instance.AddItem(activeFireEventsString, activeFireEvents);
        }

        public static List<FireEvent> GetActiveFireEvents()
        {
            return (List<FireEvent>)GlobalCachingProvider.Instance.GetItem(activeFireEventsString, false);
        }

        public static void DeleteActiveFireEvent(FireEvent fe)
        {
            List<FireEvent> activeFireEvents = GetActiveFireEvents();
            FireEvent old = activeFireEvents.FirstOrDefault(x => x.Id.SourceId == fe.Id.SourceId && x.TargetId == fe.TargetId);
            activeFireEvents.Remove(fe);
            GlobalCachingProvider.Instance.RemoveItem(activeFireEventsString);
            GlobalCachingProvider.Instance.AddItem(activeFireEventsString, activeFireEvents);
        }

        public static void InitializeDatabase(List<FireEvent> all, List<FireEvent> active)
        {
            GlobalCachingProvider.Instance.AddItem(allFireEventsString, all);
            GlobalCachingProvider.Instance.AddItem(activeFireEventsString, active);
        }
    }
}