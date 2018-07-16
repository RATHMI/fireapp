using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;

namespace FireApp.Service.DatabaseOperations
{
    public static class ServiceMembers
    {
        /// <summary>
        /// inserts a ServiceMember into the database or updates it if it already exists
        /// </summary>
        /// <param name="sm">The ServiceMember you want to insert</param>
        /// <returns>returns true if ServiceMember was inserted</returns>
        public static bool UpsertServiceMember(ServiceMember sm)
        {
            LocalDatabase.UpsertServiceMember(sm);

            return DatabaseOperations.DbUpserts.UpsertServiceMember(sm);
        }

        /// <summary>
        /// Checks if an id is already used by another ServiceMember
        /// </summary>
        /// <param name="id">the id you want to check</param>
        /// <returns>returns true if id is not used by other ServiceMember</returns>
        public static bool CheckId(int id)
        {
            List<ServiceMember> all = LocalDatabase.GetAllServiceMembers();
            foreach (ServiceMember sm in all)
            {
                if (sm.Id == id)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns a list with all ServiceMembers</returns>
        public static IEnumerable<ServiceMember> GetAllServiceMembers()
        {
            return (IEnumerable<ServiceMember>)LocalDatabase.GetAllServiceMembers();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">The id of the ServiceMember you are looking for</param>
        /// <returns>returns a ServiceMember with a matching id</returns>
        public static IEnumerable<ServiceMember> GetServiceMemberById(int id)
        {
            List<ServiceMember> serviceMembers = LocalDatabase.GetAllServiceMembers();
            List<ServiceMember> results = new List<ServiceMember>();
            foreach (ServiceMember sm in serviceMembers)
            {
                if (sm.Id == id)
                {
                    results.Add(sm);
                    break;
                }
            }

            return results;
        }
    }
}