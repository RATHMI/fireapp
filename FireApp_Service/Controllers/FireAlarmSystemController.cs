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
        /*******************************************************************************************
         * public bool CreateFireAlarmSystem([FromBody] FireAlarmSystem fas)
         * 
         * inserts a FireAlarmSystem into the database or updates it if it already exists
         ******************************************************************************************/
        [HttpPost, Route("upload")]
        public bool CreateFireAlarmSystem([FromBody] FireAlarmSystem fas)
        {
            using (var db = AppData.FireAlarmSystemDB())
            {
                var table = db.FireAlarmSystemTable();
                return table.Upsert(fas);
            }
        }

        /*******************************************************************************************
         * public IEnumerable<FireAlarmSystem> All()
         * 
         * returns a list with all FireAlarmSystems
         ******************************************************************************************/
        [HttpGet, Route("all")]
        public IEnumerable<FireAlarmSystem> All()
        {
            using (var db = AppData.FireAlarmSystemDB())
            {
                var table = db.FireAlarmSystemTable();
                return table.FindAll();
            }
        }
    }
}
