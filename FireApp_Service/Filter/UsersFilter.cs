using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;

namespace FireApp.Service.Filter
{
    //todo: implement class
    public static class UsersFilter
    {
        public static IEnumerable<User> UserFilter(IEnumerable<User> users, User user)
        {
            List<User> results = new List<User>();
            if (users != null && user != null)
            {
                if (user.UserType == UserTypes.admin)
                {
                    results = adminFilter(users).ToList<User>();
                }
                else
                {
                    results.AddRange(userFilter(users, user));
                }
            }

            return (IEnumerable<User>)results.OrderBy(x => x.UserType).ThenBy(x => x.LastName).ThenBy(x => x.FirstName);
        }

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

        private static IEnumerable<User> userFilter(IEnumerable<User> users, User user)
        {
            if (users != null && user != null)
            {
                List<User> results = new List<User>();
                foreach (User u in users)
                {
                    if (u.Id == user.Id)
                    {
                        results.Add(getClone(u));
                        break;
                    }
                }

                return (IEnumerable<User>)results;
            }
            else
            {
                return null;
            }
        }

        
        private static User getClone(User user)
        {
            if (user != null)
            {
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