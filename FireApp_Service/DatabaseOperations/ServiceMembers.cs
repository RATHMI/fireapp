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

            using (var db = AppData.ServiceMemberDB())
            {
                var table = db.ServiceMemberTable();
                return table.Upsert(sm);
            }
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
            if (serviceMembers != null)
            {
                return serviceMembers.FindAll(x => x.Id == id);
            }
            else
            {
                return null;
            }
        }  
    }
}