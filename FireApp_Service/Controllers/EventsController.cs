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
                IEnumerable<User> users = Authentication.Token.VerifyToken(Authentication.Token.GetTokenFromHeader(Request.Headers));
                if (users != null)
                {
                    User user = users.First<User>();
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
        [HttpGet, Route("getcsv")]
        public HttpResponseMessage GetCsv()
        {
            HttpResponseMessage result;
            try
            {
                IEnumerable<User> user = Authentication.Token.VerifyToken(Authentication.Token.GetTokenFromHeader(Request.Headers));
                if (user != null)
                {
                    if (user.First<User>().UserType == UserTypes.admin)
                    {
                        var stream = new MemoryStream();
                        byte[] file = FileOperations.FireEventsFiles.ExportToCSV(DatabaseOperations.Events.GetAllFireEvents());
                        stream.Write(file, 0, file.Length);

                        stream.Position = 0;
                        result = new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new ByteArrayContent(stream.ToArray())
                        };
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
            IEnumerable<User> user = Authentication.Token.VerifyToken(Authentication.Token.GetTokenFromHeader(Request.Headers));
            if (user != null)
            {
                try
                {
                    //todo: comment
                    IEnumerable<FireEvent> events = Filter.FireEventsFilter.UserFilter((DatabaseOperations.Events.GetAllFireEvents()), user.First<User>()).ToArray<FireEvent>();
                    events = Filter.FireEventsFilter.HeadersFilter(events, Request.Headers);                    

                    return events.ToArray<FireEvent>();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return new FireEvent[0];
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
        [HttpGet, Route("id/{sourceId}/{eventId}")]
        public FireEvent[] GetFireEventById(int sourceId, int eventId)
        {
            try
            {
                IEnumerable<User> user = Authentication.Token.VerifyToken(Authentication.Token.GetTokenFromHeader(Request.Headers));
                if (user != null)
                {
                    return Filter.FireEventsFilter.UserFilter(DatabaseOperations.Events.GetFireEventById(sourceId, eventId), user.First<User>()).ToArray<FireEvent>();
                }
                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);               
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceId">The sourceId of the FireAlarmSystem that sent 
        /// the FireEvent</param>
        /// <returns>returns a list of all Fireevents with a matching sourceId 
        /// (all Fireevents from a distinct fire alarm system)</returns>
        [HttpGet, Route("source/{sourceId}")]
        public FireEvent[] GetFireEventsBySourceId(int sourceId)
        {
            try
            {
                IEnumerable<User> user = Authentication.Token.VerifyToken(Authentication.Token.GetTokenFromHeader(Request.Headers));
                if (user != null)
                {
                    return Filter.FireEventsFilter.UserFilter(DatabaseOperations.Events.GetFireEventsBySourceId(sourceId), user.First<User>()).ToArray<FireEvent>();
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
        /// <param name="sourceId">The sourceId of the FireAlarmSystem that sent 
        /// the FireEvent</param>
        /// <param name="targetId">The id of the fire detector that sent the 
        /// FireEvent</param>
        /// <returns>returns a list of all FireEvents with matching sourceId and targetId</returns>
        [HttpGet, Route("target/{sourceId}/{targetId}")]
        public FireEvent[] GetFireEventsBySourceIdTargetId(int sourceId, string targetId)
        {
            try
            {
                IEnumerable<User> user = Authentication.Token.VerifyToken(Authentication.Token.GetTokenFromHeader(Request.Headers));
                if (user != null)
                {
                    return Filter.FireEventsFilter.UserFilter(DatabaseOperations.Events.GetFireEventsBySourceIdTargetId(sourceId, targetId), user.First<User>()).ToArray<FireEvent>();
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
        /// <param name="sourceId">The sourceId of the FireAlarmSystem that sent 
        /// the FireEvent</param>
        /// <param name="eventType">The EventType of the FireEvent</param>
        /// <returns>returns a list of all FireEvents with matching sourceId and eventType</returns>
        [HttpGet, Route("type/{sourceId}/{eventType}")]
        public FireEvent[] GetFireEventsBySourceIdEventType(int sourceId, EventTypes eventType)
        {
            try
            {
                IEnumerable<User> user = Authentication.Token.VerifyToken(Authentication.Token.GetTokenFromHeader(Request.Headers));
                if (user != null)
                {
                    return Filter.FireEventsFilter.UserFilter(DatabaseOperations.Events.GetFireEventsBySourceIdEventType(sourceId, eventType), user.First<User>()).ToArray<FireEvent>();
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
        /// <param name="eventType">The EventType of the FireEvents</param>
        /// <returns>returns a list of all FireEvents with matching eventType</returns>
        [HttpGet, Route("type/{eventType}")]
        public FireEvent[] GetFireEventsByEventType(EventTypes eventType)
        {
            try
            {
                IEnumerable<User> user = Authentication.Token.VerifyToken(Authentication.Token.GetTokenFromHeader(Request.Headers));
                if (user != null)
                {
                    return Filter.FireEventsFilter.UserFilter(DatabaseOperations.Events.GetFireEventsByEventType(eventType), user.First<User>()).ToArray<FireEvent>();
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
        /// <param name="sourceId">The sourceId of the FireAlarmSystem that sent 
        /// the FireEvent</param>
        /// <param name="startTime">The minimal value of the TimeStamp of the FireEvents</param>
        /// <param name="endTime">The maximal value of the TimeStamp of the FireEvents</param>
        /// <returns>returns a list of all FireEvents with matching sourceId and and a Timestamp between 
        /// startTime and endTime</returns>
        [HttpGet, Route("time/{sourceId}/{startTime}/{endTime}")]
        public FireEvent[] GetFireEventsBySourceIdTimespan(int sourceId, long startTime, long endTime)
        {
            try
            {
                IEnumerable<User> user = Authentication.Token.VerifyToken(Authentication.Token.GetTokenFromHeader(Request.Headers));
                if (user != null)
                {
                    return Filter.FireEventsFilter.UserFilter(DatabaseOperations.Events.GetFireEventsBySourceIdTimespan(sourceId, startTime, endTime), user.First<User>()).ToArray<FireEvent>();
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
        /// <param name="startTime">The minimal value of the TimeStamp of the FireEvents</param>
        /// <param name="endTime">The maximal value of the TimeStamp of the FireEvents</param>
        /// <returns>returns a list of all FireEvents with a Timestamp between 
        /// startTime and endTime</returns>
        [HttpGet, Route("time/{startTime}/{endTime}")]
        public FireEvent[] GetFireEventsByTimespan(long startTime, long endTime)
        {
            try
            {
                IEnumerable<User> user = Authentication.Token.VerifyToken(Authentication.Token.GetTokenFromHeader(Request.Headers));
                if (user != null)
                {
                    return Filter.FireEventsFilter.UserFilter(DatabaseOperations.Events.GetFireEventsByTimespan(startTime, endTime), user.First<User>()).ToArray<FireEvent>();
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
        /// <returns>returns all FireEvents from the given sourceId at the given date</returns>
        [HttpGet, Route("date/{sourceId}/{year}/{month}/{day}")]
        public FireEvent[] GetFireEventsByDate(int sourceId, int year, int month, int day)
        {
            try
            {
                IEnumerable<User> user = Authentication.Token.VerifyToken(Authentication.Token.GetTokenFromHeader(Request.Headers));
                if (user != null)
                {
                    return Filter.FireEventsFilter.UserFilter(DatabaseOperations.Events.GetFireEventsByDate(sourceId, year, month, day), user.First<User>()).ToArray<FireEvent>();
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

        ///<summary>
        ///
        ///</summary>
        /// <returns>returns all FireEvents from the given sourceId in the given year and month</returns>
        [HttpGet, Route("date/{sourceId}/{year}/{month}")]
        public FireEvent[] GetFireEventsByDate(int sourceId, int year, int month)
        {
            try
            {
                IEnumerable<User> user = Authentication.Token.VerifyToken(Authentication.Token.GetTokenFromHeader(Request.Headers));
                if (user != null)
                {
                    return Filter.FireEventsFilter.UserFilter(DatabaseOperations.Events.GetFireEventsByDate(sourceId, year, month), user.First<User>()).ToArray<FireEvent>();
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

        ///<summary>
        ///
        ///</summary>
        /// <returns>returns all FireEvents from the given sourceId in the given year</returns>
        [HttpGet, Route("date/{sourceId}/{year}")]
        public FireEvent[] GetFireEventsByDate(int sourceId, int year)
        {
            try
            {
                IEnumerable<User> user = Authentication.Token.VerifyToken(Authentication.Token.GetTokenFromHeader(Request.Headers));
                if (user != null)
                {
                    return Filter.FireEventsFilter.UserFilter(DatabaseOperations.Events.GetFireEventsByDate(sourceId, year), user.First<User>()).ToArray<FireEvent>();
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
        /// <param name="eventType">The EventType of the FireEvents</param>
        /// <param name="year">The year of the FireEvents' TimeStamp</param>
        /// <returns>returns an array with the number of FireEvents of the given EventType 
        /// that occured in the given year. Each column represents one month</returns>
        [HttpGet, Route("typeyear/{eventType}/{year}")]
        public Int32[] CountFireEventsByEventTypePerYear(EventTypes eventType, int year)
        {
            try
            {
                IEnumerable<User> user = Authentication.Token.VerifyToken(Authentication.Token.GetTokenFromHeader(Request.Headers));
                if (user != null)
                {
                    if (user.First<User>().UserType == UserTypes.admin)
                    {
                        return DatabaseOperations.Events.CountFireEventsByEventTypePerYear(eventType, year);
                    }
                    else
                    {
                        return new Int32[12];
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
                return null;
            }
        }
    }
}
