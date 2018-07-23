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
        /// Returns all active FireEvents that the user is allowed to see.
        /// If you want to filter the result use the headers of your request
        /// (see Filter.FireEventsFilter.HeadersFilter).
        /// </summary>
        /// <returns>Returns all active FireEvents that this user is allowed to see.</returns>
        [HttpGet, Route("all")]
        public FireEvent[] GetAll()
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    IEnumerable<FireEvent> events;

                    // Get all active FireEvents.
                    events = DatabaseOperations.ActiveEvents.GetAll();

                    // Filter the FireEvents according to the User.
                    events = Filter.FireEventsFilter.UserFilter(events, user);

                    // Filter FireEvents according to the headers the client sent.
                    events = Filter.FireEventsFilter.HeadersFilter(events, Request.Headers);

                    return events.ToArray();
                }
                else
                {
                    // Notify user that the login was not successful.
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
        /// <param name="eventType">The EventType of the active FireEvents.</param>
        /// <returns>Returns a list of all active FireEvents of the given EventType.</returns>
        [HttpGet, Route("type/{eventType}")] //todo: comment
        public FireEvent[] GetByEventType(EventTypes eventType)
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    IEnumerable<FireEvent> events;

                    // Get all active FireEvents with a matching eventType.
                    events = DatabaseOperations.ActiveEvents.GetByEventType(eventType);

                    // Filter the FireEvents according to the User.
                    events = Filter.FireEventsFilter.UserFilter(events, user);
                    return events.ToArray();
                }
                else
                {
                    // Notify user that the login was not successful.
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
        /// <param name="sourceId">The sourceId of the active FireEvents you want to look for.</param>
        /// <returns>Returns a list of active FireEvents with a matching sourceId.</returns>
        [HttpGet, Route("source/{sourceId}")]   //todo: comment
        public FireEvent[] GetBySourceId(int sourceId)
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    IEnumerable<FireEvent> events;

                    // Get all active FireEvents with a matching sourceId.
                    events = DatabaseOperations.ActiveEvents.GetBySourceId(sourceId);

                    // Filter the FireEvents according to the User.
                    events = Filter.FireEventsFilter.UserFilter(events, user);
                    return events.ToArray();
                }
                else
                {
                    // Notify user that the login was not successful.
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
        /// Returns a distinct active FireEvent.
        /// </summary>
        /// <param name="sourceId">The sourceId of the active FireEvent you are looking for.</param>
        /// <param name="targetId">The targetId of the active FireEvent you are looking for.</param>
        /// <returns>Returns a FireEvent with a matching sourceId and targetId.</returns>
        [HttpGet, Route("id/{sourceId}/{targetId}")] //todo: comment
        public FireEvent[] GetById(int sourceId, string targetId)
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    IEnumerable<FireEvent> events;

                    // Get all active FireEvents with a matching sourceId and targetId.
                    events = DatabaseOperations.ActiveEvents.GetByTarget(sourceId, targetId);

                    // Filter the FireEvents according to the User.
                    events = Filter.FireEventsFilter.UserFilter(events, user);
                    return events.ToArray();
                }
                else
                {
                    // Notify user that the login was not successful.
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
