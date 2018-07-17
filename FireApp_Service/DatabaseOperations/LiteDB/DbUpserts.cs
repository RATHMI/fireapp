using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;

namespace FireApp.Service.DatabaseOperations
{
    /// <summary>
    /// This class is for upserting objects into the LiteDB
    /// </summary>
    public static class DbUpserts
    {
        /// <summary>
        /// Upserts an active FireEvent into the LiteDB
        /// </summary>
        /// <param name="fe">the FireEvent you want to upsert</param>
        /// <returns>returns true if the FireEvent was inserted</returns>
        public static bool UpsertActiveFireEvent(FireEvent fe)
        {
            if (fe != null)
            {
                using (var db = AppData.ActiveFireEventDB())
                {
                    var table = db.ActiveFireEventTable();
                    return table.Upsert(fe);
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Upserts a FireEvent into the LiteDB
        /// </summary>
        /// <param name="fe">the FireEvent you want to upsert</param>
        /// <returns>returns true if the FireEvent was inserted</returns>
        public static bool UpsertFireEvent(FireEvent fe)
        {
            if (fe != null)
            {
                using (var db = AppData.FireEventDB())
                {
                    var table = db.FireEventTable();
                    return table.Upsert(fe);
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Upserts a FireAlarmSystem into the LiteDB
        /// </summary>
        /// <param name="fas">the FireAlarmSystem you want to upsert</param>
        /// <returns>returns true if the FireAlarmSystem was inserted</returns>
        public static bool UpsertFireAlarmSystem(FireAlarmSystem fas)
        {
            if (fas != null)
            {
                using (var db = AppData.FireAlarmSystemDB())
                {
                    var table = db.FireAlarmSystemTable();
                    return table.Upsert(fas);
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Upserts a FireBrigade into the LiteDB
        /// </summary>
        /// <param name="fb">the FireBrigade you want to upsert</param>
        /// <returns>returns true if the FireBrigade was inserted</returns>
        public static bool UpsertFireBrigade(FireBrigade fb)
        {
            if (fb != null)
            {
                using (var db = AppData.FireBrigadeDB())
                {
                    var table = db.FireBrigadeTable();
                    return table.Upsert(fb);
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Upserts a ServiceGroup into the LiteDB
        /// </summary>
        /// <param name="sg">the ServiceGroup you want to upsert</param>
        /// <returns>returns true if the ServiceGroup was inserted</returns>
        public static bool UpsertServiceGroup(ServiceGroup sg)
        {
            if (sg != null)
            {
                using (var db = AppData.ServiceGroupDB())
                {
                    var table = db.ServiceGroupTable();
                    return table.Upsert(sg);
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Upserts a User into the LiteDB
        /// </summary>
        /// <param name="user">the User you want to upsert</param>
        /// <returns>returns true if the User was inserted</returns>
        public static bool UpsertUser(User user)
        {
            if (user != null)
            {
                using (var db = AppData.UserDB())
                {
                    var table = db.UserTable();
                    return table.Upsert(user);
                }
            }
            else
            {
                return false;
            }
        }
    }
}