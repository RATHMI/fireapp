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
                    results = adminFilter(users).ToList();
                }
                else
                {
                    results.Add(getClone(user));
                }
            }

            return (IEnumerable<User>)results
                .OrderBy(x => x.UserType)
                .ThenBy(x => x.LastName)
                .ThenBy(x => x.FirstName);
        }

        /// <summary>
        /// returns a cloned list of users with censored password and token
        /// </summary>
        /// <param name="users">a list of Users you want to filter</param>
        /// <returns>returns the filtered list</returns>
        private static IEnumerable<User> adminFilter(IEnumerable<User> users)
        {
            if (users != null)
            {
                List<User> results = new List<User>();
                foreach (User user in users)
                {
                    results.Add(getClone(user));
                }

                return results;
            }
            else
            {
                return null;
            }
        }       

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user">the user you want to clone</param>
        /// <returns>returns a censored clone of the user</returns>
        private static User getClone(User user)
        {
            if (user != null)
            {
                //Clone needs to be a deep clone to avoid changes in the original
                User u = (User)user.Clone();    
                u.Token = null;
                u.Password = null;
                return u;
            }
            else
            {
                return null;
            }
        }
    }
}