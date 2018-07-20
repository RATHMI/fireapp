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
        /// upserts a list of ServiceGroup into the database
        /// </summary>
        /// <param name="serviceGroups">The list of ServiceGroups you want to insert</param>
        /// <returns>returns the number of upserted ServiceGroups</returns>
        public static int UpsertServiceGroups(IEnumerable<ServiceGroup> serviceGroups)
        {
            int upserted = 0;
            if (serviceGroups != null)
            {
                foreach (ServiceGroup sg in serviceGroups)
                {
                    UpsertServiceGroup(sg);           
                    upserted++;
                }
            }

            return upserted;
        }

        /// <summary>
        /// Deletes the ServiceGroup from the Database and Cache
        /// The assoziations with the users and FireAlarmSystems are also deleted
        /// </summary>
        /// <param name="id">the id of the ServiceGroup you want to delete</param>
        /// <returns>returns true if ServiceGroup was deleted from DB</returns>
        public static bool DeleteServiceGroup(int id)
        {
            bool rv = false;

            // delete from DB
            rv = DatabaseOperations.DbDeletes.DeleteServiceGroup(id);
            if (rv != true)
            {
                // delete local
                LocalDatabase.DeleteServiceGroup(id);

                // delete from authorizedObjectIds of users local
                foreach (User u in DatabaseOperations.Users.GetAllUsers())
                {
                    if (u.UserType == UserTypes.servicemember && u.AuthorizedObjectIds.Contains(id))
                    {
                        u.AuthorizedObjectIds.Remove(id);
                        DatabaseOperations.Users.UpsertUser(u);
                    }
                }

                // delete from List of ServiceGroups of FireAlarmSystems local
                foreach (FireAlarmSystem fas in DatabaseOperations.FireAlarmSystems.GetAllFireAlarmSystems())
                {
                    if (fas.ServiceGroups.Contains(id))
                    {
                        fas.ServiceGroups.Remove(id);
                        DatabaseOperations.FireAlarmSystems.UpsertFireAlarmSystem(fas);
                    }
                }

                // delete from authorizedObjectIds of users in DB
                foreach (User u in DatabaseOperations.DbQueries.QueryUsers())
                {
                    if (u.UserType == UserTypes.servicemember && u.AuthorizedObjectIds.Contains(id))
                    {
                        u.AuthorizedObjectIds.Remove(id);
                        DatabaseOperations.DbUpserts.UpsertUser(u);
                    }
                }

                // delete from List of ServiceGroups of FireAlarmSystems in DB
                foreach (FireAlarmSystem fas in DatabaseOperations.DbQueries.QueryFireAlarmSystems())
                {
                    if (fas.ServiceGroups.Contains(id))
                    {
                        fas.ServiceGroups.Remove(id);
                        DatabaseOperations.DbUpserts.UpsertFireAlarmSystem(fas);
                    }
                }

            }
            return rv;
        }

        /// <summary>
        /// Checks if an id is already used by another ServiceGroup
        /// </summary>
        /// <param name="id">the id you want to check</param>
        /// <returns>returns true if id is not used by other ServiceGroup</returns>
        public static int CheckId(int id)
        {
            IEnumerable<ServiceGroup> all = LocalDatabase.GetAllServiceGroups();
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
        public static ServiceGroup GetServiceGroupById(int id)
        {
            IEnumerable<ServiceGroup> serviceGroups = LocalDatabase.GetAllServiceGroups();
            foreach (ServiceGroup sg in serviceGroups)
            {
                if (sg.Id == id)
                {
                    return sg;
                }
            }

            throw new KeyNotFoundException();
        }
    }
}