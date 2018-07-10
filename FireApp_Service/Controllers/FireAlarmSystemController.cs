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
        /// 
        /// </summary>
        /// <returns>returns a list with all FireAlarmSystems</returns>
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
        public FireAlarmSystem GetFireAlarmSystemById(int id)
        {
            return DatabaseOperations.FireAlarmSystems.GetFireAlarmSystemById(id);
        }

        /// <summary>
        /// Adds a FireBrigade to the list of FireBrigades of a FireAlarmSystem
        /// </summary>
        /// <param name="id">identifier of the FireAlarmSystem</param>
        /// <param name="firebrigade">identifier of the firebrigade</param>
        /// <returns>returns true if the FireBrigade was added or already is in the list</returns>
        [HttpGet, Route("{id}/addfirebrigade/{firebrigade}")]
        public bool AddFireBrigadeToFireAlarmSystem(int id, int firebrigade)
        {
            return DatabaseOperations.FireAlarmSystems.AddFireBrigadeToFireAlarmSystem(id, firebrigade);
        }

        
    }
}
