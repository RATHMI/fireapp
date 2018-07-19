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
    [RoutePrefix("fb")]
    public class FireBrigadeController : ApiController
    {
        /// <summary>
        /// inserts a FireBrigade into the database or updates it if it already exists
        /// </summary>
        /// <param name="fb"></param>
        /// <returns>returns true if the insert was successful</returns>
        [HttpPost, Route("upload")]
        public bool CreateFireBrigade([FromBody] FireBrigade fb)
        {
            try { 
                IEnumerable<User> users = Authentication.Token.VerifyToken(Authentication.Token.GetTokenFromHeader(Request.Headers));
                if (users != null)
                {
                    if (users.First<User>().UserType == UserTypes.admin)
                    {
                        User user = users.First<User>();
                        Logging.Logger.Log("upsert", user.Id + "(" + user.FirstName + ", " + user.LastName + ")", fb);
                        return DatabaseOperations.FireBrigades.UpsertFireBrigade(fb);
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
        /// <returns>returns a csv file with all FireBrigades</returns>
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
                        byte[] file = FileOperations.FireBrigadeFiles.ExportToCSV(DatabaseOperations.FireBrigades.GetAllFireBrigades());
                        stream.Write(file, 0, file.Length);

                        stream.Position = 0;
                        result = new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new ByteArrayContent(stream.ToArray())
                        };
                        result.Content.Headers.ContentDisposition =
                            new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                            {
                                FileName = "FireBrigades.csv"
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
        /// Deletes the FireBrigade from the Database and Cache
        /// The assoziations with the users and FireAlarmSystems are also deleted
        /// </summary>
        /// <param name="id">the id of the FireBrigade you want to delete</param>
        /// <returns>returns true if FireBrigade was deleted from DB</returns>
        [HttpGet, Route("delete/{id}")]
        public bool DeleteFireBrigade(int id)
        {
            return DatabaseOperations.FireBrigades.DeleteFireBrigade(id);
        }

        /// <summary>
        /// Checks if an id is already used by another FireBrigade
        /// </summary>
        /// <param name="id">the id you want to check</param>
        /// <returns>returns true if id is not used by other FireBrigade or else a new id</returns>
        [HttpPost, Route("checkid/{id}")]
        public static int CheckId(int id)
        {
            return DatabaseOperations.FireBrigades.CheckId(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns a list of all FireBrigades</returns>
        [HttpGet, Route("all")]
        public FireBrigade[] All()
        {
            return (DatabaseOperations.FireBrigades.GetAllFireBrigades()).ToArray<FireBrigade>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">The id of the FireBrigade you are looking for</param>
        /// <returns>returns a FireBrigade with a matching id</returns>
        [HttpGet, Route("id/{id}")]
        public FireBrigade[] GetFireBrigadeById(int id)
        {
            return DatabaseOperations.FireBrigades.GetFireBrigadeById(id).ToArray();
        }
    }
}
