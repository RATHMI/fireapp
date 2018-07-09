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
            return DatabaseOperations.UploadFireAlarmSystem(fas);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns a list with all FireAlarmSystems</returns>
        [HttpGet, Route("all")]
        public FireAlarmSystem[] All()
        {
            return (DatabaseOperations.GetAllFireAlarmSystems()).ToArray<FireAlarmSystem>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">The id of the FireAlarmSystem you are looking for</param>
        /// <returns>returns a FireAlarmSystem with a matching id</returns>
        [HttpGet, Route("id/{id}")]
        public FireAlarmSystem GetFireAlarmSystemById(int id)
        {
            return DatabaseOperations.GetFireAlarmSystemById(id);
        }


    }
}
