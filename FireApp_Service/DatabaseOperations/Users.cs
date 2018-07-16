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
        /// inserts a User into the database or updates it if it already exists
        /// </summary>
        /// <param name="user">The User you want to insert</param>
        /// <returns>returns true if User was inserted</returns>
        public static bool UpsertUser(User user)
        {
            if(user.Token == null)
            {
                user.Token = Authentication.Token.GenerateToken(user.Id.GetHashCode());
            }
            LocalDatabase.UpsertUser(user);

            return DatabaseOperations.DbUpserts.UpsertUser(user);
        }

        /// <summary>
        /// Checks if an id is already used by another User
        /// </summary>
        /// <param name="id">the id you want to check</param>
        /// <returns>returns true if id is not used by other User</returns>
        public static bool CheckId(string id)
        {
            List<User> all = LocalDatabase.GetAllUsers();
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
        /// 
        /// </summary>
        /// <returns>returns a list with all Users</returns>
        public static IEnumerable<User> GetAllUsers()
        {
            return (IEnumerable<User>)LocalDatabase.GetAllUsers();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username">The username of the User you are looking for</param>
        /// <returns>returns a User with a matching username</returns>
        public static IEnumerable<User> GetUserById(string userName)
        {
            List<User> users = LocalDatabase.GetAllUsers();
            List<User> results = new List<User>();
            foreach (User u in users)
            {
                if (u.Id == userName)
                {
                    results.Add(u);
                    break;
                }
            }

            return results;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns a List of Users with a valid token</returns>
        public static IEnumerable<User> GetActiveUsers()
        {
            List<User> results = new List<User>();
            foreach(User user in GetAllUsers().ToList<User>())
            {
                if (DateTime.Now < user.TokenCreationDate.AddDays(365))
                {
                    results.Add(user);
                }
            }

            return results;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns a List of Users with an invalid token</returns>
        public static IEnumerable<User> GetInactiveUsers()
        {
            List<User> results = new List<User>();
            foreach (User user in GetAllUsers().ToList<User>())
            {
                if (DateTime.Now >= user.TokenCreationDate.AddDays(365))
                {
                    results.Add(user);
                }
            }

            return results;
        }
    }
}