using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;

namespace FireApp.Service.DatabaseOperations
{
    //todo: comment class
    /// <summary>
    /// This class is for upserting objects into the LiteDB
    /// </summary>
    public static class DbUpserts
    {
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