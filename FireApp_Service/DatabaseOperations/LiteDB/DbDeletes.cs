using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;

namespace FireApp.Service.DatabaseOperations
{
    /// <summary>
    /// This class is for deleting objects from the LiteDB
    /// </summary>
    //todo: comment class
    public static class DbDeletes
    {
        public static bool DeleteActiveFireEvent(FireEvent fe)
        {
            if (fe != null)
            {
                using (var db = AppData.ActiveFireEventDB())
                {
                    var table = db.ActiveFireEventTable();
                    FireEvent target = table.FindOne(x => x.Id.SourceId == fe.Id.SourceId && x.TargetId == fe.TargetId);
                    if (target != null)
                    {
                        if (table.Delete(x => x.Id == target.Id) > 0)
                        {
                            return true;
                        }                     
                    }
                }
            }

            return false;
        }

        public static bool DeleteUser(string userName)
        {
            if (userName != null)
            {
                using (var db = AppData.UserDB())
                {
                    var table = db.UserTable();
                    User user = table.FindOne(x => x.Id == userName);
                    if (user != null)
                    {
                        if (table.Delete(x => x.Id == user.Id) > 0)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public static bool DeleteFireBrigade(int id)
        {
            using (var db = AppData.FireBrigadeDB())
            {
                var table = db.FireBrigadeTable();
                FireBrigade fb = table.FindOne(x => x.Id == id);
                if (fb != null)
                {
                    if (table.Delete(x => x.Id == id) > 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool DeleteServiceGroup(int id)
        {
            using (var db = AppData.ServiceGroupDB())
            {
                var table = db.ServiceGroupTable();
                ServiceGroup sg = table.FindOne(x => x.Id == id);
                if (sg != null)
                {
                    if (table.Delete(x => x.Id == id) > 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

    }
}