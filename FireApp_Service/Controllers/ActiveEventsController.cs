using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FireApp.Domain;

namespace FireApp.Service.Controllers
{
    [RoutePrefix("active")]
    public class ActiveEventsController : ApiController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns a list of all active FireEvents</returns>
        [HttpGet, Route("all")] //todo: Access only for admin
        public FireEvent[] Active()
        {
            return (DatabaseOperations.ActiveEvents.GetAllActiveFireEvents()).ToArray<FireEvent>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventType">The EventType of the active FireEvents</param>
        /// <returns>returns a list of all active FireEvents of the given EventType</returns>
        [HttpGet, Route("type/{eventType}")]    //todo: Access only for admin
        public FireEvent[] Active(EventTypes eventType)
        {
            return (DatabaseOperations.ActiveEvents.GetActiveFireEventsByEventType(eventType)).ToArray<FireEvent>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceId">the sourceId of the active FireEvents you want to look for</param>
        /// <returns>a list of active FireEvents with a matching sourceId</returns>
        [HttpGet, Route("source/{sourceId}")]   //todo: Access only for admin, FireAlarmSystem
        public FireEvent[] GetActiveFireEventsBySourceId(int sourceId)
        {
            return (DatabaseOperations.ActiveEvents.GetActiveFireEventsBySourceId(sourceId)).ToArray<FireEvent>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceId">the sourceId of the active FireEvents you want to look for</param>
        /// <param name="eventType">The EventType of the active FireEvents</param>
        /// <returns>returns a list of all active FireEvents with a matching sourceId an of the given 
        /// EventType</returns>
        [HttpGet, Route("type/{sourceId}/{eventType}")] //todo: Access only for admin, FireAlarmSystem, restricted access for fb and sm
        public FireEvent[] GetActiveFireEventsBySourceIdEventType(int sourceId, EventTypes eventType)
        {
            return (DatabaseOperations.ActiveEvents.GetActiveFireEventsBySourceIdEventType(sourceId, eventType)).ToArray<FireEvent>();
        }

        /// <returns>returns all active FireEvents from the given sourceId at the given date</returns>
        [HttpGet, Route("date/{sourceId}/{year}/{month}/{day}")]    //todo: Access only for admin, FireAlarmSystem
        public FireEvent[] GetActiveFireEventsBySourceIdDate(int sourceId, int year, int month, int day)
        {
            return (DatabaseOperations.ActiveEvents.GetActiveFireEventsBySourceIdDate(sourceId, year, month, day)).ToArray<FireEvent>();
        }

        /// <returns>returns all active FireEvents from the given sourceId in the given month and year</returns>
        [HttpGet, Route("date/{sourceId}/{year}/{month}")]  //todo: Access only for admin, FireAlarmSystem
        public FireEvent[] GetActiveFireEventsBySourceIdDate(int sourceId, int year, int month)
        {
            return (DatabaseOperations.ActiveEvents.GetActiveFireEventsBySourceIdDate(sourceId, year, month)).ToArray<FireEvent>();
        }

        /// <returns>returns all active FireEvents from the given sourceId in the given year</returns>
        [HttpGet, Route("date/{sourceId}/{year}")]  //todo: Access only for admin, FireAlarmSystem
        public FireEvent[] GetActiveFireEventsBySourceIdDate(int sourceId, int year)
        {
            return (DatabaseOperations.ActiveEvents.GetActiveFireEventsBySourceIdDate(sourceId, year)).ToArray<FireEvent>();
        }
    }
}
