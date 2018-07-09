using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;
using System.Threading;

namespace FireApp.Service
{
    public static class DatabaseOperations
    {
        #region FireEvents
        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns all FireEvents from local Database</returns>
        public static IEnumerable<FireEvent> GetAllFireEvents()
        {
            return (IEnumerable<FireEvent>)LocalDatabase.GetAllFireEvents();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns all FireEvents from database</returns>
        public static IEnumerable<FireEvent> QueryFireEvents()
        {
            using (var db = AppData.FireEventDB())
            {
                var table = db.FireEventTable();
                return table.FindAll();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns all active FireEvents from database</returns>
        public static IEnumerable<FireEvent> QueryActiveFireEvents()
        {
            using (var db = AppData.ActiveFireEventDB())
            {
                var table = db.ActiveFireEventTable();
                return table.FindAll();
            }
        }

        /// <summary>
        /// inserts a FireEvent into the database or updates it if it already exists
        /// </summary>
        /// <param name="fe">FireEvent that should be inserted into the Database</param>
        /// <returns>returns true if new object was inserted</returns>
        public static bool UploadFireEvent(FireEvent fe)
        {
            UploadActiveFireEvent(fe);
            LocalDatabase.UpsertFireEvent(fe);

            using (var db = AppData.FireEventDB())
            {
                var table = db.FireEventTable();
                return table.Upsert(fe);
            }

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
        public static FireEvent GetFireEventById(int sourceId, int eventId)
        {
            List<FireEvent> events = LocalDatabase.GetAllFireEvents();
            if (events != null)
            {
                return events.Find(x => x.Id.SourceId == sourceId && x.Id.EventId == eventId);
            }
            else
            {
                return null;
            }     
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
            using (var db = AppData.FireEventDB())
            {
                var table = db.FireEventTable();

                return table.Find(x => x.Id.SourceId == sourceId);
            }
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
            using (var db = AppData.FireEventDB())
            {
                var table = db.FireEventTable();

                return table.Find(x => x.Id.SourceId == sourceId && x.TargetId == targetId);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceId">The sourceId of the FireAlarmSystem that sent 
        /// the FireEvent</param>
        /// <param name="targetId">The id of the fire detector that sent the 
        /// FireEvent</param>
        /// <param name="timeStamp">The date and time that the FireEvent occured</param>
        /// <returns>returns a list of all FireEvents with matching sourceId, targetId and 
        /// timeStamp</returns>
        public static IEnumerable<FireEvent> GetFireEventsBySourceIdTargetIdTimeStamp
            (int sourceId, string targetId, long timeStamp)
        {
            using (var db = AppData.FireEventDB())
            {
                var table = db.FireEventTable();

                var events = table.Find(x => x.Id.SourceId == sourceId && x.TargetId == targetId);
                List<FireEvent> results = new List<FireEvent>();
                foreach (FireEvent e in events)
                {
                    if(e.TimeStamp.Ticks == timeStamp)
                    {
                        results.Add(e);
                    }
                }
                return results;
            }
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
            using (var db = AppData.FireEventDB())
            {
                var table = db.FireEventTable();

                return table.Find(x => x.Id.SourceId == sourceId && x.EventType == eventType);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventType">The EventType of the FireEvents</param>
        /// <returns>returns a list of all FireEvents with matching eventType</returns>
        public static IEnumerable<FireEvent> GetFireEventsByEventType(EventTypes eventType)
        {
            using (var db = AppData.FireEventDB())
            {
                var table = db.FireEventTable();

                return table.Find(x => x.EventType == eventType);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceId">The sourceId of the FireAlarmSystem that sent 
        /// the FireEvent</param>
        /// <param name="startTime">The minimal value of the TimeStamp of the FireEvents</param>
        /// <param name="endTime">The maximal value of the TimeStamp of the FireEvents</param>
        /// <returns>returns a list of all FireEvents with matching sourceId and and a Timestamp between 
        /// startTime and endTime</returns>
        public static IEnumerable<FireEvent> GetFireEventsBySourceIdTimespan(int sourceId, long startTime, long endTime)
        {
            using (var db = AppData.FireEventDB())
            {
                var table = db.FireEventTable();

                var allEvents = table.Find(x => x.Id.SourceId == sourceId);
                List<FireEvent> result = new List<FireEvent>();

                foreach (FireEvent fe in allEvents)
                {
                    if (fe.TimeStamp >= new DateTime(startTime) && fe.TimeStamp <= new DateTime(endTime))
                    {
                        result.Add(fe);
                    }
                }

                return result;
            }
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
            using (var db = AppData.FireEventDB())
            {
                var table = db.FireEventTable();

                var allEvents = table.FindAll();
                List<FireEvent> result = new List<FireEvent>();

                foreach (FireEvent fe in allEvents)
                {
                    if (fe.TimeStamp >= new DateTime(startTime) && fe.TimeStamp <= new DateTime(endTime))
                    {
                        result.Add(fe);
                    }
                }

                return result;
            }
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

            foreach(FireEvent fe in events)
            {
                if(fe.TimeStamp.Year == year)
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

        #endregion

        #region FireAlarmSystems
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fas">The FireAlarmSystem you want to insert</param>
        /// <returns>inserts a FireAlarmSystem into the database or updates it if it already exists</returns>
        public static bool UploadFireAlarmSystem(FireAlarmSystem fas)
        {
            using (var db = AppData.FireAlarmSystemDB())
            {
                var table = db.FireAlarmSystemTable();
                return table.Upsert(fas);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns a list with all FireAlarmSystems</returns>
        public static IEnumerable<FireAlarmSystem> GetAllFireAlarmSystems()
        {
            using (var db = AppData.FireAlarmSystemDB())
            {
                var table = db.FireAlarmSystemTable();
                return table.FindAll();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">The id of the FireAlarmSystem you are looking for</param>
        /// <returns>returns a FireAlarmSystem with a matching id</returns>
        public static FireAlarmSystem GetFireAlarmSystemById(int id)
        {
            using (var db = AppData.FireAlarmSystemDB())
            {
                var table = db.FireAlarmSystemTable();

                return table.FindOne(x => x.Id == id);
            }
        }
        #endregion

        #region FireBrigades
        /// <summary>
        /// inserts a FireBrigade into the database or updates it if it already exists
        /// </summary>
        /// <param name="fb"></param>
        /// <returns>returns true if the insert was successful</returns>
        public static bool UploadFireBrigade(FireBrigade fb)
        {
            using (var db = AppData.FireBrigadeDB())
            {
                var table = db.FireBrigadeTable();
                return table.Upsert(fb);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns a list with all FireBrigades</returns>
        public static IEnumerable<FireBrigade> GetAllFireBrigades()
        {
            using (var db = AppData.FireBrigadeDB())
            {
                var table = db.FireBrigadeTable();
                return table.FindAll();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">The id of the FireBrigade you are looking for</param>
        /// <returns>returns a FireBrigade with a matching id</returns>
        public static FireBrigade GetFireBrigadeById(int id)
        {
            using (var db = AppData.FireBrigadeDB())
            {
                var table = db.FireBrigadeTable();

                return table.FindOne(x => x.Id == id);
            }
        }
        #endregion

        #region activeEvents
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventType">The EventType of the active FireEvents</param>
        /// <returns>returns a list of all active FireEvents of the given 
        /// TargetState</returns>
        public static IEnumerable<FireEvent> GetAllActiveFireEvents(EventTypes eventType)
        {
            List<FireEvent> events = LocalDatabase.GetActiveFireEvents();
            List<FireEvent> result = new List<FireEvent>();
            foreach(FireEvent fe in events)
            {
                if(fe.EventType == eventType)
                {
                    result.Add(fe);
                }
            }

            return (IEnumerable<FireEvent>)result;          
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
        /// <param name="fe">The FireEvent you want to create a target of</param>
        /// <returns>returns true if Target was inserted or deleted</returns>
        public static bool UploadActiveFireEvent(FireEvent fe)
        {
            if (fe.EventType == EventTypes.prealarm ||
                fe.EventType == EventTypes.alarm ||
                fe.EventType == EventTypes.disfunction ||
                fe.EventType == EventTypes.outoforder)
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
                            }else{
                                return false;
                            }
                        }
                    }
                }
            }
            return false;            
        }

        #endregion
    }
}