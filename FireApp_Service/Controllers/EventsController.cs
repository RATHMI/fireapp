﻿using FireApp.Domain;
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

        /// <summary>
        /// inserts a FireEvent into the database or updates it if it already exists
        /// </summary>
        /// <param name="fe">FireEvent that should be inserted into the Database</param>
        /// <returns>returns true if new object was inserted</returns>
        [HttpPost, Route("upload")]
        public bool UploadFireEvent([FromBody] FireEvent fe)
        {
            return DatabaseOperations.UploadFireEvent(fe);
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
        [HttpGet, Route("id/{sourceId}/{eventId}")]
        public FireEvent GetFireEventById(int sourceId, int eventId)
        {
            return DatabaseOperations.GetFireEventById(sourceId, eventId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceId">The sourceId of the FireAlarmSystem that sent 
        /// the FireEvent</param>
        /// <returns>returns a list of all Fireevents with a matching sourceId 
        /// (all Fireevents from a distinct fire alarm system)</returns>
        [HttpGet, Route("sid/{sourceId}")]
        public IEnumerable<FireEvent> GetFireEventsBySourceId(int sourceId)
        {
            return DatabaseOperations.GetFireEventsBySourceId(sourceId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceId">The sourceId of the FireAlarmSystem that sent 
        /// the FireEvent</param>
        /// <param name="targetId">The id of the fire detector that sent the 
        /// FireEvent</param>
        /// <returns>returns a list of all FireEvents with matching sourceId and targetId</returns>
        [HttpGet, Route("stid/{sourceId}/{targetId}")]
        public IEnumerable<FireEvent> GetFireEventsBySourceIdTargetId(int sourceId, string targetId)
        {
            return DatabaseOperations.GetFireEventsBySourceIdTargetId(sourceId, targetId);
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
        [HttpGet, Route("stidt/{sourceId}/{targetId}/{timeStamp}")]
        public IEnumerable<FireEvent> GetFireEventsBySourceIdTargetIdTimeStamp
           (int sourceId, string targetId, long timeStamp)
        {
            return DatabaseOperations.GetFireEventsBySourceIdTargetIdTimeStamp(sourceId, targetId, timeStamp);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceId">The sourceId of the FireAlarmSystem that sent 
        /// the FireEvent</param>
        /// <param name="eventType">The EventType of the FireEvent</param>
        /// <returns>returns a list of all FireEvents with matching sourceId and eventType</returns>
        [HttpGet, Route("et/{sourceId}/{eventType}")]
        public IEnumerable<FireEvent> GetFireEventsBySourceIdEventType(int sourceId, EventTypes eventType)
        {
            return DatabaseOperations.GetFireEventsBySourceIdEventType(sourceId, eventType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventType">The EventType of the FireEvents</param>
        /// <returns>returns a list of all FireEvents with matching eventType</returns>
        [HttpGet, Route("et/{eventType}")]
        public IEnumerable<FireEvent> GetFireEventsByEventType(EventTypes eventType)
        {
            return DatabaseOperations.GetFireEventsByEventType(eventType);
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
        [HttpGet, Route("time/{sourceId}/{startTime}/{endTime}")]
        public IEnumerable<FireEvent> GetFireEventsBySourceIdTimespan(int sourceId, long startTime, long endTime)
        {
            return DatabaseOperations.GetFireEventsBySourceIdTimespan(sourceId, startTime, endTime);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startTime">The minimal value of the TimeStamp of the FireEvents</param>
        /// <param name="endTime">The maximal value of the TimeStamp of the FireEvents</param>
        /// <returns>returns a list of all FireEvents with a Timestamp between 
        /// startTime and endTime</returns>
        [HttpGet, Route("time/{startTime}/{endTime}")]
        public IEnumerable<FireEvent> GetFireEventsByTimespan(long startTime, long endTime)
        {
            return DatabaseOperations.GetFireEventsByTimespan(startTime, endTime);
        }

         /// <summary>
         /// 
         /// </summary>
         /// <returns>returns all FireEvents</returns>
        [HttpGet, Route("all")]
        public IEnumerable<FireEvent> All()
        {
            return DatabaseOperations.GetAllFireEvents();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventType">The EventType of the FireEvents</param>
        /// <param name="year">The year of the FireEvents' TimeStamp</param>
        /// <returns>returns an array with the number of FireEvents of the given EventType 
        /// that occured in the given year. Each column represents one month</returns>
        [HttpGet, Route("typeyear/{eventType}/{year}")]
        public Int32[] CountFireEventsByEventTypePerYear(EventTypes eventType, int year)
        {
            return DatabaseOperations.CountFireEventsByEventTypePerYear(eventType, year);
        }

         /// <summary>
         /// 
         /// </summary>
         /// <param name="targetState">The TargetState of the active FireEvents</param>
         /// <returns>returns a list of all active FireEvents of the given 
         /// TargetState</returns>
        [HttpGet, Route("active/{eventType}")]
        public IEnumerable<FireEvent> Active([FromBody]EventTypes eventType)
        {
            return DatabaseOperations.GetAllActiveFireEvents(eventType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns a list of all active FireEvents</returns>
        [HttpGet, Route("active")]
        public IEnumerable<FireEvent> Active()
        {
            return DatabaseOperations.GetAllActiveFireEvents();
        }

    }
}
