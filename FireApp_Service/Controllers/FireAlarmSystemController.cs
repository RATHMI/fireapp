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
            return DatabaseOperations.FireAlarmSystems.UpsertFireAlarmSystem(fas);
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
            return (DatabaseOperations.FireAlarmSystems.GetAllFireAlarmSystems()).ToArray<FireAlarmSystem>();
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
            IEnumerable<User> user = Authentication.Token.VerifyToken(Authentication.Token.GetTokenFromHeader(Request.Headers));
            if (user != null)
            {
                return Filter.FireAlarmSystemsFilter.UserFilter((DatabaseOperations.FireAlarmSystems.GetActiveFireAlarmSystems(user.First<User>())), user.First<User>()).ToArray<FireAlarmSystem>();
            }
            else
            {
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
            return DatabaseOperations.FireAlarmSystems.GetFireAlarmSystemById(id).ToArray<FireAlarmSystem>();
        }

        /// <summary>
        /// Adds a FireBrigade to the list of FireBrigades of a FireAlarmSystem
        /// </summary>
        /// <param name="id">identifier of the FireAlarmSystem</param>
        /// <param name="firebrigade">identifier of the firebrigade</param>
        /// <returns>returns true if the FireBrigade was added</returns>
        [HttpGet, Route("addfirebrigade/{id}/{firebrigade}")]
        public bool AddFireBrigadeToFireAlarmSystem(int id, int firebrigade)
        {
            return DatabaseOperations.FireAlarmSystems.AddFireBrigade(id, firebrigade);
        }

        /// <summary>
        /// Adds a ServiceGroup to the list of ServiceGroups of a FireAlarmSystem
        /// </summary>
        /// <param name="id">identifier of the FireAlarmSystem</param>
        /// <param name="serviceGroup">identifier of the ServiceGroup</param>
        /// <returns>returns true if the ServiceGroup was added</returns>
        [HttpGet, Route("addservicemember/{id}/{servicemember}")]
        public bool AddServiceGroupToFireAlarmSystem(int id, int serviceGroup)
        {
            return DatabaseOperations.FireAlarmSystems.AddServiceGroup(id, serviceGroup);
        }
    }
}
