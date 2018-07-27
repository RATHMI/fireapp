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
    public class FireEventController : ApiController
    {
        /// <summary>
        /// Inserts a FireEvent into the database or updates it if it already exists.
        /// </summary>
        /// <param name="fe">FireEvent that should be inserted into the Database.</param>
        /// <returns>Returns true if new object was inserted.</returns>
        [HttpPost, Route("upload")]     //todo: access only for admin, actual fire alarm system
        public bool UploadFireEvent([FromBody] FireEvent fe)
        {           
            try {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user == null)
                {
                    user = new User("dummy", "dummy", "dummy", "dummy", "dummy", UserTypes.unauthorized);
                }

                // Allow upsert without authentication, because there is no authentication concept for real
                // fire alarm systems yet.
                return DatabaseOperations.Events.Upsert(fe, user);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);               
                return false;
            }           
        }

        /// <summary>
        /// Allows the admin to export all FireEvents to a CSV file.
        /// </summary>
        /// <returns>Returns a CSV file with all FireEvents.</returns>
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
                        byte[] file = FileOperations.FireEventsFiles.ExportToCSV(DatabaseOperations.Events.GetAll());
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
                    // Notify user that the login was not successful.
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
        /// Checks if an id is already used by another FireEvent.
        /// </summary>
        /// <param name="sourceId">The sourceId you want to check.</param>
        /// <param name="eventId">The eventId you want to check</param>
        /// <returns>Returns true if id is not used by other FireEvent.</returns>
        [HttpPost, Route("checkid/{sourceId}/{eventId}")]     
        public bool CheckId(int sourceId, int eventId)
        {
            return DatabaseOperations.Events.CheckId(new FireEventId(sourceId, eventId));
        }

        /// <summary>
        /// Returns all FireEvents that the user is allowed to see.
        /// If you want to filter the result use the headers of your request
        /// (see Filter.FireEventsFilter.HeadersFilter).
        /// </summary>
        /// <returns>Returns all FireEvents that this user is allowed to see.</returns>
        [HttpGet, Route("all")] // todo: comment
        public FireEvent[] GetAll()
        {

            try { 
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {

                    // Get all FireEvents.
                    IEnumerable<FireEvent> events = DatabaseOperations.Events.GetAll();

                    // Filter FireEvents according to the UserType and AuthorizedObjectIds.
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
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new FireEvent[0];
            }                  
        }

        /// <summary>
        /// Returns all active FireEvents that the user is allowed to see.
        /// If you want to filter the result use the headers of your request
        /// (see Filter.FireEventsFilter.HeadersFilter).
        /// </summary>
        /// <returns>Returns all active FireEvents that this user is allowed to see.</returns>
        [HttpGet, Route("active")]
        public FireEvent[] GetActive()
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
        /// Finds a distinct FireEvent.
        /// </summary>
        /// <param name="sourceId">The sourceId of the FireAlarmSystem that sent 
        /// the FireEvent.</param>
        /// <param name="eventId">The ongoing number of the FireEvents of one
        /// FireAlarmSystem.</param>
        /// <returns>Returns a distinct FireEvent with a matching sourceId and eventId 
        /// (a FireEvent from a distinct FireAlarmSystem with the matching eventId).</returns>
        [HttpGet, Route("id/{sourceId}/{eventId}")] //todo: comment
        public FireEvent[] GetFireEventById(int sourceId, int eventId)
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    List<FireEvent> events = new List<FireEvent>();

                    // Get all FireEvents with a matching sourceId and eventId
                    FireEvent fe = DatabaseOperations.Events.GetById(sourceId, eventId);
                    if (fe != null)
                    {
                        events.Add(fe);
                    }

                    // Filter the FireEvents according to the User.
                    events = Filter.FireEventsFilter.UserFilter(events, user).ToList();
                    return events.ToArray();
                }
                else
                {
                    // Notify user that the login was not successful.
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
        /// the FireEvent.</param>
        /// <returns>returns a list of all Fireevents with a matching sourceId 
        /// (all Fireevents from a distinct fire alarm system).</returns>
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

                    // Get all FireEvents with a matching sourceId.
                    events = DatabaseOperations.Events.GetBySourceId(sourceId);

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
        /// <param name="sourceId">The sourceId of the FireAlarmSystem that sent 
        /// the FireEvent.</param>
        /// <param name="targetId">The id of the fire detector that sent the 
        /// FireEvent.</param>
        /// <returns>Returns a list of all FireEvents with matching sourceId and targetId.</returns>
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

                    // Get all FireEvents with a matching sourceId and targetId.
                    events = DatabaseOperations.Events.GetByTarget(sourceId, targetId);

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
                        return DatabaseOperations.Events.CountByEventTypePerYear(eventType, year);
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
