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
    [RoutePrefix("fas")]
    public class FireAlarmSystemController : ApiController
    {
        /// <summary>
        /// Inserts a FireAlarmSystem into the database or updates it if it already exists.
        /// </summary>
        /// <param name="fas">The FireAlarmSystem you want to upsert.</param>
        /// <returns>Returns true if the FireAlarmSystem was inserted.</returns>
        [HttpPost, Route("upload")]
        public bool UploadFireAlarmSystem([FromBody] FireAlarmSystem fas)
        {
            try {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    // Allow the admin and authorized Users to upsert a FireAlarmSystem.
                    if (user.UserType == UserTypes.admin || 
                        (user.UserType == UserTypes.fireSafetyEngineer && 
                        user.AuthorizedObjectIds.Contains(fas.Id)))
                    {
                        return DatabaseOperations.FireAlarmSystems.Upsert(fas, user);
                    }
                    else
                    {
                        // User is not an authorized.
                        throw new InvalidOperationException();
                    }
                }
                else
                {
                    // Token is invalid.
                    throw new InvalidOperationException();
                }
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
                        return DatabaseOperations.FireAlarmSystems.BulkUpsert(fas, user);
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

        /// <summary>
        /// Retrieves FireAlarmSystems from a CSV and upserts them.
        /// </summary>
        /// <param name="byteArrayString">An array of bytes as a string that represents a CSV file.</param>
        /// <returns>The number of successfully upserted FireAlarmSystems.</returns>
        [HttpPost, Route("uploadcsv")]
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
                        IEnumerable<FireAlarmSystem> fas;
                        List<byte> bytes = new List<byte>();

                        byteArrayString = byteArrayString.Trim('"');                
                        // Convert the string into an array of bytes.
                        foreach (string s in byteArrayString.Split(' '))
                        {
                            bytes.Add(Convert.ToByte(s));
                        }

                        // Get the FireAlarmSystems from the array of bytes.
                        fas = FileOperations.FireAlarmSystemFiles.GetFireAlarmSystemsFromCSV(bytes.ToArray());

                        // Upsert the FireAlarmSystems into the database.
                        int upserted = DatabaseOperations.FireAlarmSystems.BulkUpsert(fas, user);

                        // Sets the content of the response to the number of upserted FireAlarmSystems.
                        result.Content = new ByteArrayContent(Encoding.ASCII.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(upserted)));
                    }
                    else
                    {
                        // The User is not an admin.
                        result = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                        result.Content = null;
                    }
                }
                else
                {
                    // Notify the User that the login was not successful.
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
        /// Checks if an id is already used by another FireAlarmSystem.
        /// </summary>
        /// <param name="id">The id you want to check.</param>
        /// <returns>Returns id if id is not used by other FireAlarmSystem or else an unused id.</returns>
        [HttpGet, Route("checkid/{id}")]
        public int CheckId(int id)
        {
            return DatabaseOperations.FireAlarmSystems.CheckId(id);
        }

        /// <summary>
        /// Returns all FireAlarmSystems.
        /// </summary>
        /// <returns>Returns an array of all FireAlarmSystems.</returns>
        [HttpGet, Route("all")]
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
                    fas = DatabaseOperations.FireAlarmSystems.GetAll();

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

                        // Get all FireAlarmSystems.
                        IEnumerable<FireAlarmSystem> fas = DatabaseOperations.FireAlarmSystems.GetAll();

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
                        // The User is not an admin.
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
        /// Returns all FireAlarmSystems where there is an active FireEvent(that the User can see) with a matching sourceId.
        /// </summary>
        /// <returns>Returns an array of all FireAlarmSystems with active FireEvents.</returns>
        [HttpGet, Route("active")]
        public FireAlarmSystem[] GetActiveFireAlarmSystems()
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    IEnumerable<FireAlarmSystem> fas;

                    // Get all FireAlarmSystems with active FireEvents that the User can see.
                    fas = DatabaseOperations.FireAlarmSystems.GetActiveFireAlarmSystems(user);

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
        /// Returns a list of souceIds from FireEvents where there is no FireAlarmSystem with a matching Id.
        /// </summary>
        /// <returns>Returns a list of IDs.</returns>
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
        /// Returns the FireAlarmSystem with a matching id.
        /// </summary>
        /// <param name="id">The id of the FireAlarmSystem you are looking for.</param>
        /// <returns>Returns the FireAlarmSystem with a matching id.</returns>
        [HttpGet, Route("id/{id}")]
        public FireAlarmSystem[] GetFireAlarmSystemById(int id)
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    IEnumerable<FireAlarmSystem> fas;

                    // Get the FireAlarmSystem.
                    fas = new List<FireAlarmSystem> { DatabaseOperations.FireAlarmSystems.GetById(id) };

                    // Make sure the Users only sees what they are allowed to see.
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
        public object[] GetMembers(int id, string type)
        {
            List<object> results = new List<object>();

            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    
                    if (user.UserType == UserTypes.admin || 
                        (user.UserType == UserTypes.servicemember && user.AuthorizedObjectIds.Contains(id)))
                    {
                        FireAlarmSystem fas = DatabaseOperations.FireAlarmSystems.GetById(id);
                        if (type == "fb")
                        {
                            // Add all FireBrigades of the FireAlarmSystem to result.
                            results.AddRange(DatabaseOperations.FireAlarmSystems.GetMembers(fas, typeof(FireBrigade)));
                        }
                        else
                        {                           
                            if (type == "sg")
                            {
                                // Add all ServiceGroups of the FireAlarmSystem to result.
                                results.AddRange(DatabaseOperations.FireAlarmSystems.GetMembers(fas, typeof(ServiceGroup)));
                            }
                            else
                            {
                                // Add all ServiceGroups and FireBrigades of the FireAlarmSystem to result.
                                results.AddRange(DatabaseOperations.FireAlarmSystems.GetMembers(fas));
                            }
                        }

                        // Get all FireBrigades of the FireAlarmSystem.
                        List<FireBrigade> fb = new List<FireBrigade>();
                        List<ServiceGroup> sg = new List<ServiceGroup>();
                        foreach (object o in results)
                        {
                            if (o is FireBrigade)
                            {
                                fb.Add(o as FireBrigade);
                            }
                            else
                            {
                                if(o is ServiceGroup)
                                {
                                    sg.Add(o as ServiceGroup);
                                }
                            }
                        }

                        fb = Filter.FireBrigadesFilter.UserFilter(fb, user).ToList();
                        sg = Filter.ServiceGroupsFilter.UserFilter(sg, user).ToList();

                        results.Clear();
                        results.AddRange(fb);
                        results.AddRange(sg);
                        results.Distinct();

                        return results.ToArray();
                    }
                    else
                    {
                        // The User is not an admin.
                        throw new Exception();
                    }
                }
                else
                {
                    // Notify user that the login was not successful.
                    return null;
                }                
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
        public User[] GetUsers(int id, string type)
        {
            List<User> results = new List<User>();

            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    FireAlarmSystem fas = DatabaseOperations.FireAlarmSystems.GetById(id);

                    if (type == "fb")
                    {
                        results.AddRange(DatabaseOperations.FireAlarmSystems.GetUsers(fas, UserTypes.fireFighter));
                    }
                    else
                    {
                        if (type == "sg")
                        {
                            results.AddRange(DatabaseOperations.FireAlarmSystems.GetUsers(fas, UserTypes.servicemember));
                        }
                        else
                        {
                            if (type == "fas")
                            {
                                results.AddRange(DatabaseOperations.FireAlarmSystems.GetUsers(fas, UserTypes.fireSafetyEngineer));
                            }
                            else
                            {
                                results.AddRange(DatabaseOperations.FireAlarmSystems.GetUsers(fas));
                            }
                        }
                    }

                    results = Filter.UsersFilter.UserFilter(results, user).ToList();
                    results.Distinct();

                    return results.ToArray();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return new User[0];
            }
        }

        /// <summary>
        /// Adds or deletes an assoziation with FireBrigade or ServiceGroup.
        /// </summary>
        /// <param name="fireAlarmSystem">The id of the FireAlarmSytem you want to change a member of.</param>
        /// <param name="type">The type of member you want to change (fb or sg).</param>
        /// <param name="member">The id of the member you want to change.</param>
        /// <param name="operation">The type of operation you want to perform (add or delete).</param>
        /// <returns>
        /// Returns an error code.
        /// 1 : operation was successful.
        /// 0 : the login was not successful.
        /// -1 : the User is not an admin.
        /// -2 : the type of member or operation was not found.
        /// -3 : an error occured.
        /// </returns>
        [HttpGet, Route("changemember/{fireAlarmSystem}/{type}/{member}/{operation}")]
        public Int32 ChangeMember(int fireAlarmSystem, string type, int member, string operation)
        {
            try
            {
                User user;
                Authentication.Token.CheckAccess(Request.Headers, out user);
                if (user != null)
                {
                    if (user.UserType == UserTypes.admin)
                    {
                        if (type == null || operation == null)
                        {
                            throw new ArgumentNullException();
                        }
                        else
                        {
                            // Get the FireAlarmSystem by the id.
                            // The method throws an Exception if the FireAlarmSystem does not exist.
                            FireAlarmSystem fas = DatabaseOperations.FireAlarmSystems.GetById(fireAlarmSystem);            

                            if (type == "fb")
                            {
                                // Get the FireBrigade by the id.
                                // The method throws an Exception if the FireBrigade does not exist.
                                DatabaseOperations.FireBrigades.GetById(member);

                                if(operation == "add")
                                {
                                    fas.FireBrigades.Add(member);
                                    DatabaseOperations.FireAlarmSystems.Upsert(fas, user);
                                    return 1;
                                }
                                else
                                {
                                    if(operation == "delete")
                                    {
                                        fas.FireBrigades.Remove(member);
                                        DatabaseOperations.FireAlarmSystems.Upsert(fas, user);
                                        return 1;
                                    }
                                    else
                                    {
                                        throw new ArgumentOutOfRangeException();
                                    }
                                }
                                
                            }
                            else
                            {
                                if (type == "sg")
                                {
                                    // Get the ServiceGroup by the id.
                                    // The method throws an Exception if the ServiceGroup does not exist.
                                    DatabaseOperations.ServiceGroups.GetById(member);

                                    if (operation == "add")
                                    {
                                        fas.ServiceGroups.Add(member);
                                        DatabaseOperations.FireAlarmSystems.Upsert(fas, user);
                                        return 1;
                                    }
                                    else
                                    {
                                        if (operation == "delete")
                                        {
                                            fas.ServiceGroups.Remove(member);
                                            DatabaseOperations.FireAlarmSystems.Upsert(fas, user);
                                            return 1;
                                        }
                                        else
                                        {
                                            throw new ArgumentOutOfRangeException();
                                        }
                                    }
                                }
                                else
                                {
                                    throw new ArgumentOutOfRangeException();
                                }
                            }
                        }
                    }
                    else
                    {
                        // User is not an admin.
                        return -1;
                    }
                }
                else
                {
                    // Notify user that the login was not successful.
                    return 0;
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                return -2;
            }
            catch (Exception)
            {
                return -3;
            }
        }
    }
}
