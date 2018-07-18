using FireApp.Domain;
using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FireApp.Service.DatabaseOperations;
using Newtonsoft.Json.Converters;

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
        [HttpPost, Route("upload")]     //todo: access only for admin, actual fire alarm system
        public bool UploadFireEvent([FromBody] FireEvent fe)
        {
            return DatabaseOperations.Events.UpsertFireEvent(fe);
        }

        /// <summary>
        /// Checks if an id is already used by another FireEvent
        /// </summary>
        /// <param name="sourceId">the sourceId you want to check</param>
        /// <param name="eventId">the eventId you want to check</param>
        /// <returns>returns true if id is not used by other FireEvent</returns>
        [HttpPost, Route("checkid/{sourceId}/{eventId}")]     
        public bool CheckId(int sourceId, int eventId)
        {
            return DatabaseOperations.Events.CheckId(new FireEventId(sourceId, eventId));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns all FireEvents</returns>
        [HttpGet, Route("all")]     //todo: access only for admin
        public FireEvent[] All()
        {
            IEnumerable<User> user = Authentication.Token.VerifyToken(Authentication.Token.GetTokenFromHeader(Request.Headers));
            if (user != null)
            {
                List<FireEvent> results = new List<FireEvent>();
                try
                {
                    //todo: comment
                    DateTime date1 = DateTime.MaxValue;
                    DateTime date2 = DateTime.MaxValue;
                    List<EventTypes> eventTypes = new List<EventTypes>();
                    string[] types = null;
                    IEnumerable<FireEvent> events = Filter.FireEventsFilter.UserFilter((DatabaseOperations.Events.GetAllFireEvents()), user.First<User>()).ToArray<FireEvent>();
                    IEnumerable<string> key = null;
                    string[] datestring = null;

                    if (Request.Headers.TryGetValues("startDate", out key) != false)
                    {
                        Request.Headers.TryGetValues("startDate", out key);                      
                        try
                        {
                            datestring = key.First<string>().Trim(new char[] { '"' }).Split('-');
                            date1 = new DateTime(Convert.ToInt32(datestring[0]), Convert.ToInt32(datestring[1]), Convert.ToInt32(datestring[2]));
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }                                          
                    }
                    if (Request.Headers.TryGetValues("endDate", out key) != false)
                    {
                        Request.Headers.TryGetValues("endDate", out key);
                        try
                        {
                            datestring = key.First<string>().Trim(new char[] { '"' }).Split('-');
                            date2 = new DateTime(Convert.ToInt32(datestring[0]), Convert.ToInt32(datestring[1]), Convert.ToInt32(datestring[2]));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                    if (Request.Headers.TryGetValues("events", out key) != false)
                    {
                        Request.Headers.TryGetValues("events", out key);
                        types = (key.First<string>().Trim(new char[] {'"', ','})).Split(',');
                    }

                    Console.WriteLine(key); // to prevent optimisation
                    if (date1 <= DateTime.Today && date2 <= DateTime.Today)
                    {
                        results = Filter.FireEventsFilter.DateFilter(events, date1, date2).ToList<FireEvent>();
                    }
                    else
                    {
                        results = events.ToList<FireEvent>();
                    }

                    if(types != null)
                    {
                        if(types[0] != "all")
                        {
                            foreach(string s in types)
                            {
                                switch (s)
                                {
                                    case "Activation": eventTypes.Add(EventTypes.activation); break;
                                    case "Alarm": eventTypes.Add(EventTypes.alarm); break;
                                    case "Deactivated": eventTypes.Add(EventTypes.deactivated); break;
                                    case "Disfunction": eventTypes.Add(EventTypes.disfunction); break;
                                    case "Info": eventTypes.Add(EventTypes.info); break;
                                    case "Prelarm": eventTypes.Add(EventTypes.prealarm); break;
                                    case "Reset": eventTypes.Add(EventTypes.reset); break;
                                    case "Test": eventTypes.Add(EventTypes.test); break;
                                }
                            }
                            results = Filter.FireEventsFilter.EventTypeFilter(results, eventTypes.ToArray<EventTypes>()).ToList<FireEvent>();
                        }
                    }


                    return results.ToArray<FireEvent>();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return results.ToArray();
                }
                
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
        [HttpGet, Route("id/{sourceId}/{eventId}")]     //todo: access only for admin, fas, restricted access for fb, sg
        public FireEvent[] GetFireEventById(int sourceId, int eventId)
        {
            //User user = Authentication.Token.VerifyToken(Authentication.Token.GetTokenFromHeader(Request.Headers));
           // if (user != null)
            //{
                //List<FireEvent> events = DatabaseOperations.Events.GetFireEventById(sourceId, eventId).ToList<FireEvent>();
                //return Filter.FireEventsFilter.UserFilter(events, user).ToArray<FireEvent>();
            //}
            //return null;

            return DatabaseOperations.Events.GetFireEventById(sourceId, eventId).ToArray<FireEvent>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceId">The sourceId of the FireAlarmSystem that sent 
        /// the FireEvent</param>
        /// <returns>returns a list of all Fireevents with a matching sourceId 
        /// (all Fireevents from a distinct fire alarm system)</returns>
        [HttpGet, Route("source/{sourceId}")]   //todo: access only for admin, fas, restricted access for fb, sg
        public FireEvent[] GetFireEventsBySourceId(int sourceId)
        {
            return (DatabaseOperations.Events.GetFireEventsBySourceId(sourceId)).ToArray<FireEvent>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceId">The sourceId of the FireAlarmSystem that sent 
        /// the FireEvent</param>
        /// <param name="targetId">The id of the fire detector that sent the 
        /// FireEvent</param>
        /// <returns>returns a list of all FireEvents with matching sourceId and targetId</returns>
        [HttpGet, Route("target/{sourceId}/{targetId}")]    //todo: access only for admin, fas, restricted access for fb, sg
        public FireEvent[] GetFireEventsBySourceIdTargetId(int sourceId, string targetId)
        {
            return (DatabaseOperations.Events.GetFireEventsBySourceIdTargetId(sourceId, targetId)).ToArray<FireEvent>();
        }      

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceId">The sourceId of the FireAlarmSystem that sent 
        /// the FireEvent</param>
        /// <param name="eventType">The EventType of the FireEvent</param>
        /// <returns>returns a list of all FireEvents with matching sourceId and eventType</returns>
        [HttpGet, Route("type/{sourceId}/{eventType}")]     //todo: access only for admin, fas, restricted access for fb, sg
        public FireEvent[] GetFireEventsBySourceIdEventType(int sourceId, EventTypes eventType)
        {
            return (DatabaseOperations.Events.GetFireEventsBySourceIdEventType(sourceId, eventType)).ToArray<FireEvent>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventType">The EventType of the FireEvents</param>
        /// <returns>returns a list of all FireEvents with matching eventType</returns>
        [HttpGet, Route("type/{eventType}")]    //todo: access only for admin
        public FireEvent[] GetFireEventsByEventType(EventTypes eventType)
        {
            return (DatabaseOperations.Events.GetFireEventsByEventType(eventType)).ToArray<FireEvent>();
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
        [HttpGet, Route("time/{sourceId}/{startTime}/{endTime}")]//todo: access only for admin
        public FireEvent[] GetFireEventsBySourceIdTimespan(int sourceId, long startTime, long endTime)
        {
            return (DatabaseOperations.Events.GetFireEventsBySourceIdTimespan(sourceId, startTime, endTime)).ToArray<FireEvent>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startTime">The minimal value of the TimeStamp of the FireEvents</param>
        /// <param name="endTime">The maximal value of the TimeStamp of the FireEvents</param>
        /// <returns>returns a list of all FireEvents with a Timestamp between 
        /// startTime and endTime</returns>
        [HttpGet, Route("time/{startTime}/{endTime}")]//todo: access only for admin
        public FireEvent[] GetFireEventsByTimespan(long startTime, long endTime)
        {
            return (DatabaseOperations.Events.GetFireEventsByTimespan(startTime, endTime)).ToArray<FireEvent>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns all FireEvents from the given sourceId at the given date</returns>
        [HttpGet, Route("date/{sourceId}/{year}/{month}/{day}")]//todo: access only for admin, fas
        public FireEvent[] GetFireEventsByDate(int sourceId, int year, int month, int day)
        {
            return (DatabaseOperations.Events.GetFireEventsByDate(sourceId, year, month, day)).ToArray<FireEvent>();
        }

        ///<summary>
        ///
        ///</summary>
        /// <returns>returns all FireEvents from the given sourceId in the given year and month</returns>
        [HttpGet, Route("date/{sourceId}/{year}/{month}")]//todo: access only for admin, fas, restricted access for fb, sg
        public FireEvent[] GetFireEventsByDate(int sourceId, int year, int month)
        {
            return (DatabaseOperations.Events.GetFireEventsByDate(sourceId, year, month)).ToArray<FireEvent>();
        }

        ///<summary>
        ///
        ///</summary>
        /// <returns>returns all FireEvents from the given sourceId in the given year</returns>
        [HttpGet, Route("date/{sourceId}/{year}")]//todo: access only for admin, fas, restricted access for fb, sg
        public FireEvent[] GetFireEventsByDate(int sourceId, int year)
        {
            return (DatabaseOperations.Events.GetFireEventsByDate(sourceId, year)).ToArray<FireEvent>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventType">The EventType of the FireEvents</param>
        /// <param name="year">The year of the FireEvents' TimeStamp</param>
        /// <returns>returns an array with the number of FireEvents of the given EventType 
        /// that occured in the given year. Each column represents one month</returns>
        [HttpGet, Route("typeyear/{eventType}/{year}")] //todo: access only for admin
        public Int32[] CountFireEventsByEventTypePerYear(EventTypes eventType, int year)
        {
            return DatabaseOperations.Events.CountFireEventsByEventTypePerYear(eventType, year);
        }
    }
}
