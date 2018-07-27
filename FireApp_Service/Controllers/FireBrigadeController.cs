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
    [RoutePrefix("fb")] //todo: comment
    public class FireBrigadeController : ApiController
    {
        /// <summary>
        /// Inserts a FireBrigade into the database or updates it if it already exists
        /// </summary>
        /// <param name="fb">The FireBrigade you want to upsert.</param>
        /// <returns>Returns true if the FireBrigade was inserted.</returns>
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
                        return DatabaseOperations.FireBrigades.Upsert(fb, user);
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
        /// Inserts an array of FireBrigades into the database or updates it if it already exists.
        /// </summary>
        /// <param name="fb">The FireBrigades you want to insert.</param>
        /// <returns>Returns the number of upserted FireBrigades.
        /// -1 : invalid or no token.
        /// -2 : user is not an admin.
        /// -3 : an error occurred.</returns>
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
                        return DatabaseOperations.FireBrigades.BulkUpsert(fb, user);
                    }
                    else
                    {
                        // The User is not an admin.
                        return -2;
                    }
                }
                else
                {
                    // The User is not logged in.
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
        /// Returns all FireBrigades as CSV file.
        /// </summary>
        /// <returns>Returns a CSV file with all FireBrigades.</returns>
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

                        // Get all FireBrigades.
                        IEnumerable<FireBrigade> fb = DatabaseOperations.FireBrigades.GetAll();

                        // Convert FireBrigades into a CSV file.
                        byte[] file = FileOperations.FireBrigadeFiles.ExportToCSV(fb);

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
                                FileName = "FireBrigades.csv"
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
                    // User is not logged in.
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
        /// Retrieves FireBrigades from CSV and upserts them.
        /// </summary>
        /// <param name="bytes">An array of bytes that represents a CSV file.</param>
        /// <returns>The number of successfully upserted FireBrigades.</returns>
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
                        IEnumerable<FireBrigade> fb;
                        byteArrayString = byteArrayString.Trim('"');
                        List<byte> bytes = new List<byte>();
                        foreach(string s in byteArrayString.Split(' '))
                        {
                            bytes.Add(Convert.ToByte(s));
                        }

                        fb = FileOperations.FireBrigadeFiles.GetFireBrigadesFromCSV(bytes.ToArray());
                        int upserted = DatabaseOperations.FireBrigades.BulkUpsert(fb, user);

                        // sets the content of the response to the number of upserted users
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
        /// Deletes the FireBrigade from the Database and Cache.
        /// The assoziations with the users and FireAlarmSystems are also deleted.
        /// </summary>
        /// <param name="id">The id of the FireBrigade you want to delete.</param>
        /// <returns>Returns true if the FireBrigade was deleted from the DB.</returns>
        [HttpGet, Route("delete/{id}")]
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
                        // Delete the FireBrigade from the database.
                        return DatabaseOperations.FireBrigades.Delete(id, user);
                    }
                    else
                    {
                        // User is not an admin.
                        throw new Exception();
                    }
                }
                else
                {
                    // Notify user that the login was not successful.
                    throw new Exception();
                }                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }                       
        }

        /// <summary>
        /// Checks if an id is already used by another FireBrigade.
        /// </summary>
        /// <param name="id">The id you want to check.</param>
        /// <returns>Returns true if the id is not used by another FireBrigade or else a new id.</returns>
        [HttpPost, Route("checkid/{id}")]
        public static int CheckId(int id)
        {
            return DatabaseOperations.FireBrigades.CheckId(id);
        }

        /// <summary>
        /// Returns all FireBrigades the User is allowed to see.
        /// </summary>
        /// <returns>Returns an array of all FireBrigades.</returns>
        [HttpGet, Route("all")]
        public FireBrigade[] All()
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    IEnumerable<FireBrigade> fb;

                    // Get all FireBrigades.
                    fb = DatabaseOperations.FireBrigades.GetAll();

                    // Filter the FireBrigades according to the User.
                    fb = Filter.FireBrigadesFilter.UserFilter(fb, user);
                    return fb.ToArray();
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
                return new FireBrigade[0];
            }
        }

        /// <summary>
        /// Returns the FireBrigade with a matching id if the User is allowed to see it.
        /// </summary>
        /// <param name="id">The id of the FireBrigade you are looking for.</param>
        /// <returns>Returns a FireBrigade with a matching id.</returns>
        [HttpGet, Route("id/{id}")]
        public FireBrigade[] GetFireBrigadeById(int id)
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    IEnumerable<FireBrigade> fb;

                    // Get the FireBrigade.
                    fb = new List<FireBrigade> { DatabaseOperations.FireBrigades.GetById(id) };

                    // Only return FireBrigades the User is allowed to see.
                    fb = Filter.FireBrigadesFilter.UserFilter(fb, user);
                    return fb.ToArray();
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
                return new FireBrigade[0];
            }
        }

        /// <summary>
        /// Returns all Users that are associated with this FireBrigade.
        /// </summary>
        /// <param name="firebrigade">The FireBrigade you want to get the Users of.</param>
        /// <returns>Returns all Users that are associated with this FireBrigade.</returns>
        [HttpGet, Route("users/{id}")]
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
                        // Return all Users of the FireBrigade.
                        return DatabaseOperations.FireBrigades.GetUsers(id).ToArray();
                    }
                    else
                    {
                        // User is not an admin.
                        throw new Exception();
                    }
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
    }
}
