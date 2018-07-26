﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;

namespace FireApp.Service.DatabaseOperations
{
    public static class Users
    {
        /// <summary>
        /// Inserts a User into the database or updates it if it already exists.
        /// </summary>
        /// <param name="user">The User you want to upsert.</param>
        /// <returns>Returns true if the User was inserted.</returns>
        public static bool Upsert(User user)
        {
            try
            {
                if (user != null)
                {
                    // Try to find an existing User.
                    User old = GetById(user.Id);
                    if(user.Email == null)
                    {
                        return false;
                    }

                    if (old == null)
                    {
                        Email.Email.WelcomeEmail(user);                       
                    }

                    if (user.Token == null)
                    {
                        if (old == null)
                        {
                            // If there is no existing User you have to generate a new token
                            // to guarantee a safe authentication.
                            user.Token = Authentication.Token.GenerateToken();
                        }
                        else
                        {
                            // If there is an existing user, copy the token so the User does not have to login again.
                            user.Token = old.Token;
                        }
                    }

                    if(user.Password != null)
                    {
                        // Encrypt password.
                        user.Password = Encryption.Encrypt.EncryptString(user.Password);
                    }


                    if (user.Password == null || user.Email == null)
                    {
                        if (old == null)
                        {
                            // The User should not be upserted if there is no password.
                            return false;
                        }
                        user.Password = old.Password;
                        user.Email = old.Email;
                    }                 

                    

                    // Save the User in the database.
                    LocalDatabase.UpsertUser(user);
                    return DatabaseOperations.DbUpserts.UpsertUser(user);
                }
                return false;
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Inserts a list of Users into the database or updates them if they already exist.
        /// </summary>
        /// <param name="users">A list of Users you want to upsert.</param>
        /// <returns>Returns the number of Users that were successfully upserted.</returns>
        public static int BulkUpsert(IEnumerable<User> users)
        {
            int upserted = 0;
            if (users != null)
            {
                foreach (User user in users)
                {
                    Upsert(user);
                    upserted++;
                }
            }
            return upserted;
        }

        /// <summary>
        /// Deletes a User from all databases.
        /// </summary>
        /// <param name="userName">The Id of the User you want to delete.</param>
        /// <returns>Returns true if the User was deleted.</returns>
        public static bool Delete(string userName)
        {
            if(userName != null)
            {
                LocalDatabase.DeleteUser(userName);
                return DatabaseOperations.DbDeletes.DeleteUser(userName);
            }
            return false;
        }

        /// <summary>
        /// Checks if an id is already used by another User.
        /// </summary>
        /// <param name="id">The id you want to check.</param>
        /// <returns>Returns true if the id is not used by another User.</returns>
        public static bool CheckId(string id)
        {
            IEnumerable<User> all = LocalDatabase.GetAllUsers();
            foreach (User user in all)
            {
                if (user.Id == id)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Returns all Users.
        /// </summary>
        /// <returns>Returns a list of all Users.</returns>
        public static IEnumerable<User> GetAll()
        {
            return LocalDatabase.GetAllUsers();
        }

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
        /// Returns all Users that have a UserType that is matching with "usertype".
        /// </summary>
        /// <param name="usertype">The UserType you want to filter.</param>
        /// <returns>Returns all Users with a UserType matching "usertype".</returns>
        public static IEnumerable<User> GetByUserType(UserTypes usertype)
        {
            List<User> results = new List<User>();
            foreach (User user in GetAll())
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
        /// Returns the User with a matching Username.
        /// </summary>
        /// <param name="username">The Username of the User you are looking for.</param>
        /// <returns>Returns a User with a matching Username.</returns>
        public static User GetById(string userName)
        {
            IEnumerable<User> users = GetAll();
            foreach (User u in users)
            {
                if (u.Id == userName)
                {
                    return u;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns all active Users.
        /// </summary>
        /// <returns>Returns a List of Users with a valid token.</returns>
        public static IEnumerable<User> GetActiveUsers()
        {
            List<User> results = new List<User>();
            foreach(User user in GetAll())
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
            foreach (User user in GetAll())
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
        public static IEnumerable<object> GetAuthorizedObjects(User user)
        {
            List<object> results = new List<object>();
            if (user != null)
            {
                // If the UserType is firealarmsystem.
                if (user.UserType == UserTypes.firealarmsystem)
                {
                    // Get all FireAlarmSystems that are contained in AuthorizedObjectIds.
                    foreach (int id in user.AuthorizedObjectIds)
                    {
                        try
                        {
                            results.Add(DatabaseOperations.FireAlarmSystems.GetById(id));
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
                    // Get all FireBrigades that are contained in AuthorizedObjectIds.
                    foreach (int id in user.AuthorizedObjectIds)
                    {
                        try
                        {
                            results.Add(DatabaseOperations.FireBrigades.GetById(id));
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
                    // Get all ServiceGroups that are contained in AuthorizedObjectIds.
                    foreach (int id in user.AuthorizedObjectIds)
                    {
                        try
                        {
                            results.Add(DatabaseOperations.ServiceGroups.GetById(id));
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
        public static IEnumerable<User> GetByAuthorizedObject(int id, UserTypes type)
        {
            // Get all Users with a UserType matching "type".
            IEnumerable<User> users = GetByUserType(type);

            List<User> results = new List<User>();

            // Get all Users whose AuthorizedObjectIds contains "id".
            foreach (User user in users)
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