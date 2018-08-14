using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;

namespace FireApp.Service.DatabaseOperations.LiteDB
{
    /// <summary>
    /// This class is for querying objects from the LiteDB.
    /// </summary>
    public static class LiteDbQueries
    {
        /// <summary>
        /// Queries all FireEvents from the LiteDB.
        /// </summary>
        /// <returns>Returns all FireEvents from database.</returns>
        public static IEnumerable<FireEvent> QueryFireEvents()
        {
            using (var db = AppData.FireEventDB())
            {
                var table = db.FireEventTable();
                return table.FindAll();
            }
        }

        /// <summary>
        /// Queries all active FireEvents from the LiteDB.
        /// </summary>
        /// <returns>Returns all active FireEvents from the database.</returns>
        public static IEnumerable<FireEvent> QueryActiveFireEvents()
        {
            using (var db = AppData.ActiveFireEventDB())
            {
                var table = db.ActiveFireEventTable();
                return table.FindAll();
            }
        }

        /// <summary>
        /// Queries all FireAlarmSystems from the LiteDB.
        /// </summary>
        /// <returns>Returns a list with all FireAlarmSystems from database.</returns>
        public static IEnumerable<FireAlarmSystem> QueryFireAlarmSystems()
        {
            using (var db = AppData.FireAlarmSystemDB())
            {
                var table = db.FireAlarmSystemTable();
                return table.FindAll();
            }
        }

        /// <summary>
        /// Queries all FireBrigades from the LiteDB.
        /// </summary>
        /// <returns>Returns a list with all FireBrigades from database.</returns>
        public static IEnumerable<FireBrigade> QueryFireBrigades()
        {
            using (var db = AppData.FireBrigadeDB())
            {
                var table = db.FireBrigadeTable();
                return table.FindAll();
            }
        }

        /// <summary>
        /// Queries all ServiceGroups from the LiteDB.
        /// </summary>
        /// <returns>Returns a list with all ServiceGroups from database.</returns>
        public static IEnumerable<ServiceGroup> QueryServiceGroups()
        {
            using (var db = AppData.ServiceGroupDB())
            {
                var table = db.ServiceGroupTable();
                return table.FindAll();
            }
        }

        /// <summary>
        /// Queries all Users from the LiteDB.
        /// </summary>
        /// <returns>Returns a list with all Users from database.</returns>
        public static IEnumerable<User> QueryUsers()
        {
            using (var db = AppData.UserDB())
            {
                var table = db.UserTable();
                return table.FindAll();
            }
        }
    }
}