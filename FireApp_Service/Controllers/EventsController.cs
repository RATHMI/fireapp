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
using System.Net.Http.Headers;
using System.Web;
using System.Text;

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
            try {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    Logging.Logger.Log("upsert", user.Id + "(" + user.FirstName + ", " + user.LastName + ")", fe);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);               
                return false;
            }
            return DatabaseOperations.Events.UpsertFireEvent(fe);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns a csv file with all FireEvents</returns>
        [HttpGet, Route("getcsv")]  //todo: comment
        public HttpResponseMessage GetCsv()
        {
            HttpResponseMessage result;
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    if (user.UserType == UserTypes.admin)
                    {
                        var stream = new MemoryStream();
                        byte[] file = FileOperations.FireEventsFiles.ExportToCSV(DatabaseOperations.Events.GetAllFireEvents());
                        stream.Write(file, 0, file.Length);

                        stream.Position = 0;
                        result = new HttpResponseMessage(HttpStatusCode.OK);
                        result.Content = new ByteArrayContent(stream.ToArray());

                        result.Content.Headers.ContentDisposition =
                            new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                            {
                                FileName = "FireEvents.csv"
                            };
                        result.Content.Headers.ContentType = new MediaTypeHeaderValue("text/csv");
                    }
                    else
                    {
                        result = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                        result.Content = null;
                    }
                }
                else
                {
                    result = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                    result.Content = null;
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                result = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                return result;
            }
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
        /// <returns>returns all FireEvents that this user is allowed to see</returns>
        [HttpGet, Route("all")]
        public FireEvent[] All()
        {

            try { 
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                
                    // get all FireEvents
                    List<FireEvent> events = DatabaseOperations.Events.GetAllFireEvents().ToList<FireEvent>();

                    // filter FireEvents according to the UserType and AuthorizedObjectIds
                    events = Filter.FireEventsFilter.UserFilter(events, user).ToList<FireEvent>();

                    // filter FireEvents according to the headers the client sent
                    events = Filter.FireEventsFilter.HeadersFilter(events, Request.Headers).ToList<FireEvent>();                    

                    return events.ToArray<FireEvent>();
                }
                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new FireEvent[0];
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
        [HttpGet, Route("id/{sourceId}/{eventId}")] //todo: comment
        public FireEvent[] GetFireEventById(int sourceId, int eventId)
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    IEnumerable<FireEvent> events;
                    events = DatabaseOperations.Events.GetFireEventById(sourceId, eventId);
                    events = Filter.FireEventsFilter.UserFilter(events, user);
                    return events.ToArray<FireEvent>();
                }
                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);               
                return new FireEvent[0];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceId">The sourceId of the FireAlarmSystem that sent 
        /// the FireEvent</param>
        /// <returns>returns a list of all Fireevents with a matching sourceId 
        /// (all Fireevents from a distinct fire alarm system)</returns>
        [HttpGet, Route("source/{sourceId}")]//todo: comment
        public FireEvent[] GetFireEventsBySourceId(int sourceId)
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    IEnumerable<FireEvent> events;
                    events = DatabaseOperations.Events.GetFireEventsBySourceId(sourceId);
                    events = Filter.FireEventsFilter.UserFilter(events, user);
                    return events.ToArray<FireEvent>();
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
        /// <param name="sourceId">The sourceId of the FireAlarmSystem that sent 
        /// the FireEvent</param>
        /// <param name="targetId">The id of the fire detector that sent the 
        /// FireEvent</param>
        /// <returns>returns a list of all FireEvents with matching sourceId and targetId</returns>
        [HttpGet, Route("target/{sourceId}/{targetId}")]//todo: comment
        public FireEvent[] GetFireEventsBySourceIdTargetId(int sourceId, string targetId)
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    IEnumerable<FireEvent> events;
                    events = DatabaseOperations.Events.GetFireEventsBySourceIdTargetId(sourceId, targetId);
                    events = Filter.FireEventsFilter.UserFilter(events, user);
                    return events.ToArray<FireEvent>();
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
        /// <param name="sourceId">The sourceId of the FireAlarmSystem that sent 
        /// the FireEvent</param>
        /// <param name="eventType">The EventType of the FireEvent</param>
        /// <returns>returns a list of all FireEvents with matching sourceId and eventType</returns>
        [HttpGet, Route("type/{sourceId}/{eventType}")]//todo: comment
        public FireEvent[] GetFireEventsBySourceIdEventType(int sourceId, EventTypes eventType)
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    IEnumerable<FireEvent> events;
                    events = DatabaseOperations.Events.GetFireEventsBySourceIdEventType(sourceId, eventType);
                    events = Filter.FireEventsFilter.UserFilter(events, user);
                    return events.ToArray<FireEvent>();
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
        /// <param name="eventType">The EventType of the FireEvents</param>
        /// <returns>returns a list of all FireEvents with matching eventType</returns>
        [HttpGet, Route("type/{eventType}")]//todo: comment
        public FireEvent[] GetFireEventsByEventType(EventTypes eventType)
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    IEnumerable<FireEvent> events;
                    events = DatabaseOperations.Events.GetFireEventsByEventType(eventType);
                    events = Filter.FireEventsFilter.UserFilter(events, user);
                    return events.ToArray<FireEvent>();
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
        /// <param name="sourceId">The sourceId of the FireAlarmSystem that sent 
        /// the FireEvent</param>
        /// <param name="startTime">The minimal value of the TimeStamp of the FireEvents</param>
        /// <param name="endTime">The maximal value of the TimeStamp of the FireEvents</param>
        /// <returns>returns a list of all FireEvents with matching sourceId and and a Timestamp between 
        /// startTime and endTime</returns>
        [HttpGet, Route("time/{sourceId}/{startTime}/{endTime}")]//todo: comment
        public FireEvent[] GetFireEventsBySourceIdTimespan(int sourceId, long startTime, long endTime)
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    IEnumerable<FireEvent> events;
                    events = DatabaseOperations.Events.GetFireEventsBySourceIdTimespan(sourceId, startTime, endTime);
                    events = Filter.FireEventsFilter.UserFilter(events, user);
                    return events.ToArray<FireEvent>();
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
        /// <param name="startTime">The minimal value of the TimeStamp of the FireEvents</param>
        /// <param name="endTime">The maximal value of the TimeStamp of the FireEvents</param>
        /// <returns>returns a list of all FireEvents with a Timestamp between 
        /// startTime and endTime</returns>
        [HttpGet, Route("time/{startTime}/{endTime}")]//todo: comment
        public FireEvent[] GetFireEventsByTimespan(long startTime, long endTime)
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    IEnumerable<FireEvent> events;
                    events = DatabaseOperations.Events.GetFireEventsByTimespan(startTime, endTime);
                    events = Filter.FireEventsFilter.UserFilter(events, user);
                    return events.ToArray<FireEvent>();
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
        /// <returns>returns all FireEvents from the given sourceId at the given date</returns>
        [HttpGet, Route("date/{sourceId}/{year}/{month}/{day}")]//todo: comment
        public FireEvent[] GetFireEventsByDate(int sourceId, int year, int month, int day)
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    IEnumerable<FireEvent> events;
                    events = DatabaseOperations.Events.GetFireEventsByDate(sourceId, year, month, day);
                    events = Filter.FireEventsFilter.UserFilter(events, user);
                    return events.ToArray<FireEvent>();
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

        ///<summary>
        ///
        ///</summary>
        /// <returns>returns all FireEvents from the given sourceId in the given year and month</returns>
        [HttpGet, Route("date/{sourceId}/{year}/{month}")]//todo: comment
        public FireEvent[] GetFireEventsByDate(int sourceId, int year, int month)
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    IEnumerable<FireEvent> events;
                    events = DatabaseOperations.Events.GetFireEventsByDate(sourceId, year, month);
                    events = Filter.FireEventsFilter.UserFilter(events, user);
                    return events.ToArray<FireEvent>();
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

        ///<summary>
        ///
        ///</summary>
        /// <returns>returns all FireEvents from the given sourceId in the given year</returns>
        [HttpGet, Route("date/{sourceId}/{year}")]//todo: comment
        public FireEvent[] GetFireEventsByDate(int sourceId, int year)
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    IEnumerable<FireEvent> events;
                    events = DatabaseOperations.Events.GetFireEventsByDate(sourceId, year);
                    events = Filter.FireEventsFilter.UserFilter(events, user);
                    return events.ToArray<FireEvent>();
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
        /// <param name="eventType">The EventType of the FireEvents</param>
        /// <param name="year">The year of the FireEvents' TimeStamp</param>
        /// <returns>returns an array with the number of FireEvents of the given EventType 
        /// that occured in the given year. Each column represents one month</returns>
        [HttpGet, Route("typeyear/{eventType}/{year}")]//todo: comment
        public Int32[] CountFireEventsByEventTypePerYear(EventTypes eventType, int year)
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    if (user.UserType == UserTypes.admin)
                    {
                        return DatabaseOperations.Events.CountFireEventsByEventTypePerYear(eventType, year);
                    }
                    else
                    {
                        throw new UnauthorizedAccessException();
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new Int32[12];
            }
        }
    }
}
