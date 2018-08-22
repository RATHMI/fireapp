using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;
using System.Threading;
using FireApp.Service;
using System.Net;
using static FireApp.Domain.FireEvent;
using FireApp.Domain.Extensionmethods;

namespace FireApp.Service.DatabaseOperations
{
    public static class FireEvents
    {       

        /// <summary>
        /// Fetches all FireEvents from the cache.
        /// </summary>
        /// <returns>Returns all FireEvents.</returns>
        public static IEnumerable<FireEvent> GetAll()
        {
            return DatabaseOperations.DbQueries.QueryFireEvents();
        }

        /// <summary>
        /// Inserts a FireEvent into the database or updates it if it already exists.
        /// </summary>
        /// <param name="fe">FireEvent that should be inserted into the Database.</param>
        /// <returns>Returns true if the FireEvent was inserted.</returns>
        public static bool Upsert(FireEvent fe, User user)
        {
            if (fe != null && fe.Id.SourceId != 0 && fe.Id.EventId != 0)
            {
                bool ok = DatabaseOperations.DbUpserts.UpsertFireEvent(fe);
                if (ok)
                {
                    Logging.Logger.Log("upsert", user.GetUserDescription(), fe);

                    DatabaseOperations.ActiveFireEvents.Upsert(fe);                  
                }

                return ok;
            }
            else
            {
                return false;
            }
        }      

        /// <summary>
        /// Checks if an id is already used by another FireEvent.
        /// </summary>
        /// <param name="id">The id you want to check.</param>
        /// <returns>Returns true if id is not used by other FireEvent.</returns>
        public static bool CheckId(FireEventId id)
        {
            IEnumerable<FireEvent> all = GetAll();
            if(id.SourceId == 0 || id.EventId == 0)
            {
                return false;
            }

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
        /// Returns all FireEvents from one FireAlarmSystem.
        /// </summary>
        /// <param name="sourceId">The sourceId of the FireAlarmSystem that sent 
        /// the FireEvent.</param>
        /// <returns>Returns a list of all Fireevents with a matching sourceId 
        /// (all Fireevents from a distinct fire alarm system).</returns>
        public static IEnumerable<FireEvent> GetBySourceId(int sourceId)
        {
            IEnumerable<FireEvent> events = GetAll();
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
        /// Returns a distinct FireEvent.
        /// </summary>
        /// <param name="sourceId">The sourceId of the FireAlarmSystem that sent 
        /// the FireEvent.</param>
        /// <param name="eventId">The ongoing number of the FireEvents of one
        /// FireAlarmSystem.</param>
        /// <returns>Returns a distinct FireEvent with a matching sourceId and eventId 
        /// (a FireEvent from a distinct fireAlarmSystem with the matching eventId).</returns>
        public static FireEvent GetById(int sourceId, int eventId)
        {
            // Get all FireEvents from the FireAlarmSystem.
            IEnumerable<FireEvent> events = GetBySourceId(sourceId);

            // Find FireEvent with matching eventId.
            foreach(FireEvent fe in events)
            {
                if (fe.Id.EventId == eventId)
                {
                    return fe;                    
                }
            }

            // If no matching FireEvent was found.
            throw new KeyNotFoundException();
        }

        /// <summary>
        /// Returns all FireEvents of a disinct target of a FireAlarmSystem.
        /// </summary>
        /// <param name="sourceId">The sourceId of the FireAlarmSystem.</param>
        /// <param name="targetId">The id of the fire detector that sent the 
        /// FireEvent.</param>
        /// <returns>Returns a list of all FireEvents with matching sourceId and targetId.</returns>
        public static IEnumerable<FireEvent> GetByTarget(int sourceId, string targetId)
        {
            // Get all FireEvents from the FireAlarmSystem.
            IEnumerable<FireEvent> events = GetBySourceId(sourceId);
            List<FireEvent> results = new List<FireEvent>();

            // Find all FireEvents with a matching targetId.
            foreach (FireEvent fe in events)
            {
                if (fe.TargetId == targetId)
                {
                    results.Add(fe);
                }
            }

            return results;
        }

        /// <summary>
        /// Finds all FireEvents with a matching EventType.
        /// </summary>
        /// <param name="eventType">The EventType of the FireEvents.</param>
        /// <returns>Returns a list of all FireEvents with a matching EventType.</returns>
        public static IEnumerable<FireEvent> GetByEventType(EventTypes eventType)
        {
            IEnumerable<FireEvent> events = GetAll();
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
        /// Finds all FireEvents with a matching EventType that occured in the given year.
        /// Returns the number of FireEvents per month.
        /// </summary>
        /// <param name="eventType">The EventType of the FireEvents.</param>
        /// <param name="year">The year of the FireEvents' TimeStamp.</param>
        /// <returns>Returns an array with the number of FireEvents of the given EventType 
        /// that occured in the given year. Each column represents one month.</returns>
        public static Int32[] CountByEventTypePerYear(EventTypes eventType, int year, User user)
        {
            // Find all FireEvents with a matching EventType.
            IEnumerable<FireEvent> events = GetByEventType(eventType);
            events = Filter.FireEventsFilter.UserFilter(events, user);

            Int32[] months = new Int32[12];

            foreach (FireEvent fe in events)
            {
                // If the FireEvent occured in the given year 
                if (fe.TimeStamp.Year == year)
                {
                    // increment the counter of the right month
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

        /// <summary>
        /// Returns a list of the latest FireEvents where there is no FireAlarmSystem with a matching Id.
        /// </summary>
        /// <returns>Returns a list of FireEvents.</returns>
        public static IEnumerable<FireEvent> GetUnregistered()
        {
            List<FireAlarmSystem> fireAlarmSystems = new List<FireAlarmSystem>();
            List<FireEvent> events = new List<FireEvent>();   

            try
            {
                foreach (int id in DatabaseOperations.FireAlarmSystems.GetUnregistered())
                {
                    try
                    {
                        events.Add(GetBySourceId(id).OrderBy(x => x.TimeStamp).First());
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }

                return events;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return events;
            }

        }
    }
}