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
                        Logging.Logger.Log("upsert", user.Id + "(" + user.FirstName + ", " + user.LastName + ")", fas);
                        return DatabaseOperations.FireAlarmSystems.UpsertFireAlarmSystem(fas);
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
                    fas = DatabaseOperations.FireAlarmSystems.GetAllFireAlarmSystems();
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
                        IEnumerable<FireAlarmSystem> fas = DatabaseOperations.FireAlarmSystems.GetAllFireAlarmSystems();
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
                    fas = new List<FireAlarmSystem> { DatabaseOperations.FireAlarmSystems.GetFireAlarmSystemById(id) };
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
