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
        [HttpGet, Route("all")] //todo: comment
        public FireEvent[] Active()
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    IEnumerable<FireEvent> events;
                    events = DatabaseOperations.ActiveEvents.GetAllActiveFireEvents();
                    events = Filter.FireEventsFilter.UserFilter(events, user);
                    return events.ToArray();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new FireEvent[0];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventType">The EventType of the active FireEvents</param>
        /// <returns>returns a list of all active FireEvents of the given EventType</returns>
        [HttpGet, Route("type/{eventType}")] //todo: comment
        public FireEvent[] Active(EventTypes eventType)
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    IEnumerable<FireEvent> events;
                    events = DatabaseOperations.ActiveEvents.GetActiveFireEventsByEventType(eventType);
                    events = Filter.FireEventsFilter.UserFilter(events, user);
                    return events.ToArray();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new FireEvent[0];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceId">the sourceId of the active FireEvents you want to look for</param>
        /// <returns>a list of active FireEvents with a matching sourceId</returns>
        [HttpGet, Route("source/{sourceId}")]   //todo: comment
        public FireEvent[] GetActiveFireEventsBySourceId(int sourceId)
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    IEnumerable<FireEvent> events;
                    events = DatabaseOperations.ActiveEvents.GetActiveFireEventsBySourceId(sourceId);
                    events = Filter.FireEventsFilter.UserFilter(events, user);
                    return events.ToArray();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new FireEvent[0];
            }
        }

        /// <summary>
        /// returns a distinct activeFireEvent
        /// </summary>
        /// <param name="sourceId">the sourceId of the active FireEvent you are looking for</param>
        /// <param name="targetId">the targetId of the active FireEvent you are looking for</param>
        /// <returns>returns a FireEvent with a matching sourceId and targetId</returns>
        [HttpGet, Route("id/{sourceId}/{targetId}")] //todo: comment
        public FireEvent[] GetActiveFireEventById(int sourceId, string targetId)
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    IEnumerable<FireEvent> events;
                    events = DatabaseOperations.ActiveEvents.GetActiveFireEventById(sourceId, targetId);
                    return Filter.FireEventsFilter.UserFilter(events, user).ToArray();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new FireEvent[0];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceId">the sourceId of the active FireEvents you want to look for</param>
        /// <param name="eventType">The EventType of the active FireEvents</param>
        /// <returns>returns a list of all active FireEvents with a matching sourceId an of the given 
        /// EventType</returns>
        [HttpGet, Route("type/{sourceId}/{eventType}")]//todo: comment
        public FireEvent[] GetActiveFireEventsBySourceIdEventType(int sourceId, EventTypes eventType)
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    IEnumerable<FireEvent> events;
                    events = DatabaseOperations.ActiveEvents.GetActiveFireEventsBySourceIdEventType(sourceId, eventType);
                    events = Filter.FireEventsFilter.UserFilter(events, user);
                    return events.ToArray();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new FireEvent[0];
            }
        }

        /// <returns>returns all active FireEvents from the given sourceId at the given date</returns>
        [HttpGet, Route("date/{sourceId}/{year}/{month}/{day}")]//todo: comment
        public FireEvent[] GetActiveFireEventsBySourceIdDate(int sourceId, int year, int month, int day)
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    IEnumerable<FireEvent> events;
                    events = DatabaseOperations.ActiveEvents.GetActiveFireEventsBySourceIdDate(sourceId, year, month, day);
                    events = Filter.FireEventsFilter.UserFilter(events, user);
                    return events.ToArray();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new FireEvent[0];
            }
        }

        /// <returns>returns all active FireEvents from the given sourceId in the given month and year</returns>
        [HttpGet, Route("date/{sourceId}/{year}/{month}")]//todo: comment
        public FireEvent[] GetActiveFireEventsBySourceIdDate(int sourceId, int year, int month)
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    IEnumerable<FireEvent> events;
                    events = DatabaseOperations.ActiveEvents.GetActiveFireEventsBySourceIdDate(sourceId, year, month);
                    events = Filter.FireEventsFilter.UserFilter(events, user);
                    return events.ToArray();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new FireEvent[0];
            }
        }

        /// <returns>returns all active FireEvents from the given sourceId in the given year</returns>
        [HttpGet, Route("date/{sourceId}/{year}")]//todo: comment
        public FireEvent[] GetActiveFireEventsBySourceIdDate(int sourceId, int year)
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    IEnumerable<FireEvent> events;
                    events = DatabaseOperations.ActiveEvents.GetActiveFireEventsBySourceIdDate(sourceId, year);
                    events = Filter.FireEventsFilter.UserFilter(events, user);
                    return events.ToArray();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new FireEvent[0];
            }
        }
    }
}
