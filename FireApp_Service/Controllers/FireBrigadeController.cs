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
        /*******************************************************************************************
         * public bool CreateFireBrigade([FromBody] FireBrigade fb)
         * 
         * inserts a FireBrigade into the database or updates it if it already exists
         ******************************************************************************************/
        [HttpPost, Route("upload")]
        public bool CreateFireBrigade([FromBody] FireBrigade fb)
        {
            return DatabaseOperations.UploadFireBrigade(fb);
        }

        /*******************************************************************************************
         * public IEnumerable<FireBrigade> All()
         * 
         * returns a list with all FireBrigades
         ******************************************************************************************/
        [HttpGet, Route("all")]
        public IEnumerable<FireBrigade> All()
        {
            return DatabaseOperations.GetAllFireBrigades();
        }

        /*******************************************************************************************
         * public FireBrigade GetFireBrigadeById(int id)
         * 
         * return a FireBrigade with a matching id
         ******************************************************************************************/
        [HttpGet, Route("id/{id}")]
        public FireBrigade GetFireBrigadeById(int id)
        {
            return DatabaseOperations.GetFireBrigadeById(id);
        }
    }
}
