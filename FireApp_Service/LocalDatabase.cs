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
            List<FireEvent> rv = (List<FireEvent>)GlobalCachingProvider.Instance.GetItem(allFireEventsString, false);
            if(rv == null)
            {
                rv = new List<FireEvent>();
            }
            return rv;
        }

        public static void UpsertActiveFireEvent(FireEvent fe)
        {
            List<FireEvent> activeFireEvents = GetActiveFireEvents();
            FireEvent old = null;

            foreach (FireEvent f in activeFireEvents)
            {
                if(f.Id.SourceId == fe.Id.SourceId && f.TargetId == fe.TargetId)
                {
                    old = f;
                    break;
                }
            }                        

            if (old != null)
            {
                activeFireEvents.Remove(old);
                activeFireEvents.Add(fe);
            }
            else
            {
                activeFireEvents.Add(fe);
            }

            GlobalCachingProvider.Instance.RemoveItem(activeFireEventsString);
            GlobalCachingProvider.Instance.AddItem(activeFireEventsString, activeFireEvents);
        }

        public static void UpsertFireEvent(FireEvent fe)
        {
            List<FireEvent> allFireEvents = GetAllFireEvents();
            FireEvent old = null;

            foreach (FireEvent f in allFireEvents)
            {
                if (f.Id.SourceId == fe.Id.SourceId && f.TargetId == fe.TargetId)
                {
                    old = f;
                    break;
                }
            }

            if (old != null)
            {
                allFireEvents.Remove(old);
                allFireEvents.Add(fe);
            }
            else
            {
                allFireEvents.Add(fe);
            }

            GlobalCachingProvider.Instance.RemoveItem(activeFireEventsString);
            GlobalCachingProvider.Instance.AddItem(activeFireEventsString, allFireEvents);
        }

        public static List<FireEvent> GetActiveFireEvents()
        {
            List<FireEvent> rv = (List<FireEvent>)GlobalCachingProvider.Instance.GetItem(activeFireEventsString, false);
            if (rv == null)
            {
                rv = new List<FireEvent>();
            }
            return rv;
        }

        public static void DeleteActiveFireEvent(FireEvent fe)
        {
            List<FireEvent> activeFireEvents = GetActiveFireEvents();
            FireEvent old = null;

            foreach (FireEvent f in activeFireEvents)
            {
                if (f.Id.SourceId == fe.Id.SourceId && f.TargetId == fe.TargetId)
                {
                    old = f;
                    break;
                }
            }

            if (old != null)
            {
                activeFireEvents.Remove(fe);
                GlobalCachingProvider.Instance.RemoveItem(activeFireEventsString);
                if (activeFireEvents != null)
                {
                    GlobalCachingProvider.Instance.AddItem(activeFireEventsString, activeFireEvents);
                }
            }
        }

        public static void InitializeDatabase()
        {
            List<FireEvent> all = (List<FireEvent>)DatabaseOperations.QueryFireEvents();
            List<FireEvent> active = (List<FireEvent>)DatabaseOperations.QueryActiveFireEvents();
        
            if(all != null)
            {
                GlobalCachingProvider.Instance.AddItem(allFireEventsString, all);
            }
            else
            {
                GlobalCachingProvider.Instance.AddItem(allFireEventsString, new List<FireEvent>());
            }

            if (active != null)
            {
                GlobalCachingProvider.Instance.AddItem(activeFireEventsString, active);
            }
            else
            {
                GlobalCachingProvider.Instance.AddItem(activeFireEventsString, new List<FireEvent>());
            }
        }
    }
}