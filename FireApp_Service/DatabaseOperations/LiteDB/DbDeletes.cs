using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;

namespace FireApp.Service.DatabaseOperations.LiteDB
{
    /// <summary>
    /// This class is for deleting objects from the LiteDB.
    /// </summary>
    public static class LiteDbDeletes
    {
        /// <summary>
        /// Deletes an active FireEvent from the LiteDB.
        /// </summary>
        /// <param name="fe">The FireEvent you want to delete.</param>
        /// <returns>Returns true if the FireEvent was deleted.</returns>
        public static bool DeleteActiveFireEvent(FireEvent fe)
        {
            try
            {
                if (fe != null)
                {
                    using (var db = AppData.ActiveFireEventDB())
                    {
                        var table = db.ActiveFireEventTable();
                        if (table.Delete(x => x.Id.SourceId == fe.Id.SourceId && x.TargetId == fe.TargetId) > 0)
                        {
                            return true;
                        }                        
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }        
        }

        /// <summary>
        /// Deletes a User from the LiteDB.
        /// </summary>
        /// <param name="userName">The Username of the User you want to delete.</param>
        /// <returns>Returns true if a User was deleted.</returns>
        public static bool DeleteUser(string userName)
        {
            try
            {
                if (userName != null)
                {
                    using (var db = AppData.UserDB())
                    {
                        var table = db.UserTable();
                        if (table.Delete(x => x.Id == userName) > 0)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Deletes a FireBrigade from the LiteDB.
        /// </summary>
        /// <param name="id">The id of the FireBrigade you want to delete.</param>
        /// <returns>Returns true if a FireBrigade was deleted.</returns>
        public static bool DeleteFireBrigade(int id)
        {
            try
            {
                using (var db = AppData.FireBrigadeDB())
                {
                    var table = db.FireBrigadeTable();
                    if (table.Delete(x => x.Id == id) > 0)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Deletes a ServiceGroup from the LiteDB.
        /// </summary>
        /// <param name="id">The id of the ServiceGroup you want to delete.</param>
        /// <returns>Returns true if a ServiceGroup was deleted.</returns>
        public static bool DeleteServiceGroup(int id)
        {
            try
            {
                using (var db = AppData.ServiceGroupDB())
                {
                    var table = db.ServiceGroupTable();
                    if (table.Delete(x => x.Id == id) > 0)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch(Exception)
            {
                return false;
            }
        }

    }
}