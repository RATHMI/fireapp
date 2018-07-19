using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FireApp.Domain;
using System.IO;
using System.Net.Http.Headers;

namespace FireApp.Service.Controllers
{
    [RoutePrefix("service")]
    public class ServiceGroupController : ApiController
    {
        /// <summary>
        /// inserts a ServiceGroup into the database or updates it if it already exists
        /// </summary>
        /// <param name="sg">The ServiceGroup you want to insert</param>
        /// <returns>returns true if ServiceGroup was inserted</returns>
        [HttpPost, Route("upload")]
        public bool UpsertServiceGroup(ServiceGroup sg)
        {
            try { 
                IEnumerable<User> users = Authentication.Token.VerifyToken(Authentication.Token.GetTokenFromHeader(Request.Headers));
                if (users != null)
                {
                    if (users.First<User>().UserType == UserTypes.admin)
                    {
                        User user = users.First<User>();
                        Logging.Logger.Log("upsert", user.Id + "(" + user.FirstName + ", " + user.LastName + ")", sg);
                        return DatabaseOperations.ServiceGroups.UpsertServiceGroup(sg);
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);               
                return false;
            }
            return false;          
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns a csv file with all ServiceGroups</returns>
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
                        byte[] file = FileOperations.ServiceGroupFiles.ExportToCSV(DatabaseOperations.ServiceGroups.GetAllServiceGroups());
                        stream.Write(file, 0, file.Length);

                        stream.Position = 0;
                        result = new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new ByteArrayContent(stream.ToArray())
                        };
                        result.Content.Headers.ContentDisposition =
                            new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                            {
                                FileName = "ServiceGroups.csv"
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

        //todo: implement method "FromCSV"

        /// <summary>
        /// Deletes the ServiceGroup from the Database and Cache
        /// The assoziations with the users and FireAlarmSystems are also deleted
        /// </summary>
        /// <param name="id">the id of the ServiceGroup you want to delete</param>
        /// <returns>returns true if ServiceGroup was deleted from DB</returns>
        [HttpGet, Route("delete/{id}")]
        public bool DeleteServiceGroup(int id)
        {
            try
            {
                IEnumerable<User> users = Authentication.Token.VerifyToken(Authentication.Token.GetTokenFromHeader(Request.Headers));
                if (users != null)
                {
                    if (users.First<User>().UserType == UserTypes.admin)
                    {
                        User user = users.First<User>();
                        Logging.Logger.Log("delete", user.Id + "(" + user.FirstName + ", " + user.LastName + ")", DatabaseOperations.ServiceGroups.GetServiceGroupById(id));
                        return DatabaseOperations.ServiceGroups.DeleteServiceGroup(id);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return false;          
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns a list with all ServiceGroups</returns>
        [HttpGet, Route("all")]
        public ServiceGroup[] GetAllServiceGroups()
        {
            try
            {
                IEnumerable<User> user = Authentication.Token.VerifyToken(Authentication.Token.GetTokenFromHeader(Request.Headers));
                if (user != null)
                {
                    return Filter.ServiceGroupsFilter.UserFilter(DatabaseOperations.ServiceGroups.GetAllServiceGroups(), user.First<User>()).ToArray<ServiceGroup>();
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
        /// Checks if an id is already used by another ServiceGroup
        /// </summary>
        /// <param name="id">the id you want to check</param>
        /// <returns>returns true if id is not used by other ServiceGroup or else a new id</returns>
        [HttpPost, Route("checkid/{id}")]
        public static int CheckId(int id)
        {
            return DatabaseOperations.ServiceGroups.CheckId(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">The id of the ServiceGroup you are looking for</param>
        /// <returns>returns a ServiceGroup with a matching id</returns>
        [HttpGet, Route("id/{id}")]
        public ServiceGroup[] GetServiceGroupById(int id)
        {
            try
            {
                IEnumerable<User> user = Authentication.Token.VerifyToken(Authentication.Token.GetTokenFromHeader(Request.Headers));
                if (user != null)
                {
                    return Filter.ServiceGroupsFilter.UserFilter(DatabaseOperations.ServiceGroups.GetServiceGroupById(id), user.First<User>()).ToArray<ServiceGroup>();
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
