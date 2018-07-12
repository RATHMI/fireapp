using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FireApp.Domain;

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

        /// <summary>
        /// Checks if an id is already used by another FireAlarmSystem
        /// </summary>
        /// <param name="id">the id you want to check</param>
        /// <returns>returns true if id is not used by other FireAlarmSystem</returns>
        [HttpPost, Route("checkid/{id}")]
        public bool CheckId(int id)
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
        /// Adds a ServiceMember to the list of ServiceMembers of a FireAlarmSystem
        /// </summary>
        /// <param name="id">identifier of the FireAlarmSystem</param>
        /// <param name="serviceMember">identifier of the ServiceMember</param>
        /// <returns>returns true if the ServiceMember was added</returns>
        [HttpGet, Route("addservicemember/{id}/{servicemember}")]
        public bool AddServiceMemberToFireAlarmSystem(int id, int serviceMember)
        {
            return DatabaseOperations.FireAlarmSystems.AddServiceMember(id, serviceMember);
        }
    }
}
