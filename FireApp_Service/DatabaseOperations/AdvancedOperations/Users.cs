using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;

namespace FireApp.Service.DatabaseOperations.AdvancedOperations
{
    public static class Users
    {       
        /// <summary>
        /// Returns all Users that have a UserType that is matching with a UserType from "usertypes".
        /// </summary>
        /// <param name="usertypes">An array of UserTypes.</param>
        /// <returns>Returns a list of all users with matching UserTypes.</returns>
        public static IEnumerable<User> GetByUserType(UserTypes[] usertypes)
        {
            List<User> results = new List<User>();
            foreach(UserTypes type in usertypes)
            {
                results.AddRange(GetByUserType(type));
            }

            return results;
        }

        /// <summary>
        /// Returns all Users that have a UserType that is matching with usertype.
        /// </summary>
        /// <param name="usertype">The UserType you want to filter.</param>
        /// <returns>Returns all Users with a UserType matching "usertype".</returns>
        public static IEnumerable<User> GetByUserType(UserTypes usertype) // todo: comment
        {
            List<User> results = new List<User>();
            foreach (User user in BasicOperations.Users.GetAll())
            {
                if (usertype == user.UserType)
                {
                    results.Add(user);
                }
            }

            return results;
        }

        /// <summary>
        /// Returns the first admin that is found in the database.
        /// </summary>
        /// <returns>Returns a User with Usertype admin.</returns>
        public static User GetFirstAdmin()
        {
            IEnumerable<User> admins = GetByUserType(UserTypes.admin);
            if(admins.Count() > 0)
            {
                return admins.First();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns all active Users.
        /// </summary>
        /// <returns>Returns a List of Users with a valid token.</returns>
        public static IEnumerable<User> GetActiveUsers()
        {
            List<User> results = new List<User>();
            foreach(User user in BasicOperations.Users.GetAll())
            {
                if (DateTime.Now < user.TokenCreationDate.AddDays(user.TokenValidDays))
                {
                    results.Add(user);
                }
            }

            return results;
        }

        /// <summary>
        /// Returns all inactive Users.
        /// </summary>
        /// <returns>Returns a List of Users with an invalid token.</returns>
        public static IEnumerable<User> GetInactiveUsers()
        {
            List<User> results = new List<User>();
            foreach (User user in BasicOperations.Users.GetAll())
            {
                if (DateTime.Now >= user.TokenCreationDate.AddDays(user.TokenValidDays))
                {
                    results.Add(user);
                }
            }

            return results;
        }

        /// <summary>
        /// Finds all authorized objects of the User depending on the UserType.
        /// </summary>
        /// <param name="user">The User you want the authorized objects from.</param>
        /// <returns>Returns a list of all authorized objects.</returns>
        public static IEnumerable<object> GetAuthorizedObjects(User user) //todo: comment
        {
            List<object> results = new List<object>();
            if (user != null)
            {
                // If the UserType is firealarmsystem.
                if (user.UserType == UserTypes.firealarmsystem)
                {
                    foreach (int id in user.AuthorizedObjectIds)
                    {
                        try
                        {
                            results.Add(BasicOperations.FireAlarmSystems.GetById(id));
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }
                }

                // If the UserType is firebrigade.
                if (user.UserType == UserTypes.firebrigade)
                {
                    foreach (int id in user.AuthorizedObjectIds)
                    {
                        try
                        {
                            results.Add(BasicOperations.FireBrigades.GetById(id));
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }
                }

                // If the UserType is servicemember
                if (user.UserType == UserTypes.servicemember)
                {
                    foreach (int id in user.AuthorizedObjectIds)
                    {
                        try
                        {
                            results.Add(BasicOperations.ServiceGroups.GetById(id));
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }
                }          
            }

            return results;
        }

        /// <summary>
        /// Returns all Users that contain the "id" in their AuthorizedObjectIds.
        /// </summary>
        /// <param name="id">The id of the authorized object.</param>
        /// <param name="type">The UserType of the User.</param>
        /// <returns>Returns a list of all Users where their AuthorizedObjectIds contain "id"</returns>
        public static IEnumerable<User> GetByAuthorizedObject(int id, UserTypes type) //todo: comment
        {
            IEnumerable<User> users = GetByUserType(type);
            List<User> results = new List<User>();

            foreach(User user in users)
            {
                if (user.AuthorizedObjectIds.Contains(id))
                {
                    results.Add(user);
                }
            }

            return results;
        }
        
    }
}