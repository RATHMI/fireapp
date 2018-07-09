using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;
using FireApp.Service.Cache;

namespace FireApp.Service
{
    /// <summary>
    /// this class is for storing the FireEvents in a cache
    /// </summary>
    public static class LocalDatabase
    {
        // the key to retrieve the list of all FireEvents from the cache
        static string allFireEventsString = "allFireEvents";

        // the key to retrieve the list of active FireEvents from the cache
        static string activeFireEventsString = "activeFireEvents";

        static LocalDatabase(){}

        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns a List of all FireEvents that are stored in the cache</returns>
        public static List<FireEvent> GetAllFireEvents()
        {
            List<FireEvent> rv = (List<FireEvent>)GlobalCachingProvider.Instance.GetItem(allFireEventsString, false);
            if(rv == null)
            {
                rv = new List<FireEvent>();
            }
            return rv;
        }

        /// <summary>
        /// Inserts or updates an active FireEvent in the cache
        /// </summary>
        /// <param name="fe">active FireEvent that should be stored in the cache</param>
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

        /// <summary>
        /// Inserts or updates a FireEvent in the cache
        /// </summary>
        /// <param name="fe">FireEvent that should be stored in the cache</param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns a list of all active FireEvents from the cache</returns>
        public static List<FireEvent> GetActiveFireEvents()
        {
            List<FireEvent> rv = (List<FireEvent>)GlobalCachingProvider.Instance.GetItem(activeFireEventsString, false);
            if (rv == null)
            {
                rv = new List<FireEvent>();
            }
            return rv;
        }

        /// <summary>
        /// Deletes the active FireEvent 'fe' from the cache
        /// </summary>
        /// <param name="fe">The active FireEvent that should be deleted from the cache</param>
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
                if (activeFireEvents != null)   // Server crashes if you try to insert null
                {
                    GlobalCachingProvider.Instance.AddItem(activeFireEventsString, activeFireEvents);
                }
            }
        }

        /// <summary>
        /// Queries all FireEvents from the database and stores it in the cache
        /// </summary>
        public static void InitializeDatabase()
        {
            List<FireEvent> all = (DatabaseOperations.QueryFireEvents()).ToList<FireEvent>();
            List<FireEvent> active = (DatabaseOperations.QueryActiveFireEvents()).ToList<FireEvent>();
            
            if(all != null)     // trying to insert null into the cache creates a server error
            {
                GlobalCachingProvider.Instance.AddItem(allFireEventsString, all);
            }
            else
            {
                GlobalCachingProvider.Instance.AddItem(allFireEventsString, new List<FireEvent>());
            }

            if(active != null)  // trying to insert null into the cache creates a server error
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