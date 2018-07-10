﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;
using System.Threading;

namespace FireApp.Service
{
    public static class DatabaseOperations
    {
        #region Queries
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
        /// 
        /// </summary>
        /// <returns>returns a list with all FireAlarmSystems from database</returns>
        public static IEnumerable<FireAlarmSystem> QueryFireAlarmSystems()
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
        /// <returns>returns a list with all FireBrigades from database</returns>
        public static IEnumerable<FireBrigade> QueryFireBrigades()
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
        /// <returns>returns a list with all ServiceMembers from database</returns>
        public static IEnumerable<ServiceMember> QueryServiceMembers()
        {
            using (var db = AppData.ServiceMemberDB())
            {
                var table = db.ServiceMemberTable();
                return table.FindAll();
            }
        }
        #endregion

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
        /// inserts a FireEvent into the database or updates it if it already exists
        /// </summary>
        /// <param name="fe">FireEvent that should be inserted into the Database</param>
        /// <returns>returns true if new object was inserted</returns>
        public static bool UpsertFireEvent(FireEvent fe)
        {
            UpsertActiveFireEvent(fe);
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
        /// <returns>returns a list of all Fireevents with a matching sourceId 
        /// (all Fireevents from a distinct fire alarm system)</returns>
        public static IEnumerable<FireEvent> GetFireEventsBySourceId(int sourceId)
        {
            List<FireEvent> events = LocalDatabase.GetAllFireEvents();
            if (events != null)
            {
                return (IEnumerable<FireEvent>)events.Find(x => x.Id.SourceId == sourceId);
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
        /// <param name="eventId">The ongoing number of the FireEvents of one
        /// FireAlarmSystem</param>
        /// <returns>returns a distinct FireEvent with a matching sourceId and eventId 
        /// (a FireEvent from a distinct fireAlarmSystem with the matching eventId)</returns>
        public static FireEvent GetFireEventById(int sourceId, int eventId)
        {
            List<FireEvent> events = (GetFireEventsBySourceId(sourceId)).ToList<FireEvent>();
            if (events != null)
            {
                return events.Find(x => x.Id.EventId == eventId);
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
        /// <param name="targetId">The id of the fire detector that sent the 
        /// FireEvent</param>
        /// <returns>returns a list of all FireEvents with matching sourceId and targetId</returns>
        public static IEnumerable<FireEvent> GetFireEventsBySourceIdTargetId(int sourceId, string targetId)
        {
            List<FireEvent> events = (GetFireEventsBySourceId(sourceId)).ToList<FireEvent>();
            if (events != null)
            {
                return (IEnumerable<FireEvent>)events.Find(x => x.TargetId == targetId);
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
        /// <param name="eventType">The EventType of the FireEvent</param>
        /// <returns>returns a list of all FireEvents with matching sourceId and eventType</returns>
        public static IEnumerable<FireEvent> GetFireEventsBySourceIdEventType(int sourceId, EventTypes eventType)
        {
            List<FireEvent> events = (GetFireEventsBySourceId(sourceId)).ToList<FireEvent>();

            if (events != null)
            {
                return (IEnumerable<FireEvent>)events.Find(x => x.EventType == eventType);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventType">The EventType of the FireEvents</param>
        /// <returns>returns a list of all FireEvents with matching eventType</returns>
        public static IEnumerable<FireEvent> GetFireEventsByEventType(EventTypes eventType)
        {
            List<FireEvent> events = LocalDatabase.GetAllFireEvents();

            if (events != null)
            {
                return (IEnumerable<FireEvent>)events.Find(x => x.EventType == eventType);
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
        /// <param name="startTime">The minimal value of the TimeStamp of the FireEvents</param>
        /// <param name="endTime">The maximal value of the TimeStamp of the FireEvents</param>
        /// <returns>returns a list of all FireEvents with matching sourceId and and a Timestamp between 
        /// startTime and endTime</returns>
        public static IEnumerable<FireEvent> GetFireEventsBySourceIdTimespan(int sourceId, long startTime, long endTime)
        {
            List<FireEvent> events = (GetFireEventsBySourceId(sourceId)).ToList<FireEvent>();

            if (events != null)
            {
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
            else
            {
                return null;
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
            List<FireEvent> events = LocalDatabase.GetAllFireEvents();
            List<FireEvent> result = new List<FireEvent>();
            if (events != null)
            {
                foreach (FireEvent fe in events)
                {
                    if (fe.TimeStamp >= new DateTime(startTime) && fe.TimeStamp <= new DateTime(endTime))
                    {
                        result.Add(fe);
                    }
                }

                return result;
            }
            else
            {
                return null;
            }
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
        public static bool UpsertActiveFireEvent(FireEvent fe)
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

        #endregion

        #region FireAlarmSystems
        /// <summary>
        /// inserts a FireAlarmSystem into the database or updates it if it already exists
        /// </summary>
        /// <param name="fas">The FireAlarmSystem you want to insert</param>
        /// <returns>returns true if FireAlarmSystem was inserted</returns>
        public static bool UpsertFireAlarmSystem(FireAlarmSystem fas)
        {
            LocalDatabase.UpsertFireAlarmSystem(fas);

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
            return (IEnumerable<FireAlarmSystem>)LocalDatabase.GetAllFireAlarmSystems();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">The id of the FireAlarmSystem you are looking for</param>
        /// <returns>returns a FireAlarmSystem with a matching id</returns>
        public static FireAlarmSystem GetFireAlarmSystemById(int id)
        {
            List<FireAlarmSystem> fireAlarmSystems = LocalDatabase.GetAllFireAlarmSystems();
            if(fireAlarmSystems != null)
            {
                return fireAlarmSystems.Find(x => x.Id == id);
            }
            else
            {
                return null;
            }       
        }     

        /// <summary>
        /// Adds a FireBrigade to the list of FireBrigades of a FireAlarmSystem
        /// </summary>
        /// <param name="id">identifier of the FireAlarmSystem</param>
        /// <param name="firebrigade">identifier of the firebrigade</param>
        /// <returns>returns true if the FireBrigade was added</returns>
        public static bool AddFireBrigadeToFireAlarmSystem(int id, int firebrigade)
        {
            bool rv = false;
            FireAlarmSystem fas = DatabaseOperations.GetFireAlarmSystemById(id);
            if(fas != null)
            {
                FireBrigade fb = DatabaseOperations.GetFireBrigadeById(firebrigade);
                if(fb != null)
                {
                    fas.FireBrigades.Add(firebrigade);
                    rv = UpsertFireAlarmSystem(fas);                 
                }
            }

            return rv;
        }

        /// <summary>
        /// Adds a ServiceMember to the list of ServiceMembers of a FireAlarmSystem
        /// </summary>
        /// <param name="id">identifier of the FireAlarmSystem</param>
        /// <param name="firebrigade">identifier of the ServiceMember</param>
        /// <returns>returns true if the ServiceMember was added</returns>
        public static bool AddServiceMemberToFireAlarmSystem(int id, int serviceMember)
        {
            bool rv = false;
            FireAlarmSystem fas = DatabaseOperations.GetFireAlarmSystemById(id);
            if (fas != null)
            {
                ServiceMember sm = DatabaseOperations.GetServiceMemberById(serviceMember);
                if (sm != null)
                {
                    fas.ServiceMembers.Add(serviceMember);
                    rv = UpsertFireAlarmSystem(fas);
                }
            }

            return rv;
        }
        #endregion

        #region FireBrigades
        /// <summary>
        /// inserts a FireBrigade into the database or updates it if it already exists
        /// </summary>
        /// <param name="fb"></param>
        /// <returns>returns true if the insert was successful</returns>
        public static bool UpsertFireBrigade(FireBrigade fb)
        {
            LocalDatabase.UpsertFireBrigade(fb);

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
            return (IEnumerable<FireBrigade>)LocalDatabase.GetAllFireBrigades();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">The id of the FireBrigade you are looking for</param>
        /// <returns>returns a FireBrigade with a matching id</returns>
        public static FireBrigade GetFireBrigadeById(int id)
        {
            List<FireBrigade> fireBrigades = LocalDatabase.GetAllFireBrigades();
            if(fireBrigades != null)
            {
                return fireBrigades.Find(x => x.Id == id);
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region ServiceMembers
        /// <summary>
        /// inserts a ServiceMember into the database or updates it if it already exists
        /// </summary>
        /// <param name="sm">The ServiceMember you want to insert</param>
        /// <returns>returns true if ServiceMember was inserted</returns>
        public static bool UpsertServiceMember(ServiceMember sm)
        {
            LocalDatabase.UpsertServiceMember(sm);

            using (var db = AppData.ServiceMemberDB())
            {
                var table = db.ServiceMemberTable();
                return table.Upsert(sm);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns a list with all ServiceMembers</returns>
        public static IEnumerable<ServiceMember> GetAllServiceMembers()
        {
            return (IEnumerable<ServiceMember>)LocalDatabase.GetAllServiceMembers();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">The id of the ServiceMember you are looking for</param>
        /// <returns>returns a ServiceMember with a matching id</returns>
        public static ServiceMember GetServiceMemberById(int id)
        {
            List<ServiceMember> serviceMembers = LocalDatabase.GetAllServiceMembers();
            if (serviceMembers != null)
            {
                return serviceMembers.Find(x => x.Id == id);
            }
            else
            {
                return null;
            }
        }
        #endregion    
    }
}