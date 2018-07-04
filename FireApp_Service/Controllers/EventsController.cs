using FireApp.Domain;
using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FireApp.Service.Controllers
{
    [RoutePrefix("events")]
    public class EventsController : ApiController
    {

        /*******************************************************************************************
         * public bool DetailsByCompanyId([FromBody] FireEvent fe)
         * 
         * inserts a FireEvent into the database or updates it if it already exists
         ******************************************************************************************/
        [HttpPost, Route("upload")]
        public bool DetailsByCompanyId([FromBody] FireEvent fe)
        {
            using (var db = AppData.FireEventDB())
            {
                var table = db.FireEventTable();
                return table.Upsert(fe);
            }
        }

        /*******************************************************************************************
         * public FireEvent GetFireEventById(int sourceId, int eventId)
         * 
         * returns a distinct Fireevent with a matching sourceId and eventId
         * (a Fireevent from a distinct fire alarm system with the matching eventId)
         ******************************************************************************************/
        [HttpGet, Route("id/{sourceId}/{eventId}")]
        public FireEvent GetFireEventById(int sourceId, int eventId)
        {
            using (var db = AppData.FireEventDB())
            {
                var table = db.FireEventTable();

                return table.FindOne(x => x.Id.SourceId == sourceId && x.Id.EventId == eventId);
            }
        }

        /*******************************************************************************************
         * public IEnumerable<FireEvent> GetFireEventBySourceId(int sourceId)
         * 
         * returns a list of all Fireevents with a matching sourceId
         * (all Fireevents from a distinct fire alarm system)
         ******************************************************************************************/
        [HttpGet, Route("sid/{sourceId}")]
        public IEnumerable<FireEvent> GetFireEventsBySourceId(int sourceId)
        {
            using (var db = AppData.FireEventDB())
            {
                var table = db.FireEventTable();

                return table.Find(x => x.Id.SourceId == sourceId);
            }
        }

        /*******************************************************************************************
         * public IEnumerable<FireEvent> GetFireEventsBySourceIdTargetId(int sourceId, string targetId)
         * 
         * returns a list of all FireEvents with matching sourceId and targetId
         ******************************************************************************************/
        [HttpGet, Route("stid/{sourceId}/{targetId}")]
        public IEnumerable<FireEvent> GetFireEventsBySourceIdTargetId(int sourceId, string targetId)
        {
            using (var db = AppData.FireEventDB())
            {
                var table = db.FireEventTable();

                return table.Find(x => x.Id.SourceId == sourceId && x.TargetId == targetId);
            }
        }

        /*******************************************************************************************
         * public IEnumerable<FireEvent> GetFireEventsBySourceIdEventType(int sourceId, EventTypes eventType)
         * 
         * returns a list of all FireEvents with matching sourceId and eventType
         ******************************************************************************************/
        [HttpGet, Route("et/{sourceId}/{eventType}")]
        public IEnumerable<FireEvent> GetFireEventsBySourceIdEventType(int sourceId, EventTypes eventType)
        {
            using (var db = AppData.FireEventDB())
            {
                var table = db.FireEventTable();

                return table.Find(x => x.Id.SourceId == sourceId && x.EventType == eventType);
            }
        }

        /*******************************************************************************************
         * public IEnumerable<FireEvent> GetFireEventsByEventType(EventTypes eventType)
         * 
         * returns a list of all FireEvents with matching eventType
         ******************************************************************************************/
        [HttpGet, Route("et/{eventType}")]
        public IEnumerable<FireEvent> GetFireEventsByEventType(EventTypes eventType)
        {
            using (var db = AppData.FireEventDB())
            {
                var table = db.FireEventTable();

                return table.Find(x => x.EventType == eventType);
            }
        }

        /*******************************************************************************************
         * IEnumerable<FireEvent> GetFireEventsBySourceIdTimespan(int sourceId, DateTime startTime, DateTime endTime)
         * 
         * returns a list of all FireEvents with matching sourceId and and a Timestamp between 
         * startTime and endTime
         ******************************************************************************************/
        [HttpGet, Route("time/{sourceId}/{startTime}/{endTime}")]
        public IEnumerable<FireEvent> GetFireEventsBySourceIdTimespan(int sourceId, long startTime, long endTime)
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
         *  public IEnumerable<FireEvent> GetFireEventsByTimespan(long startTime, long endTime)
         * 
         * returns a list of all FireEvents with a Timestamp between 
         * startTime and endTime
         ******************************************************************************************/
        [HttpGet, Route("time/{startTime}/{endTime}")]
        public IEnumerable<FireEvent> GetFireEventsByTimespan(long startTime, long endTime)
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

        /*******************************************************************************************
         * public IEnumerable<FireEvent> All()
         * 
         * returns a list with all Fireevents
         ******************************************************************************************/
        [HttpGet, Route("all")]
        public IEnumerable<FireEvent> All()
        {
            using (var db = AppData.FireEventDB())
            {
                var table = db.FireEventTable();
                return table.FindAll();
            }
        }
    }
}
