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
        public bool UploadFireEvent([FromBody] FireEvent fe)
        {
            return DatabaseOperations.UploadFireEvent(fe);         
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
            return DatabaseOperations.GetFireEventById(sourceId, eventId);
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
            return DatabaseOperations.GetFireEventsBySourceId(sourceId);
        }

        /*******************************************************************************************
         * public IEnumerable<FireEvent> GetFireEventsBySourceIdTargetId(int sourceId, string targetId)
         * 
         * returns a list of all FireEvents with matching sourceId and targetId
         ******************************************************************************************/
        [HttpGet, Route("stid/{sourceId}/{targetId}")]
        public IEnumerable<FireEvent> GetFireEventsBySourceIdTargetId(int sourceId, string targetId)
        {
            return DatabaseOperations.GetFireEventsBySourceIdTargetId(sourceId, targetId);
        }

        /*******************************************************************************************
         * public IEnumerable<FireEvent> GetFireEventsBySourceIdEventType(int sourceId, EventTypes eventType)
         * 
         * returns a list of all FireEvents with matching sourceId and eventType
         ******************************************************************************************/
        [HttpGet, Route("et/{sourceId}/{eventType}")]
        public IEnumerable<FireEvent> GetFireEventsBySourceIdEventType(int sourceId, EventTypes eventType)
        {
            return DatabaseOperations.GetFireEventsBySourceIdEventType(sourceId, eventType);
        }

        /*******************************************************************************************
         * public IEnumerable<FireEvent> GetFireEventsByEventType(EventTypes eventType)
         * 
         * returns a list of all FireEvents with matching eventType
         ******************************************************************************************/
        [HttpGet, Route("et/{eventType}")]
        public IEnumerable<FireEvent> GetFireEventsByEventType(EventTypes eventType)
        {
            return DatabaseOperations.GetFireEventsByEventType(eventType);
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
            return DatabaseOperations.GetFireEventsBySourceIdTimespan(sourceId, startTime, endTime);
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
            return DatabaseOperations.GetFireEventsByTimespan(startTime, endTime);
        }

        /*******************************************************************************************
         * public IEnumerable<FireEvent> All()
         * 
         * returns a list with all Fireevents
         ******************************************************************************************/
        [HttpGet, Route("all")]
        public IEnumerable<FireEvent> All()
        {
            return DatabaseOperations.GetAllFireEvents();
        }

        /*******************************************************************************************
         * public IEnumerable<FireEvent> Active([FromBody] TargetState targetState)
         * 
         * returns a list with all active FireEvents with a matching TargetState
         ******************************************************************************************/
        [HttpGet, Route("active/{targetState}")]
        public IEnumerable<FireEvent> Active([FromBody] TargetState targetState)
        {
            return DatabaseOperations.GetAllActiveFireEvents(targetState);
        }


    }
}
