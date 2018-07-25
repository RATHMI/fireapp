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
    [RoutePrefix("fas")]
    public class FireAlarmSystemController : ApiController
    {
        /// <summary>
        /// Inserts a FireAlarmSystem into the database or updates it if it already exists.
        /// </summary>
        /// <param name="fas">The FireAlarmSystem you want to upsert.</param>
        /// <returns>Returns true if the FireAlarmSystem was inserted.</returns>
        [HttpPost, Route("upload")]//todo: comment
        public bool UploadFireAlarmSystem([FromBody] FireAlarmSystem fas)
        {
            try {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    if (user.UserType == UserTypes.admin)
                    {
                        Logging.Logger.Log("upsert", user.GetUserDescription(), fas);
                        return DatabaseOperations.BasicOperations.FireAlarmSystems.Upsert(fas);
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
        /// Inserts an array of FireAlarmSystems into the database or updates it if it already exists.
        /// </summary>
        /// <param name="fas">The FireAlarmSystems you want to upsert<./param>
        /// <returns>Returns the number of upserted FireAlarmSystems.
        /// -1 : invalid or no token.
        /// -2 : user is not an admin.
        /// -3 : an error occurred.</returns>
        [HttpPost, Route("uploadbulk")]
        public int UpsertBulk([FromBody] FireAlarmSystem[] fas)
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
                        return DatabaseOperations.BasicOperations.FireAlarmSystems.BulkUpsert(fas);
                    }
                    else
                    {
                        // User is not an admin.
                        return -2;
                    }
                }
                else
                {
                    // User is not logged in.
                    return -1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -3;
            }
        }

        //todo: implement method "FromCSV" with option insert or update to prevent unwanted updates

        /// <summary>
        /// Checks if an id is already used by another FireAlarmSystem.
        /// </summary>
        /// <param name="id">The id you want to check.</param>
        /// <returns>Returns id if id is not used by other FireAlarmSystem or else an unused id.</returns>
        [HttpPost, Route("checkid/{id}")]
        public int CheckId(int id)
        {
            return DatabaseOperations.BasicOperations.FireAlarmSystems.CheckId(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns a list of all FireAlarmSystems.</returns>
        [HttpGet, Route("all")]//todo: comment
        public FireAlarmSystem[] All()
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    IEnumerable<FireAlarmSystem> fas;

                    // Get all FireAlarmSystems.
                    fas = DatabaseOperations.BasicOperations.FireAlarmSystems.GetAll();

                    // Filter the FireAlarmSystems according to the User.
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

        /// <summary>
        /// Allows the admin to export all FireAlarmSystems to a CSV file.
        /// </summary>
        /// <returns>Returns a CSV file with all FireAlarmSystems.</returns>
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

                        // Get all FireAlarmSystems.
                        IEnumerable<FireAlarmSystem> fas = DatabaseOperations.BasicOperations.FireAlarmSystems.GetAll();

                        // Convert FireAlarmSystems into a CSV file.
                        byte[] file = FileOperations.FireAlarmSystemFiles.ExportToCSV(fas);

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
                                FileName = "FireAlarmSystems.csv"
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
        /// 
        /// </summary>
        /// <returns>Returns a list of all FireAlarmSystems with active FireEvents.</returns>
        [HttpGet, Route("active")]//todo: comment
        public FireAlarmSystem[] GetActiveFireAlarmSystems()
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    IEnumerable<FireAlarmSystem> fas;

                    fas = DatabaseOperations.AdvancedOperations.FireAlarmSystems.GetActiveFireAlarmSystems(user);

                    fas = Filter.FireAlarmSystemsFilter.UserFilter(fas, user);
                    return fas.ToArray();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new FireAlarmSystem[0];
            }
                
        }

        /// <summary>
        /// Returns a list of souceIds from FireEvents where there is no FireAlarmSystem with a matching Id
        /// </summary>
        /// <returns>returns a list of IDs</returns>
        [HttpGet, Route("unregistered")]
        public int[] GetUnregistered()
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    if (user.UserType == UserTypes.admin)
                    {
                        return DatabaseOperations.AdvancedOperations.FireAlarmSystems.GetUnregistered().ToArray();
                    }
                }
                else
                {
                    return null;
                }
                return new int[0];
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new int[0];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">The id of the FireAlarmSystem you are looking for</param>
        /// <returns>returns a FireAlarmSystem with a matching id</returns>
        [HttpGet, Route("id/{id}")]//todo: comment
        public FireAlarmSystem[] GetFireAlarmSystemById(int id)
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    IEnumerable<FireAlarmSystem> fas;
                    fas = new List<FireAlarmSystem> { DatabaseOperations.BasicOperations.FireAlarmSystems.GetById(id) };
                    fas = Filter.FireAlarmSystemsFilter.UserFilter(fas, user);
                    return fas.ToArray();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new FireAlarmSystem[0];
            }
        }

        /// <summary>
        /// Returns FireBrigades or ServiceGroups or both 
        /// that are associated with this FireAlarmSystem.
        /// </summary>
        /// <param name="id">The id of the FireAlarmSystem.</param>
        /// <param name="type">The type of member (fb, sg) you want.</param>
        /// <returns>Returns members associated with this FireAlarmSystem.</returns>
        [HttpGet, Route("members/{id}/{type}")]
        public object[] GetMembers(int id, string type) //todo: comment
        {
            IEnumerable<object> results = new object[0];

            try
            {
                FireAlarmSystem fas = DatabaseOperations.BasicOperations.FireAlarmSystems.GetById(id);

                if (type == "fb")
                {
                    results = DatabaseOperations.AdvancedOperations.FireAlarmSystems.GetMembers(fas, typeof(FireBrigade));
                }
                else
                {
                    if (type == "sg")
                    {
                        results = DatabaseOperations.AdvancedOperations.FireAlarmSystems.GetMembers(fas, typeof(ServiceGroup));
                    }
                    else
                    {
                        results = DatabaseOperations.AdvancedOperations.FireAlarmSystems.GetMembers(fas);
                    }
                }

                return results.ToArray();
            }
            catch (Exception)
            {
                return new object[0];
            }
        }

        /// <summary>
        /// Returns the Users that are associated with this FireAlarmSystem.
        /// </summary>
        /// <param name="id">The id of the FireAlarmSystem</param>
        /// <param name="type">The type of User (fb, sg, fas).</param>
        /// <returns>Returns Users of this FireAlarmSystem of the given type.</returns>
        [HttpGet, Route("users/{id}/{type}")]
        public User[] GetUsers(int id, string type) //todo: comment
        {
            List<User> results = new List<User>();

            try
            {
                FireAlarmSystem fas = DatabaseOperations.BasicOperations.FireAlarmSystems.GetById(id);

                if (type == "fb")
                {
                    results.AddRange(DatabaseOperations.AdvancedOperations.FireAlarmSystems.GetUsers(fas, UserTypes.firebrigade));
                }
                else
                {
                    if (type == "sg")
                    {
                        results.AddRange(DatabaseOperations.AdvancedOperations.FireAlarmSystems.GetUsers(fas, UserTypes.servicemember));
                    }
                    else
                    {
                        if (type == "fas")
                        {
                            results.AddRange(DatabaseOperations.AdvancedOperations.FireAlarmSystems.GetUsers(fas, UserTypes.firealarmsystem));
                        }
                        else
                        {
                            results.AddRange(DatabaseOperations.AdvancedOperations.FireAlarmSystems.GetUsers(fas));
                        }
                    }
                }

                return results.ToArray();
            }
            catch (Exception)
            {
                return new User[0];
            }
        }
    }
}
