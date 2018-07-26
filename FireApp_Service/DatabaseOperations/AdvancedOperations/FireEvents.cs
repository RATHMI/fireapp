using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;
using System.Threading;
using FireApp.Service;
using System.Net;

namespace FireApp.Service.DatabaseOperations.AdvancedOperations
{
    public static class FireEvents
    {       

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
            IEnumerable<FireEvent> events = BasicOperations.FireEvents.GetBySourceId(sourceId);
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
            IEnumerable<FireEvent> events = LocalDatabase.GetAllFireEvents();
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
        public static Int32[] CountByEventTypePerYear(EventTypes eventType, int year)
        {
            // Find all FireEvents with a matching EventType.
            IEnumerable<FireEvent> events = GetByEventType(eventType);
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
    }
}