using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;

namespace FireApp.Service.DatabaseOperations
{
    public static class ServiceGroups
    {
        /// <summary>
        /// inserts a ServiceGroup into the database or updates it if it already exists
        /// </summary>
        /// <param name="sg">The ServiceGroup you want to insert</param>
        /// <returns>returns true if ServiceGroup was inserted</returns>
        public static bool UpsertServiceGroup(ServiceGroup sg)
        {
            if (sg != null)
            {
                LocalDatabase.UpsertServiceGroup(sg);
                return DatabaseOperations.DbUpserts.UpsertServiceGroup(sg);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if an id is already used by another ServiceGroup
        /// </summary>
        /// <param name="id">the id you want to check</param>
        /// <returns>returns true if id is not used by other ServiceGroup</returns>
        public static int CheckId(int id)
        {
            List<ServiceGroup> all = LocalDatabase.GetAllServiceGroups();
            int max = 0;
            foreach (ServiceGroup sg in all)
            {
                if(max < sg.Id)
                {
                    max = sg.Id;
                }
                if (sg.Id == id)
                {
                    return max + 1;
                }
            }
            return id;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns a list with all ServiceGroups</returns>
        public static IEnumerable<ServiceGroup> GetAllServiceGroups()
        {
            return (IEnumerable<ServiceGroup>)LocalDatabase.GetAllServiceGroups();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">The id of the ServiceGroup you are looking for</param>
        /// <returns>returns a ServiceGroup with a matching id</returns>
        public static IEnumerable<ServiceGroup> GetServiceGroupById(int id)
        {
            List<ServiceGroup> serviceGroups = LocalDatabase.GetAllServiceGroups();
            List<ServiceGroup> results = new List<ServiceGroup>();
            foreach (ServiceGroup sg in serviceGroups)
            {
                if (sg.Id == id)
                {
                    results.Add(sg);
                    break;
                }
            }

            return results;
        }
    }
}