using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FireApp.Domain;

namespace FireApp.Service.Controllers
{
    //todo: implement authorization
    [RoutePrefix("service")]
    public class ServiceGroupController : ApiController
    {
        /// <summary>
        /// inserts a ServiceGroup into the database or updates it if it already exists
        /// </summary>
        /// <param name="sg">The ServiceGroup you want to insert</param>
        /// <returns>returns true if ServiceGroup was inserted</returns>
        [HttpPost, Route("upload")]
        public bool UpsertServiceGroup(ServiceGroup sg)
        {
            return DatabaseOperations.ServiceGroups.UpsertServiceGroup(sg);
        }

        /// <summary>
        /// Deletes the ServiceGroup from the Database and Cache
        /// The assoziations with the users and FireAlarmSystems are also deleted
        /// </summary>
        /// <param name="id">the id of the ServiceGroup you want to delete</param>
        /// <returns>returns true if ServiceGroup was deleted from DB</returns>
        [HttpGet, Route("delete/{id}")]
        public bool DeleteServiceGroup(int id)
        {
            return DatabaseOperations.ServiceGroups.DeleteServiceGroup(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns a list with all ServiceGroups</returns>
        [HttpGet, Route("all")]
        public ServiceGroup[] GetAllServiceGroups()
        {
            return DatabaseOperations.ServiceGroups.GetAllServiceGroups().ToArray<ServiceGroup>();
        }

        /// <summary>
        /// Checks if an id is already used by another ServiceGroup
        /// </summary>
        /// <param name="id">the id you want to check</param>
        /// <returns>returns true if id is not used by other ServiceGroup or else a new id</returns>
        [HttpPost, Route("checkid/{id}")]
        public static int CheckId(int id)
        {
            return DatabaseOperations.ServiceGroups.CheckId(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">The id of the ServiceGroup you are looking for</param>
        /// <returns>returns a ServiceGroup with a matching id</returns>
        [HttpGet, Route("id/{id}")]
        public ServiceGroup[] GetServiceGroupById(int id)
        {
            return DatabaseOperations.ServiceGroups.GetServiceGroupById(id).ToArray<ServiceGroup>();
        }
    }
}
