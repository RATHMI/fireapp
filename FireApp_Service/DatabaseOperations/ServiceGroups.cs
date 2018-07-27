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
        /// Inserts a ServiceGroup into the database or updates it if it already exists.
        /// </summary>
        /// <param name="sg">The ServiceGroup you want to insert.</param>
        /// <returns>Returns true if the ServiceGroup was inserted.</returns>
        public static bool Upsert(ServiceGroup sg, User user)
        {
            if (sg != null)
            {
                bool ok = DatabaseOperations.DbUpserts.UpsertServiceGroup(sg);
                if (ok)
                {
                    Logging.Logger.Log("upsert", user.GetUserDescription(), sg);
                    LocalDatabase.UpsertServiceGroup(sg);
                }

                return ok;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Upserts a list of ServiceGroup into the database.
        /// </summary>
        /// <param name="serviceGroups">The list of ServiceGroups you want to upsert.</param>
        /// <returns>Returns the number of upserted ServiceGroups.</returns>
        public static int BulkUpsert(IEnumerable<ServiceGroup> serviceGroups, User user)
        {
            int upserted = 0;
            if (serviceGroups != null)
            {
                foreach (ServiceGroup sg in serviceGroups)
                {
                    if (Upsert(sg, user) == true)
                    {
                        upserted++;
                    }
                }
            }

            return upserted;
        }

        /// <summary>
        /// Deletes the ServiceGroup from the database and cache.
        /// The assoziations with the Users and FireAlarmSystems are also deleted.
        /// </summary>
        /// <param name="id">The id of the ServiceGroup you want to delete.</param>
        /// <returns>Returns true if the ServiceGroup was deleted from the DB.</returns>
        public static bool Delete(int id, User user)
        {
            // Delete from database.
            bool ok = DatabaseOperations.DbDeletes.DeleteServiceGroup(id);
            if (ok)
            {
                // Write log message.
                ServiceGroup old = GetById(id);
                Logging.Logger.Log("delete", user.GetUserDescription(), old);

                // Delete from cache.
                LocalDatabase.DeleteServiceGroup(id);

                // Delete from authorizedObjectIds of Users from cache.
                foreach (User u in DatabaseOperations.Users.GetAll())
                {
                    if (u.UserType == UserTypes.servicemember && u.AuthorizedObjectIds.Contains(id))
                    {
                        u.AuthorizedObjectIds.Remove(id);
                        DatabaseOperations.Users.Upsert(u, user);
                    }
                }

                // Delete from List of ServiceGroups of FireAlarmSystems from cache.
                foreach (FireAlarmSystem fas in DatabaseOperations.FireAlarmSystems.GetAll())
                {
                    if (fas.ServiceGroups.Contains(id))
                    {
                        fas.ServiceGroups.Remove(id);
                        DatabaseOperations.FireAlarmSystems.Upsert(fas, user);
                    }
                }

                // Delete from authorizedObjectIds of Users in database.
                foreach (User u in DatabaseOperations.DbQueries.QueryUsers())
                {
                    if (u.UserType == UserTypes.servicemember && u.AuthorizedObjectIds.Contains(id))
                    {
                        u.AuthorizedObjectIds.Remove(id);
                        DatabaseOperations.DbUpserts.UpsertUser(u);
                    }
                }

                // Delete from List of ServiceGroups of FireAlarmSystems in database.
                foreach (FireAlarmSystem fas in DatabaseOperations.DbQueries.QueryFireAlarmSystems())
                {
                    if (fas.ServiceGroups.Contains(id))
                    {
                        fas.ServiceGroups.Remove(id);
                        DatabaseOperations.DbUpserts.UpsertFireAlarmSystem(fas);
                    }
                }

            }
            return ok;
        }

        /// <summary>
        /// Checks if an id is already used by another ServiceGroup.
        /// </summary>
        /// <param name="id">The id you want to check.</param>
        /// <returns>Returns true if the id is not used by another ServiceGroup.</returns>
        public static int CheckId(int id)
        {
            IEnumerable<ServiceGroup> all = LocalDatabase.GetAllServiceGroups();

            // The highest Id of all ServiceGroups.
            int maxId = 0;
            int rv = id;

            foreach (ServiceGroup sg in all)
            {
                if (maxId < sg.Id)
                {
                    maxId = sg.Id;
                }

                if (sg.Id == id)
                {
                    rv = -1;
                }
            }

            // If the id is already used by another ServiceGroup
            // return a new id.
            if (rv == -1)
            {
                rv = maxId + 1;
            }

            return rv;
        }

        /// <summary>
        /// Returns all ServiceGroups.
        /// </summary>
        /// <returns>Returns a list of all ServiceGroups.</returns>
        public static IEnumerable<ServiceGroup> GetAll()
        {
            return LocalDatabase.GetAllServiceGroups();
        }

        /// <summary>
        /// Returns the ServiceGroup with a matching id.
        /// </summary>
        /// <param name="id">The id of the ServiceGroup you are looking for.</param>
        /// <returns>Returns a ServiceGroup with a matching id.</returns>
        public static ServiceGroup GetById(int id)
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

        /// <summary>
        /// Returns all ServiceGroups that are in the list of ServiceGroups of the FireAlarmSystem.
        /// </summary>
        /// <param name="fas">The FireAlarmSystem you want to get the ServiceGroups of.</param>
        /// <returns>Returns all ServiceGroups that are assoziated with this FireAlarmSystem.</returns>
        public static IEnumerable<ServiceGroup> GetByFireAlarmSystem(FireAlarmSystem fas)
        {
            List<ServiceGroup> results = new List<ServiceGroup>();

            try
            {
                foreach (int id in fas.FireBrigades)
                {
                    results.Add(DatabaseOperations.ServiceGroups.GetById(id));
                }

                return results;
            }
            catch (Exception)
            {
                return new List<ServiceGroup>();
            }
        }

        /// <summary>
        /// Returns all Users that are associated with this ServiceGroup.
        /// </summary>
        /// <param name="servicegroup">The ServiceGroup you want to get the Users of.</param>
        /// <returns>Returns all Users whose AuthorizedObjectIds contains "servicegroup".</returns>
        public static IEnumerable<User> GetUsers(int servicegroup)
        {
            return DatabaseOperations.Users.GetByAuthorizedObject(servicegroup, UserTypes.servicemember);
        }

        /// <summary>
        /// Returns all FireAlarmSystems that are associated with this ServiceGroup.
        /// </summary>
        /// <param name="servicegroup">The ServiceGroup you want to get the FireAlarmSystems of.</param>
        /// <returns>Returns all FireAlarmSystems that are associated with this ServiceGroup.</returns>
        public static IEnumerable<FireAlarmSystem> GetFireAlarmSystems(int servicegroup)
        {
            List<FireAlarmSystem> results = new List<FireAlarmSystem>();

            // Find all FireAlarmSystems where the id of the FireAlarmSystem is contained
            // in the list of FireAlarmSystems.
            foreach (FireAlarmSystem fas in DatabaseOperations.FireAlarmSystems.GetAll())
            {
                // If the FireAlarmSystem contains the id of the ServiceGroup.
                if (fas.ServiceGroups.Contains(servicegroup))
                {
                    results.Add(fas);
                }
            }

            return results;
        }
    }
}