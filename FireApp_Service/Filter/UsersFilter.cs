using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;

namespace FireApp.Service.Filter
{
    /// <summary>
    /// This class provide methods to filter Users by their properties and the UserType.
    /// </summary>
    public static class UsersFilter
    {
        /// <summary>
        /// Filters the list of Users and creates clones of them with censored password and token.
        /// </summary>
        /// <param name="users">A list of Users you want to filter.</param>
        /// <param name="user">The User that uses the filter.</param>
        /// <returns>Returns a filtered list of Users.</returns>
        public static IEnumerable<User> UserFilter(IEnumerable<User> users, User user) //todo: comment
        {
            List<User> results = new List<User>();
            if (users != null && user != null)
            {              
                if (user.UserType == UserTypes.admin)
                {
                    foreach(User u in adminFilter(users))
                    {
                        results.Add(u);
                    }
                }
                else
                {
                    // If the User is a FireAlarmSystem show all Users that have an authorized object id 
                    // matching one of its authorized object ids and all ServiceMembers of the FireAlarmSystems.
                    if (user.UserType == UserTypes.firealarmsystem)
                    {
                        results.AddRange(fireAlarmSystemFilter(users, user));
                    }
                    else
                    {
                        // If the User is a FireBrigade show all Users of the same FireBrigade.
                        if (user.UserType == UserTypes.firebrigade)
                        {
                            results.AddRange(fireBrigadeFilter(users, user));
                        }
                        else
                        {
                            // If the User is a ServiceMember show all Users of the same ServiceGroup and all Users of the 
                            // FireAlarmSystems that are connected to its ServiceGroups.
                            if (user.UserType == UserTypes.servicemember)
                            {
                                results.AddRange(serviceGroupFilter(users, user));
                            }
                        }
                    }                                                          
                }

                if (results.Exists(x => x.Id == user.Id))
                {
                    // If the User is contained in the result show more information about it.
                    // Remove it from the list of Users so it is not redundant in the result.
                    results.Remove(results.Find(x => x.Id == user.Id));
                    results.AddRange(adminFilter(new User[] { user }));                  
                }
            }

            return results
                .OrderBy(x => x.UserType)
                .ThenBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .Distinct();
        }

        /// <summary>
        /// Returns a cloned list of users with censored password and token.
        /// </summary>
        /// <param name="users">A list of Users you want to filter.</param>
        /// <returns>Returns the filtered list.</returns>
        private static IEnumerable<User> adminFilter(IEnumerable<User> users)
        {
            if (users != null)
            {
                List<User> results = new List<User>();
                foreach (User user in users)
                {
                    if (user != null)
                    {
                        //Clone needs to be a deep clone to avoid changes in the original.
                        User u = (User)user.Clone();
                        u.Token = null;
                        u.Password = null;
                        results.Add(u);
                    }          
                }

                return results;
            }
            else
            {
                return null;
            }
        }       

        private static IEnumerable<User> fireAlarmSystemFilter(IEnumerable<User> users, User user) // todo: comment
        {
            HashSet<User> result = new HashSet<User>();
            FireAlarmSystem fas;

            // If the User is a FireAlarmSystem show all Users that have an authorized object id 
            // matching one of its authorized object ids and all ServiceMembers of the FireAlarmSystems.

            // Get all Users of the FireAlarmSystems the User is allowed to see.
            foreach (User u in users)
            {
                if (u.UserType == UserTypes.firealarmsystem)
                {
                    foreach (int authobject in u.AuthorizedObjectIds)
                    {
                        if (user.AuthorizedObjectIds.Contains(authobject))
                        {
                            result.Add(u.SafeClone());
                        }
                    }
                }
            }

            // Get all ServiceMembers of the FireAlarmSystems.
            foreach(int authobject in user.AuthorizedObjectIds)
            {
                try
                {
                    fas = DatabaseOperations.FireAlarmSystems.GetById(authobject);

                    foreach(User u in DatabaseOperations.FireAlarmSystems.GetUsers(fas, UserTypes.servicemember))
                    {
                        if (users.Contains(u))
                        {
                            result.Add(u.SafeClone());
                        }
                    }
                }
                catch(Exception)
                {
                    continue;
                }
            }

            return result;
        }

        private static IEnumerable<User> fireBrigadeFilter(IEnumerable<User> users, User user) // todo: comment
        {
            HashSet<User> result = new HashSet<User>();

            // If the User is a FireBrigade show all Users of the same FireBrigade.
            foreach (User u in users)
            {
                if (u.UserType == UserTypes.firebrigade) {
                    foreach (int authobject in u.AuthorizedObjectIds)
                    {
                        if (user.AuthorizedObjectIds.Contains(authobject)){
                            result.Add(u.SafeClone());
                        }
                    }
                }
            }

            return result;
        }

        private static IEnumerable<User> serviceGroupFilter(IEnumerable<User> users, User user)// todo: comment
        {
            HashSet<User> result = new HashSet<User>();
   
            // If the User is a ServiceMember show all Users of the same ServiceGroup and all Users of the 
            // FireAlarmSystems that are connected to its ServiceGroups.
            

            // Get all Users of the same ServiceGroups as user.
            foreach(User u in users)
            {
                if (u.UserType == UserTypes.servicemember)
                {
                    foreach (int authobject in u.AuthorizedObjectIds)
                    {
                        if (user.AuthorizedObjectIds.Contains(authobject))
                        {
                            result.Add(u.SafeClone());
                        }
                    }
                }
            }

            // Get all FireAlarmSystems where the Users ServiceGroups are in the list of ServiceGroups.
            HashSet<FireAlarmSystem> fireAlarmSystems = new HashSet<FireAlarmSystem>();
            foreach(int authobject in user.AuthorizedObjectIds)
            {
                foreach (FireAlarmSystem fas in DatabaseOperations.ServiceGroups.GetFireAlarmSystems(authobject))
                {
                    fireAlarmSystems.Add(fas);
                }
            }

            // Get all Users of the FireAlarmSystems the User is authorized for.
            foreach(FireAlarmSystem fas in fireAlarmSystems)
            {
                // For each User of the FireAlarmSystem.
                foreach(User u in DatabaseOperations.FireAlarmSystems.GetUsers(fas, UserTypes.firealarmsystem))
                {
                    // If the User is contained in users add it to the result.
                    if (users.Contains(u))
                    {
                        result.Add(u);
                    }
                }
            }

            return result;
        }
    }
}