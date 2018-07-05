﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;

namespace FireApp.Service
{
    public static class DatabaseOperations
    {
        #region FireEvents
        /*******************************************************************************************
        * public static IEnumerable<FireEvent> All()
        * 
        * returns a list with all Fireevents
        ******************************************************************************************/
        public static IEnumerable<FireEvent> GetAllFireEvents()
        {
            using (var db = AppData.FireEventDB())
            {
                var table = db.FireEventTable();
                return table.FindAll();
            }
        }

        /*******************************************************************************************
         * public static bool UploadFireEvent(FireEvent fe)
         * 
         * inserts a FireEvent into the database or updates it if it already exists
         ******************************************************************************************/
        public static bool UploadFireEvent(FireEvent fe)
        {
            if (fe.EventType == EventTypes.prealarm ||
                fe.EventType == EventTypes.alarm ||
                fe.EventType == EventTypes.disfunction ||
                fe.EventType == EventTypes.outoforder)
            {
                using (var db = AppData.TargetDB())
                {
                    TargetState state;
                    switch (fe.EventType)
                    {
                        case (EventTypes.prealarm):  state = TargetState.prealarm; break;
                        case (EventTypes.alarm): state = TargetState.alarm; break;
                        case (EventTypes.outoforder): state = TargetState.outoforder; break;
                        default: state = TargetState.disfunction; break;
                    }

                    Target target = new Target(new TargetId(fe.Id.SourceId, fe.TargetId), state, fe.TimeStamp);
                    var table = db.TargetTable();
                    return table.Upsert(target);
                }
            } else {
                if (fe.EventType == EventTypes.reset){
                    using (var db = AppData.TargetDB())
                    {
                        var table = db.TargetTable();
                        Target target = table.FindOne(x => x.Id.SourceId == fe.Id.SourceId && x.Id.Target == fe.TargetId);
                        if(target != null)
                        {
                            table.Delete(x => x.Id == target.Id);
                        }
                    }
                }
            }

            using (var db = AppData.FireEventDB())
            {
                var table = db.FireEventTable();
                return table.Upsert(fe);
            }

        }

        /*******************************************************************************************
         * public static FireEvent GetFireEventById(int sourceId, int eventId)
         * 
         * returns a distinct Fireevent with a matching sourceId and eventId
         * (a Fireevent from a distinct fire alarm system with the matching eventId)
         ******************************************************************************************/
        public static FireEvent GetFireEventById(int sourceId, int eventId)
        {
            using (var db = AppData.FireEventDB())
            {
                var table = db.FireEventTable();

                return table.FindOne(x => x.Id.SourceId == sourceId && x.Id.EventId == eventId);
            }
        }

        /*******************************************************************************************
         * public static IEnumerable<FireEvent> GetFireEventBySourceId(int sourceId)
         * 
         * returns a list of all Fireevents with a matching sourceId
         * (all Fireevents from a distinct fire alarm system)
         ******************************************************************************************/
        public static IEnumerable<FireEvent> GetFireEventsBySourceId(int sourceId)
        {
            using (var db = AppData.FireEventDB())
            {
                var table = db.FireEventTable();

                return table.Find(x => x.Id.SourceId == sourceId);
            }
        }

        /*******************************************************************************************
         * public static IEnumerable<FireEvent> GetFireEventsBySourceIdTargetId(int sourceId, string targetId)
         * 
         * returns a list of all FireEvents with matching sourceId and targetId
         ******************************************************************************************/
        public static IEnumerable<FireEvent> GetFireEventsBySourceIdTargetId(int sourceId, string targetId)
        {
            using (var db = AppData.FireEventDB())
            {
                var table = db.FireEventTable();

                return table.Find(x => x.Id.SourceId == sourceId && x.TargetId == targetId);
            }
        }

        /*******************************************************************************************
         * public static IEnumerable<FireEvent> GetFireEventsBySourceIdTargetIdTimeStamp(
         * int sourceId, string targetId, DateTime timeStamp)
         * 
         * returns a list of all FireEvents with matching sourceId, targetId and timeStamp
         ******************************************************************************************/
        public static IEnumerable<FireEvent> GetFireEventsBySourceIdTargetIdTimeStamp
            (int sourceId, string targetId, DateTime timeStamp)
        {
            using (var db = AppData.FireEventDB())
            {
                var table = db.FireEventTable();

                return table.Find(x => x.Id.SourceId == sourceId && x.TargetId == targetId && x.TimeStamp == timeStamp);
            }
        }

