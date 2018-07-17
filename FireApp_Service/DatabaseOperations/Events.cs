using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;
using System.Threading;
using FireApp.Service;
using System.Net;

namespace FireApp.Service.DatabaseOperations
{
    public static class Events
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns all FireEvents from local Database</returns>
        public static IEnumerable<FireEvent> GetAllFireEvents()
        {
            return (IEnumerable<FireEvent>)LocalDatabase.GetAllFireEvents();
        }

        /// <summary>
        /// inserts a FireEvent into the database or updates it if it already exists
        /// </summary>
        /// <param name="fe">FireEvent that should be inserted into the Database</param>
        /// <returns>returns true if new object was inserted</returns>
        public static bool UpsertFireEvent(FireEvent fe)
        {
            if (fe != null)
            {
                Logging.Logger.Log("upsert", fe);
                DatabaseOperations.ActiveEvents.UpsertActiveFireEvent(fe);
                LocalDatabase.UpsertFireEvent(fe);

                return DatabaseOperations.DbUpserts.UpsertFireEvent(fe);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if an id is already used by another FireEvent
        /// </summary>
        /// <param name="id">the id you want to check</param>
        /// <returns>returns true if id is not used by other FireEvent</returns>
        public static bool CheckId(FireEventId id)
        {
            List<FireEvent> all = LocalDatabase.GetAllFireEvents();
            foreach(FireEvent fe in all)
            {
                if(fe.Id.SourceId == id.SourceId && fe.Id.EventId == id.EventId)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceId">The sourceId of the FireAlarmSystem that sent 
        /// the FireEvent</param>
        /// <returns>returns a list of all Fireevents with a matching sourceId 
        /// (all Fireevents from a distinct fire alarm system)</returns>
        public static IEnumerable<FireEvent> GetFireEventsBySourceId(int sourceId)
        {
            List<FireEvent> events = LocalDatabase.GetAllFireEvents();
            List<FireEvent> results = new List<FireEvent>();
            foreach (FireEvent fe in events)
            {
                if (fe.Id.SourceId == sourceId)
                {
                    results.Add(fe);
                }
            }

            return results;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceId">The sourceId of the FireAlarmSystem that sent 
        /// the FireEvent</param>
        /// <param name="eventId">The ongoing number of the FireEvents of one
        /// FireAlarmSystem</param>
        /// <returns>returns a distinct FireEvent with a matching sourceId and eventId 
        /// (a FireEvent from a distinct fireAlarmSystem with the matching eventId)</returns>
        public static IEnumerable<FireEvent> GetFireEventById(int sourceId, int eventId)
        {
            List<FireEvent> events = GetFireEventsBySourceId(sourceId).ToList<FireEvent>();
            List<FireEvent> results = new List<FireEvent>();
            foreach(FireEvent fe in events)
            {
                if (fe.Id.EventId == eventId)
                {
                    results.Add(fe);
                    break;
                }
            }

            return results;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceId">The sourceId of the FireAlarmSystem that sent 
        /// the FireEvent</param>
        /// <param name="targetId">The id of the fire detector that sent the 
        /// FireEvent</param>
        /// <returns>returns a list of all FireEvents with matching sourceId and targetId</returns>
        public static IEnumerable<FireEvent> GetFireEventsBySourceIdTargetId(int sourceId, string targetId)
        {
            List<FireEvent> events = (GetFireEventsBySourceId(sourceId)).ToList<FireEvent>();
            List<FireEvent> results = new List<FireEvent>();
            foreach(FireEvent fe in events)
            {
                if(fe.TargetId == targetId)
                {
                    results.Add(fe);
                }
            }

            return results;           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceId">The sourceId of the FireAlarmSystem that sent 
        /// the FireEvent</param>
        /// <param name="eventType">The EventType of the FireEvent</param>
        /// <returns>returns a list of all FireEvents with matching sourceId and eventType</returns>
        public static IEnumerable<FireEvent> GetFireEventsBySourceIdEventType(int sourceId, EventTypes eventType)
        {
            List<FireEvent> events = (GetFireEventsBySourceId(sourceId)).ToList<FireEvent>();
            List<FireEvent> results = new List<FireEvent>();
            foreach (FireEvent fe in events)
            {
                if (fe.EventType == eventType)
                {
                    results.Add(fe);
                }
            }

            return results;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventType">The EventType of the FireEvents</param>
        /// <returns>returns a list of all FireEvents with matching eventType</returns>
        public static IEnumerable<FireEvent> GetFireEventsByEventType(EventTypes eventType)
        {
            List<FireEvent> events = LocalDatabase.GetAllFireEvents();
            List<FireEvent> results = new List<FireEvent>();
            foreach(FireEvent fe in events)
            {
                if(fe.EventType == eventType)
                {
                    results.Add(fe);
                }
            }

            return results;            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceId">The sourceId of the FireAlarmSystem that sent 
        /// the FireEvent</param>
        /// <param name="startTime">The minimal value of the TimeStamp of the FireEvents</param>
        /// <param name="endTime">The maximal value of the TimeStamp of the FireEvents</param>
        /// <returns>returns a list of all FireEvents with a matching sourceId and and a Timestamp between 
        /// startTime and endTime</returns>
        public static IEnumerable<FireEvent> GetFireEventsBySourceIdTimespan(int sourceId, long startTime, long endTime)
        {
            List<FireEvent> events = (GetFireEventsBySourceId(sourceId)).ToList<FireEvent>();
            List<FireEvent> result = new List<FireEvent>();

            foreach (FireEvent fe in events)
            {
                if (fe.TimeStamp >= new DateTime(startTime) && fe.TimeStamp <= new DateTime(endTime))
                {
                    result.Add(fe);
                }
            }

            return result;           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startTime">The minimal value of the TimeStamp of the FireEvents</param>
        /// <param name="endTime">The maximal value of the TimeStamp of the FireEvents</param>
        /// <returns>returns a list of all FireEvents with a Timestamp between 
        /// startTime and endTime</returns>
        public static IEnumerable<FireEvent> GetFireEventsByTimespan(long startTime, long endTime)
        {
            List<FireEvent> events = LocalDatabase.GetAllFireEvents();
            List<FireEvent> result = new List<FireEvent>();
            foreach (FireEvent fe in events)
            {
                if (fe.TimeStamp >= new DateTime(startTime) && fe.TimeStamp <= new DateTime(endTime))
                {
                    result.Add(fe);
                }
            }

            return result;
        }

        /// <returns>returns all FireEvents from the given sourceId at the given date</returns>
        public static IEnumerable<FireEvent> GetFireEventsByDate(int sourceId, int year, int month, int day)
        {
            List<FireEvent> events = GetFireEventsBySourceId(sourceId).ToList<FireEvent>();
            List<FireEvent> results = new List<FireEvent>();

            foreach (FireEvent fe in events)
            {
                if (fe.TimeStamp.Year == year && fe.TimeStamp.Month == month && fe.TimeStamp.Day == day)
                {
                    results.Add(fe);
                }
            }

            return results;
        }

        /// <returns>returns all FireEvents from the given sourceId in the given year and month</returns>
        public static IEnumerable<FireEvent> GetFireEventsByDate(int sourceId, int year, int month)
        {
            List<FireEvent> events = GetFireEventsBySourceId(sourceId).ToList<FireEvent>();
            List<FireEvent> results = new List<FireEvent>();

            foreach (FireEvent fe in events)
            {
                if (fe.TimeStamp.Year == year && fe.TimeStamp.Month == month)
                {
                    results.Add(fe);
                }
            }

            return results;
        }

        /// <returns>returns all FireEvents from the given sourceId in the given year</returns>
        public static IEnumerable<FireEvent> GetFireEventsByDate(int sourceId, int year)
        {
            List<FireEvent> events = GetFireEventsBySourceId(sourceId).ToList<FireEvent>();
            List<FireEvent> results = new List<FireEvent>();

            foreach (FireEvent fe in events)
            {
                if (fe.TimeStamp.Year == year)
                {
                    results.Add(fe);
                }
            }

            return results;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventType">The EventType of the FireEvents</param>
        /// <param name="year">The year of the FireEvents' TimeStamp</param>
        /// <returns>returns an array with the number of FireEvents of the given EventType 
        /// that occured in the given year. Each column represents one month</returns>
        public static Int32[] CountFireEventsByEventTypePerYear(EventTypes eventType, int year)
        {
            IEnumerable<FireEvent> events = GetFireEventsByEventType(eventType);
            Int32[] months = new Int32[12];

            foreach (FireEvent fe in events)
            {
                if (fe.TimeStamp.Year == year)
                {
                    switch (fe.TimeStamp.Month)
                    {
                        case 1: months[0]++; break;
                        case 2: months[1]++; break;
                        case 3: months[2]++; break;
                        case 4: months[3]++; break;
                        case 5: months[4]++; break;
                        case 6: months[5]++; break;
                        case 7: months[6]++; break;
                        case 8: months[7]++; break;
                        case 9: months[8]++; break;
                        case 10: months[9]++; break;
                        case 11: months[10]++; break;
                        case 12: months[11]++; break;
                    }
                }
            }

            return months;
        }

    }
}