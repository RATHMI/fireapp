using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;

namespace FireApp.Service.Filter
{
    /// <summary>
    /// This class provide methods to filter Users by their properties and the UserType
    /// </summary>
    public static class UsersFilter
    {
        /// <summary>
        /// filters the list of users and creates clones of them with censored password and token
        /// </summary>
        /// <param name="users">a list of Users you want to filter</param>
        /// <param name="user">the User that uses the filter</param>
        /// <returns>returns a filtered list of Users</returns>
        public static IEnumerable<User> UserFilter(IEnumerable<User> users, User user)
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
                else // todo: extend filter
                {
                    // If the User is a FireAlarmSystem show all Users that have an authorized object id 
                    // matching one of its authorized object ids and all ServiceMembers of the FireAlarmSystems.

                    // If the User is a ServiceMember show all Users of the same ServiceGroup and all Users of the 
                    // FireAlarmSystems that are connected to its ServiceGroups.

                    // If the User is a FireBrigade show all Users of the same FireBrigade

                                       
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

        private static IEnumerable<User> fireAlarmSystemFilter(IEnumerable<User> users)
        {

        }
    }
}