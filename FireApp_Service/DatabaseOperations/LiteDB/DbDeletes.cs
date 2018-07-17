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
    public static class DbDeletes
    {
        /// <summary>
        /// Deletes an active FireEvent from the LiteDB
        /// </summary>
        /// <param name="fe">the FireEvent you want to delete</param>
        /// <returns>returns true if FireEvent was deleted</returns>
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

        /// <summary>
        /// Deletes a User from the LiteDB
        /// </summary>
        /// <param name="userName">the UserName of the User you want to delete</param>
        /// <returns>returns true if a User was deleted</returns>
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

        /// <summary>
        /// Deletes a FireBrigade from the LiteDB
        /// </summary>
        /// <param name="id">the id of the FireBrigade you want to delete</param>
        /// <returns>returns true if a FireBrigade was deleted</returns>
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

        /// <summary>
        /// Deletes a ServiceGroup from the LiteDB
        /// </summary>
        /// <param name="id">the id of the ServiceGroup you want to delete</param>
        /// <returns>returns true if a ServiceGroup was deleted</returns>
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