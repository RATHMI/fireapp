using FireApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FireApp.Service.DatabaseOperations.BasicOperations
{
    public class FireEvents
    {
        /// <summary>
        /// Fetches all FireEvents from the cache.
        /// </summary>
        /// <returns>Returns all FireEvents.</returns>
        public static IEnumerable<FireEvent> GetAll()
        {
            return LocalDatabase.GetAllFireEvents();
        }

        /// <summary>
        /// Inserts a FireEvent into the database or updates it if it already exists.
        /// </summary>
        /// <param name="fe">FireEvent that should be inserted into the Database.</param>
        /// <returns>Returns true if the FireEvent was inserted.</returns>
        public static bool Upsert(FireEvent fe)
        {
            if (fe != null)
            {
                DatabaseOperations.BasicOperations.ActiveEvents.Upsert(fe);
                LocalDatabase.UpsertFireEvent(fe);

                return DatabaseOperations.DbUpserts.UpsertFireEvent(fe);
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
            IEnumerable<FireEvent> all = LocalDatabase.GetAllFireEvents();
            foreach (FireEvent fe in all)
            {
                if (fe.Id.SourceId == id.SourceId && fe.Id.EventId == id.EventId)
                {
                    return false;
                }
            }
            return true;
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
            foreach (FireEvent fe in events)
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
        /// Returns all FireEvents from one FireAlarmSystem.
        /// </summary>
        /// <param name="sourceId">The sourceId of the FireAlarmSystem that sent 
        /// the FireEvent.</param>
        /// <returns>Returns a list of all Fireevents with a matching sourceId 
        /// (all Fireevents from a distinct fire alarm system).</returns>
        public static IEnumerable<FireEvent> GetBySourceId(int sourceId)
        {
            IEnumerable<FireEvent> events = LocalDatabase.GetAllFireEvents();
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
    }
}