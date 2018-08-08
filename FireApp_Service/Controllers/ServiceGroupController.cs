using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FireApp.Domain;
using System.IO;
using System.Net.Http.Headers;
using System.Text;

namespace FireApp.Service.Controllers
{
    [RoutePrefix("service")]//todo: comment
    public class ServiceGroupController : ApiController
    {
        /// <summary>
        /// Inserts a ServiceGroup into the database or updates it if it already exists.
        /// </summary>
        /// <param name="sg">The ServiceGroup you want to insert.</param>
        /// <returns>Returns true if the ServiceGroup was inserted.</returns>
        [HttpPost, Route("upload")]
        public bool UpsertServiceGroup(ServiceGroup sg)
        {
            try {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    if (user.UserType == UserTypes.admin)
                    {
                        return DatabaseOperations.ServiceGroups.Upsert(sg, user);
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
        /// Inserts an array of ServiceGroups into the database or updates it if it already exists.
        /// </summary>
        /// <param name="sg">The ServiceGroups you want to insert.</param>
        /// <returns>Returns the number of upserted ServiceGroups.
        /// -1 : invalid or no token.
        /// -2 : user is not an admin.
        /// -3 : an error occurred.</returns>
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
                        return DatabaseOperations.ServiceGroups.BulkUpsert(sg, user);
                    }
                    else
                    {
                        // User is not an admin.
                        return -2;
                    }
                }
                else
                {
                    // Notify the User that the login was not successful.
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
        /// Returns all ServiceGroups as a CSV file.
        /// </summary>
        /// <returns>Returns a CSV file with all ServiceGroups.</returns>
        [HttpGet, Route("getcsv")]
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

                        // Get all ServiceGroups.
                        IEnumerable<ServiceGroup> sg = DatabaseOperations.ServiceGroups.GetAll();

                        // Convert ServiceGroups into a CSV file.
                        byte[] file = FileOperations.ServiceGroupFiles.ExportToCSV(sg);

                        // Write CSV file into the stream.
                        stream.Write(file, 0, file.Length);

                        // Set position of stream to 0 to avoid problems with the index.
                        stream.Position = 0;
                        result = new HttpResponseMessage(HttpStatusCode.OK);

                        // Add the CSV file to the content of the response.
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
                        // User is not an admin.
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
        /// Retrieves ServiceGroups from a CSV and upserts them.
        /// </summary>
        /// <param name="bytes">An array of bytes that represents a CSV file.</param>
        /// <returns>The number of successfully upserted ServiceGroups.</returns>
        [HttpPost, Route("uploadcsv")]//todo: comment
        public HttpResponseMessage UpsertCsv([FromBody] string byteArrayString)
        {
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    if (user.UserType == UserTypes.admin)
                    {
                        // todo: comment
                        IEnumerable<ServiceGroup> sg;
                        byteArrayString = byteArrayString.Trim('"');
                        List<byte> bytes = new List<byte>();
                        foreach (string s in byteArrayString.Split(' '))
                        {
                            bytes.Add(Convert.ToByte(s));
                        }

                        sg = FileOperations.ServiceGroupFiles.GetServiceGroupsFromCSV(bytes.ToArray());
                        int upserted = DatabaseOperations.ServiceGroups.BulkUpsert(sg, user);

                        // Sets the content of the response to the number of upserted ServiceGroups.
                        result.Content = new ByteArrayContent(Encoding.ASCII.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(upserted)));
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
        /// Deletes the ServiceGroup from the Database and Cache.
        /// The assoziations with the Users and FireAlarmSystems are also deleted.
        /// </summary>
        /// <param name="id">The id of the ServiceGroup you want to delete.</param>
        /// <returns>Returns true if ServiceGroup was deleted from the DB.</returns>
        [HttpGet, Route("delete/{id}")]
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
                        return DatabaseOperations.ServiceGroups.Delete(id, user);
                    }
                    else
                    {
                        // User is not an admin.
                        throw new InvalidOperationException();
                    }
                }
                else
                {
                    // Notify user that the login was not successful.
                    throw new NullReferenceException();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }                      
        }

        /// <summary>
        /// Returns all ServiceGroups.
        /// </summary>
        /// <returns>Returns a list of all ServiceGroups.</returns>
        [HttpGet, Route("all")]
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
                    // Notify user that the login was not successful.
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
        /// Checks if an id is already used by another ServiceGroup.
        /// </summary>
        /// <param name="id">The id you want to check.</param>
        /// <returns>Returns true if id is not used by other ServiceGroup or else a new id.</returns>
        [HttpGet, Route("checkid/{id}")]
        public int CheckId(int id)
        {
            return DatabaseOperations.ServiceGroups.CheckId(id);
        }

        /// <summary>
        /// Returns the ServiceGroup with a matching id.
        /// </summary>
        /// <param name="id">The id of the ServiceGroup you are looking for.</param>
        /// <returns>Returns a ServiceGroup with a matching id.</returns>
        [HttpGet, Route("id/{id}")]
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
                    // Notify user that the login was not successful.
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
        /// Returns all Users that are associated with this ServiceGroup.
        /// </summary>
        /// <param name="id">The ServiceGroup you want to get the Users of.</param>
        /// <returns>Returns all Users whose AuthorizedObjectIds contains id.</returns>
        [HttpGet, Route("users/{id}")]
        public User[] GetUsers(int id)
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    // Get all Users of the ServiceGroup.
                    IEnumerable<User> users = DatabaseOperations.ServiceGroups.GetUsers(id);

                    // Only return Users the User is allowed to see.
                    users = Filter.UsersFilter.UserFilter(users, user);

                    return users.ToArray();
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
                return new User[0];
            }
        }

        /// <summary>
        /// Returns all FireAlarmSystems that are associated with this ServiceGroup.
        /// </summary>
        /// <param name="servicegroup">The ServiceGroup you want to get the FireAlarmSystems of.</param>
        /// <returns>Returns all FireAlarmSystems that are associated with this ServiceGroup.</returns>
        [HttpGet, Route("fas/{id}")]
        public FireAlarmSystem[] GetFireAlarmSystems(int id)
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    // Get all FireAlarmsystems of the ServiceGroup.
                    IEnumerable<FireAlarmSystem> fas = DatabaseOperations.ServiceGroups.GetFireAlarmSystems(id);

                    // Only return FireAlarmSystems the User is allowed to see.
                    fas = Filter.FireAlarmSystemsFilter.UserFilter(fas, user);

                    return fas.ToArray();
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
                return new FireAlarmSystem[0];
            }
        }
    }
}
