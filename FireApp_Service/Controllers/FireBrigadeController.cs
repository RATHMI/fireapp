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
    [RoutePrefix("fb")] //todo: comment
    public class FireBrigadeController : ApiController
    {
        /// <summary>
        /// inserts a FireBrigade into the database or updates it if it already exists
        /// </summary>
        /// <param name="fb"></param>
        /// <returns>returns true if the insert was successful</returns>
        [HttpPost, Route("upload")]//todo: comment
        public bool UploadFireBrigade([FromBody] FireBrigade fb)
        {
            try {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    if (user.UserType == UserTypes.admin)
                    {
                        Logging.Logger.Log("upsert", user.GetUserDescription(), fb);
                        return DatabaseOperations.FireBrigades.Upsert(fb);
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
        /// inserts an array of FireBrigades into the database or updates it if it already exists
        /// </summary>
        /// <param name="fb">The FireBrigades you want to insert</param>
        /// <returns>returns the number of upserted FireBrigades.
        /// -1 : invalid or no token
        /// -2 : user is not an admin
        /// -3 : an error occurred</returns>
        [HttpPost, Route("uploadbulk")]
        public int UpsertBulk([FromBody] FireBrigade[] fb)
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
                        return DatabaseOperations.FireBrigades.BulkUpsert(fb);
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
        /// <returns>returns a csv file with all FireBrigades</returns>
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
                        byte[] file = FileOperations.FireBrigadeFiles.ExportToCSV(DatabaseOperations.FireBrigades.GetAll());
                        stream.Write(file, 0, file.Length);

                        stream.Position = 0;
                        result = new HttpResponseMessage(HttpStatusCode.OK);
                        result.Content = new ByteArrayContent(stream.ToArray());
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
        [HttpGet, Route("delete/{id}")]//todo: comment
        public bool DeleteFireBrigade(int id)
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    if (user.UserType == UserTypes.admin)
                    {
                        FireBrigade old = DatabaseOperations.FireBrigades.GetById(id);
                        Logging.Logger.Log("delete", user.GetUserDescription(), old);
                        return DatabaseOperations.FireBrigades.Delete(id);
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
        /// Checks if an id is already used by another FireBrigade
        /// </summary>
        /// <param name="id">the id you want to check</param>
        /// <returns>returns true if id is not used by other FireBrigade or else a new id</returns>
        [HttpPost, Route("checkid/{id}")]//todo: comment
        public static int CheckId(int id)
        {
            return DatabaseOperations.FireBrigades.CheckId(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns a list of all FireBrigades</returns>
        [HttpGet, Route("all")]//todo: comment
        public FireBrigade[] All()
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    IEnumerable<FireBrigade> fb;
                    fb = DatabaseOperations.FireBrigades.GetAll();
                    fb = Filter.FireBrigadesFilter.UserFilter(fb, user);
                    return fb.ToArray();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new FireBrigade[0];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">The id of the FireBrigade you are looking for</param>
        /// <returns>returns a FireBrigade with a matching id</returns>
        [HttpGet, Route("id/{id}")]//todo: comment
        public FireBrigade[] GetFireBrigadeById(int id)
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    IEnumerable<FireBrigade> fb;
                    fb = new List<FireBrigade> { DatabaseOperations.FireBrigades.GetById(id) };
                    fb = Filter.FireBrigadesFilter.UserFilter(fb, user);
                    return fb.ToArray();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new FireBrigade[0];
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
                        return DatabaseOperations.FireBrigades.GetUsers(id).ToArray();
                    }
                    else
                    {
                        throw new Exception();
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
                return new User[0];
            }
        }
    }
}
