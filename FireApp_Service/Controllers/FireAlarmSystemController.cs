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
        /// inserts a FireAlarmSystem into the database or updates it if it already exists
        /// </summary>
        /// <param name="fas">The FireAlarmSystem you want to insert</param>
        /// <returns>returns true if the insert was successful</returns>
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
                        return DatabaseOperations.FireAlarmSystems.Upsert(fas);
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
        /// inserts an array of FireAlarmSystems into the database or updates it if it already exists
        /// </summary>
        /// <param name="fas">The FireAlarmSystems you want to insert</param>
        /// <returns>returns the number of upserted FireAlarmSystems.
        /// -1 : invalid or no token
        /// -2 : user is not an admin
        /// -3 : an error occurred</returns>
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
                        return DatabaseOperations.FireAlarmSystems.BulkUpsert(fas);
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

        //todo: implement method "FromCSV" with option insert or update to prevent unwanted updates

        /// <summary>
        /// Checks if an id is already used by another FireAlarmSystem
        /// </summary>
        /// <param name="id">the id you want to check</param>
        /// <returns>returns id if id is not used by other FireAlarmSystem or else a new id</returns>
        [HttpPost, Route("checkid/{id}")]
        public int CheckId(int id)
        {
            return DatabaseOperations.FireAlarmSystems.CheckId(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns a list of all FireAlarmSystems</returns>
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
                    fas = DatabaseOperations.FireAlarmSystems.GetAll();
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
        /// 
        /// </summary>
        /// <returns>returns a csv file with all FireAlarmSystems</returns>
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
                        IEnumerable<FireAlarmSystem> fas = DatabaseOperations.FireAlarmSystems.GetAll();
                        byte[] file = FileOperations.FireAlarmSystemFiles.ExportToCSV(fas);
                        stream.Write(file, 0, file.Length);

                        stream.Position = 0;
                        result = new HttpResponseMessage(HttpStatusCode.OK);
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
        /// <returns>returns a list of all FireAlarmSystems with active FireEvents</returns>
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
                    fas = DatabaseOperations.FireAlarmSystems.GetActiveFireAlarmSystems(user);
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
                        return DatabaseOperations.FireAlarmSystems.GetUnregistered().ToArray();
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
                    fas = new List<FireAlarmSystem> { DatabaseOperations.FireAlarmSystems.GetById(id) };
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
    }
}
