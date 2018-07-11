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
        [HttpGet, Route("all")]
        public FireEvent[] Active()
        {
            return (DatabaseOperations.ActiveEvents.GetAllActiveFireEvents()).ToArray<FireEvent>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetState">The TargetState of the active FireEvents</param>
        /// <returns>returns a list of all active FireEvents of the given 
        /// TargetState</returns>
        [HttpGet, Route("type/{eventType}")]
        public FireEvent[] Active(EventTypes eventType)
        {
            return (DatabaseOperations.ActiveEvents.GetActiveFireEventsByEventType(eventType)).ToArray<FireEvent>();
        }

        
    }
}
