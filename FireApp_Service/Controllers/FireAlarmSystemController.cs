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
        [HttpPost, Route("upload")]
        public bool UploadFireAlarmSystem([FromBody] FireAlarmSystem fas)
        {
            try { 
                IEnumerable<User> users = Authentication.Token.VerifyToken(Authentication.Token.GetTokenFromHeader(Request.Headers));
                if (users != null)
                {
                    if (users.First<User>().UserType == UserTypes.admin)
                    {
                        User user = users.First<User>();
                        Logging.Logger.Log("upsert", user.Id + "(" + user.FirstName + ", " + user.LastName + ")", fas);
                        return DatabaseOperations.FireAlarmSystems.UpsertFireAlarmSystem(fas);
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
        [HttpGet, Route("all")]
        public FireAlarmSystem[] All()
        {
            try
            {
                IEnumerable<User> user = Authentication.Token.VerifyToken(Authentication.Token.GetTokenFromHeader(Request.Headers));
                if (user != null)
                {
                    return Filter.FireAlarmSystemsFilter.UserFilter(DatabaseOperations.FireAlarmSystems.GetAllFireAlarmSystems(), user.First<User>()).ToArray<FireAlarmSystem>();
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
        /// <returns>returns a csv file with all FireAlarmSystems</returns>
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
                        byte[] file = FileOperations.FireAlarmSystemFiles.ExportToCSV(DatabaseOperations.FireAlarmSystems.GetAllFireAlarmSystems());
                        stream.Write(file, 0, file.Length);

                        stream.Position = 0;
                        result = new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new ByteArrayContent(stream.ToArray())
                        };
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
        [HttpGet, Route("active")]
        public FireAlarmSystem[] GetActiveFireAlarmSystems()
        {
            try
            {
                IEnumerable<User> user = Authentication.Token.VerifyToken(Authentication.Token.GetTokenFromHeader(Request.Headers));
                if (user != null && user.Count() > 0)
                {
                    return Filter.FireAlarmSystemsFilter.UserFilter((DatabaseOperations.FireAlarmSystems.GetActiveFireAlarmSystems(user.First<User>())), user.First<User>()).ToArray<FireAlarmSystem>();
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
        /// <param name="id">The id of the FireAlarmSystem you are looking for</param>
        /// <returns>returns a FireAlarmSystem with a matching id</returns>
        [HttpGet, Route("id/{id}")]
        public FireAlarmSystem[] GetFireAlarmSystemById(int id)
        {
            try
            {
                IEnumerable<User> user = Authentication.Token.VerifyToken(Authentication.Token.GetTokenFromHeader(Request.Headers));
                if (user != null)
                {
                    return Filter.FireAlarmSystemsFilter.UserFilter(DatabaseOperations.FireAlarmSystems.GetFireAlarmSystemById(id), user.First<User>()).ToArray<FireAlarmSystem>();
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
