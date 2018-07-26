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
    [RoutePrefix("service")]//todo: comment
    public class ServiceGroupController : ApiController
    {
        /// <summary>
        /// inserts a ServiceGroup into the database or updates it if it already exists
        /// </summary>
        /// <param name="sg">The ServiceGroup you want to insert</param>
        /// <returns>returns true if ServiceGroup was inserted</returns>
        [HttpPost, Route("upload")]//todo: comment
        public bool UpsertServiceGroup(ServiceGroup sg)
        {
            try {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    if (user.UserType == UserTypes.admin)
                    {
                        Logging.Logger.Log("upsert", user.GetUserDescription(), sg);
                        return DatabaseOperations.ServiceGroups.Upsert(sg);
                    }
                }
                return false;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);               
                return false;
            }                      
        }

        /// <summary>
        /// inserts an array of ServiceGroups into the database or updates it if it already exists
        /// </summary>
        /// <param name="sg">The ServiceGroups you want to insert</param>
        /// <returns>returns the number of upserted ServiceGroups.
        /// -1 : invalid or no token
        /// -2 : user is not an admin
        /// -3 : an error occurred</returns>
        [HttpPost, Route("uploadbulk")]
        public int UpsertBulk([FromBody] ServiceGroup[] sg)
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    if (user.UserType == UserTypes.admin)
                    {
                        Logging.Logger.Log("upsert", user.GetUserDescription(), user);
                        return DatabaseOperations.ServiceGroups.BulkUpsert(sg);
                    }
                    else
                    {
                        return -2;
                    }
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -3;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns a csv file with all ServiceGroups</returns>
        [HttpGet, Route("getcsv")]//todo: comment
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
                        IEnumerable<ServiceGroup> sg;
                        sg = DatabaseOperations.ServiceGroups.GetAll();
                        byte[] file = FileOperations.ServiceGroupFiles.ExportToCSV(sg);
                        stream.Write(file, 0, file.Length);

                        stream.Position = 0;
                        result = new HttpResponseMessage(HttpStatusCode.OK);
                        result.Content = new ByteArrayContent(stream.ToArray());
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
        [HttpGet, Route("delete/{id}")]//todo: comment
        public bool DeleteServiceGroup(int id)
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    if (user.UserType == UserTypes.admin)
                    {
                        ServiceGroup old = DatabaseOperations.ServiceGroups.GetById(id);
                        Logging.Logger.Log("delete", user.GetUserDescription(), old);
                        return DatabaseOperations.ServiceGroups.Delete(id);
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }                      
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns a list with all ServiceGroups</returns>
        [HttpGet, Route("all")]//todo: comment
        public ServiceGroup[] GetAllServiceGroups()
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    IEnumerable<ServiceGroup> sg;
                    sg = DatabaseOperations.ServiceGroups.GetAll();
                    sg = Filter.ServiceGroupsFilter.UserFilter(sg, user);
                    return sg.ToArray();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new ServiceGroup[0];
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
        [HttpGet, Route("id/{id}")]//todo: comment
        public ServiceGroup[] GetServiceGroupById(int id)
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    IEnumerable<ServiceGroup> sg;
                    sg = new List<ServiceGroup> { DatabaseOperations.ServiceGroups.GetById(id) };
                    sg = Filter.ServiceGroupsFilter.UserFilter(sg, user);
                    return sg.ToArray();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new ServiceGroup[0];
            }
        }

        [HttpGet, Route("users/{id}")] // todo: comment
        public User[] GetUsers(int id)
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    if (user.UserType == UserTypes.admin)
                    {
                        return DatabaseOperations.ServiceGroups.GetUsers(id).ToArray();
                    }
                }
                throw new NullReferenceException();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new User[0];
            }
        }
    }
}
