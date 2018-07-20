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
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    return Filter.FireEventsFilter.UserFilter(DatabaseOperations.ActiveEvents.GetAllActiveFireEvents(), user).ToArray<FireEvent>();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventType">The EventType of the active FireEvents</param>
        /// <returns>returns a list of all active FireEvents of the given EventType</returns>
        [HttpGet, Route("type/{eventType}")]
        public FireEvent[] Active(EventTypes eventType)
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    return Filter.FireEventsFilter.UserFilter(DatabaseOperations.ActiveEvents.GetActiveFireEventsByEventType(eventType), user).ToArray<FireEvent>();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceId">the sourceId of the active FireEvents you want to look for</param>
        /// <returns>a list of active FireEvents with a matching sourceId</returns>
        [HttpGet, Route("source/{sourceId}")]   
        public FireEvent[] GetActiveFireEventsBySourceId(int sourceId)
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    return Filter.FireEventsFilter.UserFilter(DatabaseOperations.ActiveEvents.GetActiveFireEventsBySourceId(sourceId), user).ToArray<FireEvent>();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// returns a distinct activeFireEvent
        /// </summary>
        /// <param name="sourceId">the sourceId of the active FireEvent you are looking for</param>
        /// <param name="targetId">the targetId of the active FireEvent you are looking for</param>
        /// <returns>returns a FireEvent with a matching sourceId and targetId</returns>
        [HttpGet, Route("id/{sourceId}/{targetId}")]
        public FireEvent[] GetActiveFireEventById(int sourceId, string targetId)
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    return Filter.FireEventsFilter.UserFilter(DatabaseOperations.ActiveEvents.GetActiveFireEventById(sourceId, targetId), user).ToArray<FireEvent>();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceId">the sourceId of the active FireEvents you want to look for</param>
        /// <param name="eventType">The EventType of the active FireEvents</param>
        /// <returns>returns a list of all active FireEvents with a matching sourceId an of the given 
        /// EventType</returns>
        [HttpGet, Route("type/{sourceId}/{eventType}")]
        public FireEvent[] GetActiveFireEventsBySourceIdEventType(int sourceId, EventTypes eventType)
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    return Filter.FireEventsFilter.UserFilter(DatabaseOperations.ActiveEvents.GetActiveFireEventsBySourceIdEventType(sourceId, eventType), user).ToArray<FireEvent>();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <returns>returns all active FireEvents from the given sourceId at the given date</returns>
        [HttpGet, Route("date/{sourceId}/{year}/{month}/{day}")]
        public FireEvent[] GetActiveFireEventsBySourceIdDate(int sourceId, int year, int month, int day)
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    return Filter.FireEventsFilter.UserFilter(DatabaseOperations.ActiveEvents.GetActiveFireEventsBySourceIdDate(sourceId, year, month, day), user).ToArray<FireEvent>();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <returns>returns all active FireEvents from the given sourceId in the given month and year</returns>
        [HttpGet, Route("date/{sourceId}/{year}/{month}")]
        public FireEvent[] GetActiveFireEventsBySourceIdDate(int sourceId, int year, int month)
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    return Filter.FireEventsFilter.UserFilter(DatabaseOperations.ActiveEvents.GetActiveFireEventsBySourceIdDate(sourceId, year, month), user).ToArray<FireEvent>();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <returns>returns all active FireEvents from the given sourceId in the given year</returns>
        [HttpGet, Route("date/{sourceId}/{year}")]
        public FireEvent[] GetActiveFireEventsBySourceIdDate(int sourceId, int year)
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    return Filter.FireEventsFilter.UserFilter(DatabaseOperations.ActiveEvents.GetActiveFireEventsBySourceIdDate(sourceId, year), user).ToArray<FireEvent>();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
