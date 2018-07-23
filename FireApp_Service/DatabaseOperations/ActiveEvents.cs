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
        /// Upserts a FireEvents into the databases.
        /// </summary>
        /// <param name="fe">The FireEvent you want to upsert.</param>
        /// <returns>Returns true if FireEvent was inserted or deleted.</returns>
        public static bool Upsert(FireEvent fe)
        {
            // If the EventType is one of these it is an active FireEvent.
            if (fe.EventType == EventTypes.prealarm ||
                fe.EventType == EventTypes.alarm ||
                fe.EventType == EventTypes.disfunction ||
                fe.EventType == EventTypes.deactivated)
            {
                // Insert into local database.
                LocalDatabase.UpsertActiveFireEvent(fe);

                // Insert into remote database.                
                DatabaseOperations.DbUpserts.UpsertActiveFireEvent(fe);
            }
            else
            {
                // If the EventType is reset delete active FireEvents of this target.
                if (fe.EventType == EventTypes.reset)
                {
                    // Delete active FireEvent from local database.
                    LocalDatabase.DeleteActiveFireEvent(fe);

                    // Delete active FireEvent from remote database.
                    return DatabaseOperations.DbDeletes.DeleteActiveFireEvent(fe);
                }
            }

            // All other EventTypes.
            return false;
        }

        /// <summary>
        /// Returns all active FireEvents.
        /// </summary>
        /// <returns>Returns a list of all active FireEvents.</returns>
        public static IEnumerable<FireEvent> GetAll()
        {
            return LocalDatabase.GetActiveFireEvents();
        }

        /// <summary>
        /// Returns a distinct active FireEvent.
        /// </summary>
        /// <param name="sourceId">The sourceId of the active FireEvent you are looking for.</param>
        /// <param name="targetId">The targetId of the active FireEvent you are looking for.</param>
        /// <returns>Returns a FireEvent with a matching sourceId and targetId.</returns>
        public static IEnumerable<FireEvent> GetByTarget(int sourceId, string targetId)
        {
            IEnumerable<FireEvent> events = LocalDatabase.GetActiveFireEvents();
            List<FireEvent> result = new List<FireEvent>();
            foreach (FireEvent fe in events)
            {
                if (fe.Id.SourceId == sourceId && fe.TargetId == targetId)
                {
                    result.Add(fe);
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventType">The EventType of the active FireEvents.</param>
        /// <returns>Returns a list of all active FireEvents of the given EventType.</returns>
        public static IEnumerable<FireEvent> GetByEventType(EventTypes eventType)
        {
            IEnumerable<FireEvent> events = LocalDatabase.GetActiveFireEvents();
            List<FireEvent> result = new List<FireEvent>();
            foreach (FireEvent fe in events)
            {
                if (fe.EventType == eventType)
                {
                    result.Add(fe);
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceId">The sourceId of the active FireEvents you want to look for.</param>
        /// <returns>Returns a list of active FireEvents with a matching sourceId.</returns>
        public static IEnumerable<FireEvent> GetBySourceId(int sourceId)
        {
            IEnumerable<FireEvent> events = LocalDatabase.GetActiveFireEvents();
            List<FireEvent> results = new List<FireEvent>();
            foreach(FireEvent fe in events)
            {
                if(fe.Id.SourceId == sourceId)
                {
                    results.Add(fe);
                }
            }

            return results;
        }
    }
}