        /*******************************************************************************************
         * public static IEnumerable<FireEvent> GetFireEventsBySourceIdEventType(int sourceId, EventTypes eventType)
         * 
         * returns a list of all FireEvents with matching sourceId and eventType
         ******************************************************************************************/
        public static IEnumerable<FireEvent> GetFireEventsBySourceIdEventType(int sourceId, EventTypes eventType)
        {
            using (var db = AppData.FireEventDB())
            {
                var table = db.FireEventTable();

                return table.Find(x => x.Id.SourceId == sourceId && x.EventType == eventType);
            }
        }

        /*******************************************************************************************
         * public static IEnumerable<FireEvent> GetFireEventsByEventType(EventTypes eventType)
         * 
         * returns a list of all FireEvents with matching eventType
         ******************************************************************************************/
        public static IEnumerable<FireEvent> GetFireEventsByEventType(EventTypes eventType)
        {
            using (var db = AppData.FireEventDB())
            {
                var table = db.FireEventTable();

                return table.Find(x => x.EventType == eventType);
            }
        }

        /*******************************************************************************************
         * public static IEnumerable<FireEvent> GetFireEventsBySourceIdTimespan(int sourceId, 
         * DateTime startTime, DateTime endTime)
         * 
         * returns a list of all FireEvents with matching sourceId and and a Timestamp between 
         * startTime and endTime
         ******************************************************************************************/
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

        /*******************************************************************************************
         * public static IEnumerable<FireEvent> GetFireEventsByTimespan(long startTime, long endTime)
         * 
         * returns a list of all FireEvents with a Timestamp between 
         * startTime and endTime
         ******************************************************************************************/
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

        #endregion

        #region FireAlarmSystems
        /*******************************************************************************************
         * public static bool UploadFireAlarmSystem(FireAlarmSystem fas)
         * 
         * inserts a FireAlarmSystem into the database or updates it if it already exists
         ******************************************************************************************/
        public static bool UploadFireAlarmSystem(FireAlarmSystem fas)
        {
            using (var db = AppData.FireAlarmSystemDB())
            {
                var table = db.FireAlarmSystemTable();
                return table.Upsert(fas);
            }
        }

        /*******************************************************************************************
         * public static IEnumerable<FireAlarmSystem> GetAllFireAlarmSystems()
         * 
         * returns a list with all FireAlarmSystems
         ******************************************************************************************/
        public static IEnumerable<FireAlarmSystem> GetAllFireAlarmSystems()
        {
            using (var db = AppData.FireAlarmSystemDB())
            {
                var table = db.FireAlarmSystemTable();
                return table.FindAll();
            }
        }

        /*******************************************************************************************
         * public static IEnumerable<FireAlarmSystem> GetFireAlarmSystemById(int id)
         * 
         * return a FireAlarmSystem with a matching id
         ******************************************************************************************/
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
        /*******************************************************************************************
         * public static bool UploadFireBrigade(FireBrigade fb)
         * 
         * inserts a FireBrigade into the database or updates it if it already exists
         ******************************************************************************************/
        public static bool UploadFireBrigade(FireBrigade fb)
        {
            using (var db = AppData.FireBrigadeDB())
            {
                var table = db.FireBrigadeTable();
                return table.Upsert(fb);
            }
        }

        /*******************************************************************************************
         * public static IEnumerable<FireBrigade> GetAllFireBrigades()
         * 
         * returns a list with all FireBrigades
         ******************************************************************************************/
        public static IEnumerable<FireBrigade> GetAllFireBrigades()
        {
            using (var db = AppData.FireBrigadeDB())
            {
                var table = db.FireBrigadeTable();
                return table.FindAll();
            }
        }

        /*******************************************************************************************
         * public static FireBrigade GetFireBrigadeById(int id)
         * 
         * return a FireBrigade with a matching id
         ******************************************************************************************/
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
        /*******************************************************************************************
         * public static IEnumerable<FireEvent> GetAllActiveFireEvents(TargetState state)
         * 
         * returns a list with all active FireEvents with a matching TargetState
         ******************************************************************************************/
        public static IEnumerable<FireEvent> GetAllActiveFireEvents(TargetState state)
        {
            using (var db = AppData.TargetDB())
            {
                var table = db.TargetTable();
                List<Target> targets = table.FindAll().ToList<Target>();
                List<FireEvent> fireEvents = new List<FireEvent>();

                foreach(Target t in targets)
                {
                    if (t.State == state)
                    {
                        fireEvents.AddRange(GetFireEventsBySourceIdTargetIdTimeStamp(t.Id.SourceId, t.Id.Target, t.TimeStamp));
                    }
                }

                return fireEvents;
            }
        }

        #endregion
    }
}