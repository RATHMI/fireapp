using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;

namespace FireApp.Service.DatabaseOperations
{
    public static class ActiveEvents
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fe">The FireEvent you want to create a target of</param>
        /// <returns>returns true if Target was inserted or deleted</returns>
        public static bool UpsertActiveFireEvent(FireEvent fe)
        {
            if (fe.EventType == EventTypes.prealarm ||
                fe.EventType == EventTypes.alarm ||
                fe.EventType == EventTypes.disfunction ||
                fe.EventType == EventTypes.deactivated)
            {
                // insert into local database
                LocalDatabase.UpsertActiveFireEvent(fe);

                // insert into remote database                
                using (var db = AppData.ActiveFireEventDB())
                {
                    var table = db.ActiveFireEventTable();
                    return table.Upsert(fe);
                }
            }
            else
            {
                if (fe.EventType == EventTypes.reset)
                {
                    // delete active FireEvent from local database
                    LocalDatabase.DeleteActiveFireEvent(fe);

                    // delete active FireEvent from remote database
                    using (var db = AppData.ActiveFireEventDB())
                    {
                        var table = db.ActiveFireEventTable();
                        FireEvent target = table.FindOne(x => x.Id.SourceId == fe.Id.SourceId && x.TargetId == fe.TargetId);
                        if (target != null)
                        {
                            if (table.Delete(x => x.Id == target.Id) == 1)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns a list of all active FireEvents</returns>
        public static IEnumerable<FireEvent> GetAllActiveFireEvents()
        {
            return (IEnumerable<FireEvent>)LocalDatabase.GetActiveFireEvents();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventType">The EventType of the active FireEvents</param>
        /// <returns>returns a list of all active FireEvents of the given 
        /// TargetState</returns>
        public static IEnumerable<FireEvent> GetActiveFireEventsByEventType(EventTypes eventType)
        {
            List<FireEvent> events = LocalDatabase.GetActiveFireEvents();
            List<FireEvent> result = new List<FireEvent>();
            foreach (FireEvent fe in events)
            {
                if (fe.EventType == eventType)
                {
                    result.Add(fe);
                }
            }

            return (IEnumerable<FireEvent>)result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceId">the sourceId of the active FireEvents you want to look for</param>
        /// <returns>a list of active FireEvents with a matching sourceId</returns>
        public static IEnumerable<FireEvent> GetActiveFireEventsBySourceId(int sourceId)
        {
            List<FireEvent> events = LocalDatabase.GetActiveFireEvents();
            if (events != null)
            {
                return (IEnumerable<FireEvent>)events.Find(x => x.Id.SourceId == sourceId);
            }
            else
            {
                return null;
            }
        }

        /// <returns>returns all active FireEvents at the given date</returns>
        public static IEnumerable<FireEvent> GetActiveFireEventsByDate(int year, int month, int day)
        {
            List<FireEvent> events = LocalDatabase.GetActiveFireEvents();
            List<FireEvent> results = new List<FireEvent>();
            if (events != null)
            {
                foreach (FireEvent fe in events)
                {
                    if (fe.TimeStamp.Year == year && fe.TimeStamp.Month == month && fe.TimeStamp.Day == day)
                    {
                        results.Add(fe);
                    }
                }
                return results;
            }
            else
            {
                return null;
            }
        }

        /// <returns>returns all active FireEvents in the given month and year</returns>
        public static IEnumerable<FireEvent> GetActiveFireEventsByDate(int year, int month)
        {
            List<FireEvent> events = LocalDatabase.GetActiveFireEvents();
            List<FireEvent> results = new List<FireEvent>();
            if (events != null)
            {
                foreach (FireEvent fe in events)
                {
                    if (fe.TimeStamp.Year == year && fe.TimeStamp.Month == month)
                    {
                        results.Add(fe);
                    }
                }
                return results;
            }
            else
            {
                return null;
            }
        }

        /// <returns>returns all active FireEvents in the given year</returns>
        public static IEnumerable<FireEvent> GetActiveFireEventsByDate(int year)
        {
            List<FireEvent> events = LocalDatabase.GetActiveFireEvents();
            List<FireEvent> results = new List<FireEvent>();
            if (events != null)
            {
                foreach (FireEvent fe in events)
                {
                    if (fe.TimeStamp.Year == year)
                    {
                        results.Add(fe);
                    }
                }
                return results;
            }
            else
            {
                return null;
            }
        }

        /// <returns>returns all active FireEvents from the given sourceId at the given date</returns>
        public static IEnumerable<FireEvent> GetActiveFireEventsBySourceIdDate(int sourceId, int year, int month, int day)
        {
            List<FireEvent> events = DatabaseOperations.GetActiveFireEventsBySourceId(sourceId).ToList<FireEvent>();
            List<FireEvent> results = new List<FireEvent>();
            if (events != null)
            {
                foreach (FireEvent fe in events)
                {
                    if (fe.TimeStamp.Year == year && fe.TimeStamp.Month == month && fe.TimeStamp.Day == day)
                    {
                        results.Add(fe);
                    }
                }
                return results;
            }
            else
            {
                return null;
            }
        }

        /// <returns>returns all active FireEvents from the given sourceId in the given month and year</returns>
        public static IEnumerable<FireEvent> GetActiveFireEventsBySourceIdDate(int sourceId, int year, int month)
        {
            List<FireEvent> events = DatabaseOperations.GetActiveFireEventsBySourceId(sourceId).ToList<FireEvent>();
            List<FireEvent> results = new List<FireEvent>();
            if (events != null)
            {
                foreach (FireEvent fe in events)
                {
                    if (fe.TimeStamp.Year == year && fe.TimeStamp.Month == month)
                    {
                        results.Add(fe);
                    }
                }
                return results;
            }
            else
            {
                return null;
            }
        }

        /// <returns>returns all active FireEvents from the given sourceId in the given year</returns>
        public static IEnumerable<FireEvent> GetActiveFireEventsBySourceIdDate(int sourceId, int year)
        {
            List<FireEvent> events = DatabaseOperations.GetActiveFireEventsBySourceId(sourceId).ToList<FireEvent>();
            List<FireEvent> results = new List<FireEvent>();
            if (events != null)
            {
                foreach (FireEvent fe in events)
                {
                    if (fe.TimeStamp.Year == year)
                    {
                        results.Add(fe);
                    }
                }
                return results;
            }
            else
            {
                return null;
            }
        }
    }
}