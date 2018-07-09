using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FireApp.Domain;

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
            return DatabaseOperations.UploadFireBrigade(fb);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns a list with all FireBrigades</returns>
        [HttpGet, Route("all")]
        public FireBrigade[] All()
        {
            return (DatabaseOperations.GetAllFireBrigades()).ToArray<FireBrigade>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">The id of the FireBrigade you are looking for</param>
        /// <returns>returns a FireBrigade with a matching id</returns>
        [HttpGet, Route("id/{id}")]
        public FireBrigade GetFireBrigadeById(int id)
        {
            return DatabaseOperations.GetFireBrigadeById(id);
        }
    }
}
