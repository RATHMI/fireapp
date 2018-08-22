using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;
using static FireApp.Domain.User;
using FireApp.Domain.Extensionmethods;

namespace FireApp.Service.DatabaseOperations
{
    public static class FireAlarmSystems
    {
        /// <summary>
        /// Inserts a FireAlarmSystem into the database or updates it if it already exists.
        /// </summary>
        /// <param name="fas">The FireAlarmSystem you want to upsert.</param>
        /// <returns>Returns true if the FireAlarmSystem was inserted.</returns>
        public static bool Upsert(FireAlarmSystem fas, User user)
        {
            if (fas != null && fas.Id != 0)
            {
                bool ok = DatabaseOperations.DbUpserts.UpsertFireAlarmSystem(fas);
                if (ok)
                {
                    Logging.Logger.Log("upsert", user.GetUserDescription(), fas);                    
                }

                return ok;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Upserts a list of FireAlarmSystems into the database.
        /// </summary>
        /// <param name="fireAlarmSystems">The list of FireAlarmSystems you want to upsert.</param>
        /// <returns>Returns the number of upserted FireAlarmSystems.</returns>
        public static int BulkUpsert(IEnumerable<FireAlarmSystem> fireAlarmSystems, User user)
        {
            int upserted = 0;
            if (fireAlarmSystems != null)
            {
                foreach (FireAlarmSystem fas in fireAlarmSystems)
                {
                    if (Upsert(fas, user) == true)
                    {
                        upserted++;
                    }
                }
            }

            return upserted;
        }

        /// <summary>
        /// Checks if an id is already used by another FireAlarmSystem.
        /// </summary>
        /// <param name="id">The id you want to check.</param>
        /// <returns>Returns true if id is not used by other FireAlarmSystem.</returns>
        public static int CheckId(int id)
        {
            IEnumerable<FireAlarmSystem> all = GetAll();

            // The highest Id of all FireAlarmSystems.
            int maxId = 0;
            int rv = id;

            foreach (FireAlarmSystem fas in all)
            {
                if (maxId < fas.Id)
                {
                    maxId = fas.Id;
                }

                if (fas.Id == id)
                {
                    rv = -1;
                }
            }

            // If the id is already used by another FireAlarmSystem
            // return a new id.
            if(rv == -1 || id == 0)
            {
                rv = maxId + 1;
            }

            return rv;
        }

        /// <summary>
        /// Returns all FireAlarmSystems. 
        /// </summary>
        /// <returns>Returns a list of all FireAlarmSystems.</returns>
        public static IEnumerable<FireAlarmSystem> GetAll()
        {
            return DatabaseOperations.DbQueries.QueryFireAlarmSystems().OrderBy(x => x.Company);
        }

        /// <summary>
        /// Returns all FireAlarmSystems with a sourceId matching the sourceId of an active FireEvent.
        /// </summary>
        /// <returns>Returns a list of all FireAlarmSystems with active FireEvents.</returns>
        public static IEnumerable<FireAlarmSystem> GetActiveFireAlarmSystems(User user)
        {
            IEnumerable<FireEvent> events;

            // Get all active FireEvents from the database.
            events = DatabaseOperations.ActiveFireEvents.GetAll();

            // Filter the FireEvents according to the User.
            // If you do not filter the FireEvents the User will get some FireAlarmSystems 
            // but may not see the active FireEvents because of the UserType.
            events = Filter.FireEventsFilter.UserFilter(events, user);

            // HashSet not other type of list because there could be several FireEvents with the same sourceId.
            HashSet<FireAlarmSystem> results = new HashSet<FireAlarmSystem>();
            FireAlarmSystem fas;

            foreach(FireEvent fe in events)
            {
                try { 
                    // Get FireAlarmSystem with matching sourceId and add to results.
                    fas = GetById(fe.Id.SourceId);
                    results.Add(fas);
                }
                catch (Exception)
                {        
                    // If there is no FireAlarmSystem with a matching sourceId.
                    // E.g. if there are FireEvents, but no FireAlarmSystem in the database.          
                    continue;
                }
            }

            return results;
        }

        /// <summary>
        /// Returns the FireAlarmSystem with a matching id.
        /// </summary>
        /// <param name="id">The id of the FireAlarmSystem you are looking for.</param>
        /// <returns>Returns a FireAlarmSystem with a matching id.</returns>
        public static FireAlarmSystem GetById(int id)
        {
            IEnumerable<FireAlarmSystem> fireAlarmSystems = GetAll();
            foreach (FireAlarmSystem fas in fireAlarmSystems)
            {
                if (fas.Id == id)
                {
                    return fas;
                }
            }

            // If no FireAlarmSystem with a matching id was found.
            throw new KeyNotFoundException();
        }

        /// <summary>
        /// Returns a list of souceIds from FireEvents where there is no FireAlarmSystem with a matching Id.
        /// </summary>
        /// <returns>Returns a list of IDs.</returns>
        public static IEnumerable<int> GetUnregistered()
        {
            List<FireAlarmSystem> fireAlarmSystems = GetAll().OrderBy(x => x.Id).ToList();
            List<FireEvent> events = DatabaseOperations.FireEvents.GetAll().OrderBy(x => x.Id.SourceId).ToList();

            // Use a HashSet to prevent redundant entries.
            HashSet<int> results = new HashSet<int>();

            try
            {
                // Remove all FireEvents from the list where there is a FireAlarmSystem with a matching id.
                foreach (FireAlarmSystem fas in fireAlarmSystems)
                {
                    events.RemoveAll(x => x.Id.SourceId == fas.Id);
                }

                // "events" now contains all FireEvents with no matching FireAlarmSystem.

                // Add the sourceId of all remaining FireEvents to "results".
                foreach(FireEvent fe in events)
                {
                    results.Add(fe.Id.SourceId);
                }

                return results;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return results;
            }

        }

        /// <summary>
        /// Returns all FireAlarmSystems that are associated with the User.
        /// </summary>
        /// <param name="user">The User you want to get the FireAlarmSystems of.</param>
        /// <returns>Returns a list of FireAlarmSystems that are associated with a User.</returns>
        public static IEnumerable<FireAlarmSystem> GetByUser(User user)
        {           
            List<FireAlarmSystem> results = new List<FireAlarmSystem>();
            List<object> authorizedObjects = DatabaseOperations.Users.GetAuthorizedObjects(user).ToList();


            if (user.UserType == UserTypes.fireFighter)
            {                              
                foreach (FireBrigade fb in authorizedObjects)
                {
                    results.AddRange(DatabaseOperations.FireBrigades.GetFireAlarmSystems(fb.Id));
                }
            }

            if (user.UserType == UserTypes.servicemember)
            {                
                foreach (ServiceGroup sg in authorizedObjects)
                {
                    results.AddRange(DatabaseOperations.ServiceGroups.GetFireAlarmSystems(sg.Id));
                }
            }

            if(user.UserType == UserTypes.fireSafetyEngineer)
            {
                foreach(FireAlarmSystem fas in authorizedObjects)
                {
                    results.Add(fas);
                }
            }

            return results;
        }

        /// <summary>
        /// Returns a list of FireBrigades and ServiceGroups of this FireAlarmSystem.
        /// </summary>
        /// <param name="fas">The FireAlarmSystem with the Ids of the FireBrigades and ServiceGroups.</param>
        /// <returns>Returns a list of FireBrigades and ServiceGroups of this FireAlarmSystem.</returns>
        public static IEnumerable<object> GetMembers(FireAlarmSystem fas)
        {
            List<object> results = new List<object>();
      
            results.AddRange(DatabaseOperations.FireBrigades.GetByFireAlarmSystem(fas));
            results.AddRange(DatabaseOperations.ServiceGroups.GetByFireAlarmSystem(fas));

            return results;
        }

        /// <summary>
        /// Returns a list of FireBrigades or ServiceGroups of this FireAlarmSystem.
        /// </summary>
        /// <param name="fas">The FireAlarmSystem with the Ids of the FireBrigades and ServiceGroups.</param>
        /// <param name="type">The type of member you want (FireBrigde or ServiceGroup).</param>
        /// <returns>Returns a list of FireBrigades or ServiceGroups of this FireAlarmSystem.</returns>
        public static IEnumerable<object> GetMembers(FireAlarmSystem fas, Type type)
        {
            List<object> results = new List<object>();
           
            if(type == typeof(FireBrigade))
            {
                results.AddRange(DatabaseOperations.FireBrigades.GetByFireAlarmSystem(fas));
            }
            else
            {
                if(type == typeof(ServiceGroup))
                {
                    results.AddRange(DatabaseOperations.ServiceGroups.GetByFireAlarmSystem(fas));
                }
            }          

            return results;
        }

        /// <summary>
        /// Returns all Users that are associated with this FireAlarmSystem.
        /// </summary>
        /// <param name="fas">The FireAlarmSystem you want to get the Users of.</param>
        /// <returns>Returns all Users that are associated with this FireAlarmSystem.</returns>
        public static IEnumerable<User> GetUsers(FireAlarmSystem fas)
        {
            List<User> results = new List<User>();

            // Get all Users from the FireBrigades of the FireAlarmSystem.
            foreach(int firebrigade in fas.FireBrigades)
            {
                results.AddRange(DatabaseOperations.Users.GetByAuthorizedObject(firebrigade, UserTypes.fireFighter));
            }

            // Get all Users from the ServiceGroups of the FireAlarmSystem.
            foreach (int servicegroup in fas.ServiceGroups)
            {
                results.AddRange(DatabaseOperations.Users.GetByAuthorizedObject(servicegroup, UserTypes.servicemember));
            }

            // Get all Users from the FireAlarmSystem itself.
            results.AddRange(DatabaseOperations.Users.GetByAuthorizedObject(fas.Id, UserTypes.fireSafetyEngineer));

            return results;
        }

        /// <summary>
        /// Returns all Users of the given UserType that are associated with this FireAlarmSystem.
        /// </summary>
        /// <param name="fas">The FireAlarmSystem you want to get the Users of.</param>
        /// <param name="type">The UserType of Users.</param>
        /// <returns>Returns all Users of the given UserType that are associated with this FireAlarmSystem.</returns>
        public static IEnumerable<User> GetUsers(FireAlarmSystem fas, UserTypes type)
        {
            List<User> results = new List<User>();

            if (type == UserTypes.fireFighter)
            {
                foreach (int firebrigade in fas.FireBrigades)
                {
                    results.AddRange(DatabaseOperations.Users.GetByAuthorizedObject(firebrigade, UserTypes.fireFighter));
                }
            }else
            {
                if (type == UserTypes.servicemember)
                {
                    foreach (int servicegroup in fas.ServiceGroups)
                    {
                        results.AddRange(DatabaseOperations.Users.GetByAuthorizedObject(servicegroup, UserTypes.servicemember));
                    }
                }
                else
                {
                    if(type == UserTypes.fireSafetyEngineer)
                    {
                        results.AddRange(DatabaseOperations.Users.GetByAuthorizedObject(fas.Id, UserTypes.fireSafetyEngineer));
                    }
                }
            }

            return results;
        }
    }
}