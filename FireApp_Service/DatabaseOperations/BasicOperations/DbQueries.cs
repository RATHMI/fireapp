﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;

namespace FireApp.Service.DatabaseOperations
{
    /// <summary>
    /// This class is for querying objects from the database.
    /// </summary>
    public static class DbQueries
    {
        /// <summary>
        /// Queries all FireEvents from the database.
        /// </summary>
        /// <returns>Returns all FireEvents from database.</returns>
        public static IEnumerable<FireEvent> QueryFireEvents()
        {
            //return LocalDatabase.GetAllFireEvents();
            return DatabaseOperations.LiteDB.LiteDbQueries.QueryFireEvents();
        }

        /// <summary>
        /// Queries all active FireEvents from the database.
        /// </summary>
        /// <returns>Returns all active FireEvents from the database.</returns>
        public static IEnumerable<FireEvent> QueryActiveFireEvents()
        {
            //return LocalDatabase.GetActiveFireEvents();
            return DatabaseOperations.LiteDB.LiteDbQueries.QueryActiveFireEvents();
        }

        /// <summary>
        /// Queries all FireAlarmSystems from the database.
        /// </summary>
        /// <returns>Returns a list with all FireAlarmSystems from database.</returns>
        public static IEnumerable<FireAlarmSystem> QueryFireAlarmSystems()
        {
            //return LocalDatabase.GetAllFireAlarmSystems();
            return DatabaseOperations.LiteDB.LiteDbQueries.QueryFireAlarmSystems();
        }

        /// <summary>
        /// Queries all FireBrigades from the database.
        /// </summary>
        /// <returns>Returns a list with all FireBrigades from database.</returns>
        public static IEnumerable<FireBrigade> QueryFireBrigades()
        {
            //return LocalDatabase.GetAllFireBrigades();
            return DatabaseOperations.LiteDB.LiteDbQueries.QueryFireBrigades();
        }

        /// <summary>
        /// Queries all ServiceGroups from the database.
        /// </summary>
        /// <returns>Returns a list with all ServiceGroups from database.</returns>
        public static IEnumerable<ServiceGroup> QueryServiceGroups()
        {
            //return LocalDatabase.GetAllServiceGroups();
            return DatabaseOperations.LiteDB.LiteDbQueries.QueryServiceGroups();
        }

        /// <summary>
        /// Queries all Users from the database.
        /// </summary>
        /// <returns>Returns a list with all Users from database.</returns>
        public static IEnumerable<User> QueryUsers()
        {
            //return LocalDatabase.GetAllUsers();
            return DatabaseOperations.LiteDB.LiteDbQueries.QueryUsers();
        }
    }
}