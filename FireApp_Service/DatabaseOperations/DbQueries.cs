using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;

namespace FireApp.Service.DatabaseOperations
{
    public static class DbQueries
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns all FireEvents from database</returns>
        public static IEnumerable<FireEvent> QueryFireEvents()
        {
            using (var db = AppData.FireEventDB())
            {
                var table = db.FireEventTable();
                return table.FindAll();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns all active FireEvents from database</returns>
        public static IEnumerable<FireEvent> QueryActiveFireEvents()
        {
            using (var db = AppData.ActiveFireEventDB())
            {
                var table = db.ActiveFireEventTable();
                return table.FindAll();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns a list with all FireAlarmSystems from database</returns>
        public static IEnumerable<FireAlarmSystem> QueryFireAlarmSystems()
        {
            using (var db = AppData.FireAlarmSystemDB())
            {
                var table = db.FireAlarmSystemTable();
                return table.FindAll();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns a list with all FireBrigades from database</returns>
        public static IEnumerable<FireBrigade> QueryFireBrigades()
        {
            using (var db = AppData.FireBrigadeDB())
            {
                var table = db.FireBrigadeTable();
                return table.FindAll();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns a list with all ServiceMembers from database</returns>
        public static IEnumerable<ServiceMember> QueryServiceMembers()
        {
            using (var db = AppData.ServiceMemberDB())
            {
                var table = db.ServiceMemberTable();
                return table.FindAll();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns a list with all Users from database</returns>
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