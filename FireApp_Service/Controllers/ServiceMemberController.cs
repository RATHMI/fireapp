using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FireApp.Domain;

namespace FireApp.Service.Controllers
{
    [RoutePrefix("service")]
    public class ServiceMemberController : ApiController
    {
        /// <summary>
        /// inserts a ServiceMember into the database or updates it if it already exists
        /// </summary>
        /// <param name="sm">The ServiceMember you want to insert</param>
        /// <returns>returns true if ServiceMember was inserted</returns>
        [HttpPost, Route("upload")]
        public bool UpsertServiceMember(ServiceMember sm)
        {
            return DatabaseOperations.ServiceMembers.UpsertServiceMember(sm);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns a list with all ServiceMembers</returns>
        [HttpGet, Route("all")]
        public ServiceMember[] GetAllServiceMembers()
        {
            return DatabaseOperations.ServiceMembers.GetAllServiceMembers().ToArray<ServiceMember>();
        }

        /// <summary>
        /// Checks if an id is already used by another ServiceMember
        /// </summary>
        /// <param name="id">the id you want to check</param>
        /// <returns>returns true if id is not used by other ServiceMember</returns>
        [HttpPost, Route("checkid/{id}")]
        public static bool CheckId(int id)
        {
            return DatabaseOperations.ServiceMembers.CheckId(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">The id of the ServiceMember you are looking for</param>
        /// <returns>returns a ServiceMember with a matching id</returns>
        [HttpGet, Route("id/{id}")]
        public ServiceMember[] GetServiceMemberById(int id)
        {
            return DatabaseOperations.ServiceMembers.GetServiceMemberById(id).ToArray<ServiceMember>();
        }
    }
}
