using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;

namespace FireApp.Service.DatabaseOperations.AdvancedOperations
{
    public static class FireAlarmSystems
    {
        /// <summary>
        /// Returns all FireAlarmSystems with a sourceId matching the sourceId of an active FireEvent.
        /// </summary>
        /// <returns>Returns a list of all FireAlarmSystems with active FireEvents.</returns>
        public static IEnumerable<FireAlarmSystem> GetActiveFireAlarmSystems(User user)
        {
            IEnumerable<FireEvent> events;

            // Get all active FireEvents from the database.
            events = BasicOperations.ActiveEvents.GetAll(); 

            // Filter the FireEvents according to the User.
            events = Filter.FireEventsFilter.UserFilter(events, user);

            // HashSet not other type of list because there could be several FireEvents with the same sourceId.
            HashSet<FireAlarmSystem> results = new HashSet<FireAlarmSystem>();
            FireAlarmSystem fas;

            foreach(FireEvent fe in events)
            {
                try { 
                    // Get FireAlarmSystem with matching sourceId and add to results.
                    fas = BasicOperations.FireAlarmSystems.GetById(fe.Id.SourceId);
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
        /// Returns a list of souceIds from FireEvents where there is no FireAlarmSystem with a matching Id.
        /// </summary>
        /// <returns>Returns a list of IDs.</returns>
        public static IEnumerable<int> GetUnregistered()
        {
            List<FireAlarmSystem> fireAlarmSystems = BasicOperations.FireAlarmSystems.GetAll().OrderBy(x => x.Id).ToList();
            List<FireEvent> events = BasicOperations.FireEvents.GetAll().OrderBy(x => x.Id.SourceId).ToList();

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
        public static IEnumerable<FireAlarmSystem> GetByUser(User user) //todo: comment
        {           
            List<FireAlarmSystem> results = new List<FireAlarmSystem>();
            List<object> authorizedObjects = Users.GetAuthorizedObjects(user).ToList();


            if (user.UserType == UserTypes.firebrigade)
            {                              
                foreach (FireBrigade fb in authorizedObjects)
                {
                    results.AddRange(FireBrigades.GetFireAlarmSystems(fb.Id));
                }
            }

            if (user.UserType == UserTypes.servicemember)
            {                
                foreach (ServiceGroup sg in authorizedObjects)
                {
                    results.AddRange(ServiceGroups.GetFireAlarmSystems(sg.Id));
                }
            }

            if(user.UserType == UserTypes.firealarmsystem)
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
        public static IEnumerable<object> GetMembers(FireAlarmSystem fas)//todo: comment
        {
            List<object> results = new List<object>();
      
            results.AddRange(FireBrigades.GetByFireAlarmSystem(fas));
            results.AddRange(ServiceGroups.GetByFireAlarmSystem(fas));

            return results;
        }

        /// <summary>
        /// Returns a list of FireBrigades or ServiceGroups of this FireAlarmSystem.
        /// </summary>
        /// <param name="fas">The FireAlarmSystem with the Ids of the FireBrigades and ServiceGroups.</param>
        /// <param name="type">The type of member you want (FireBrigde or ServiceGroup).</param>
        /// <returns>Returns a list of FireBrigades or ServiceGroups of this FireAlarmSystem.</returns>
        public static IEnumerable<object> GetMembers(FireAlarmSystem fas, Type type)//todo: comment
        {
            List<object> results = new List<object>();
           
            if(type == typeof(FireBrigade))
            {
                results.AddRange(FireBrigades.GetByFireAlarmSystem(fas));
            }
            else
            {
                if(type == typeof(ServiceGroup))
                {
                    results.AddRange(ServiceGroups.GetByFireAlarmSystem(fas));
                }
            }          

            return results;
        }

        /// <summary>
        /// Returns all Users that are associated with this FireAlarmSystem.
        /// </summary>
        /// <param name="fas">The FireAlarmSystem you want to get the Users of.</param>
        /// <returns>Returns all Users that are associated with this FireAlarmSystem.</returns>
        public static IEnumerable<User> GetUsers(FireAlarmSystem fas)//todo: comment
        {
            List<User> results = new List<User>();

            foreach(int firebrigade in fas.FireBrigades)
            {
                results.AddRange(Users.GetByAuthorizedObject(firebrigade, UserTypes.firebrigade));
            }

            foreach (int servicegroup in fas.ServiceGroups)
            {
                results.AddRange(Users.GetByAuthorizedObject(servicegroup, UserTypes.servicemember));
            }

            results.AddRange(Users.GetByAuthorizedObject(fas.Id, UserTypes.firealarmsystem));

            return results;
        }

        /// <summary>
        /// Returns all Users of the given UserType that are associated with this FireAlarmSystem.
        /// </summary>
        /// <param name="fas">The FireAlarmSystem you want to get the Users of.</param>
        /// <param name="type">The UserType of Users.</param>
        /// <returns>Returns all Users of the given UserType that are associated with this FireAlarmSystem.</returns>
        public static IEnumerable<User> GetUsers(FireAlarmSystem fas, UserTypes type)//todo: comment
        {
            List<User> results = new List<User>();

            if (type == UserTypes.firebrigade)
            {
                foreach (int firebrigade in fas.FireBrigades)
                {
                    results.AddRange(Users.GetByAuthorizedObject(firebrigade, UserTypes.firebrigade));
                }
            }else
            {
                if (type == UserTypes.servicemember)
                {
                    foreach (int servicegroup in fas.ServiceGroups)
                    {
                        results.AddRange(Users.GetByAuthorizedObject(servicegroup, UserTypes.servicemember));
                    }
                }
                else
                {
                    if(type == UserTypes.firealarmsystem)
                    {
                        results.AddRange(Users.GetByAuthorizedObject(fas.Id, UserTypes.firealarmsystem));
                    }
                }
            }

            return results;
        }
    }
}