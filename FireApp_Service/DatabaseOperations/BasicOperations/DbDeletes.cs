using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;

namespace FireApp.Service.DatabaseOperations
{
    /// <summary>
    /// This class is for deleting objects from the database.
    /// </summary>
    public static class DbDeletes
    {
        /// <summary>
        /// Deletes an active FireEvent from the database.
        /// </summary>
        /// <param name="fe">The FireEvent you want to delete.</param>
        /// <returns>Returns true if the FireEvent was deleted.</returns>
        public static bool DeleteActiveFireEvent(FireEvent fe)
        {
            try
            {
                // Delete active FireEvent from local database.
                LocalDatabase.DeleteActiveFireEvent(fe);

                // Delete active FireEvent from remote database.
                return DatabaseOperations.LiteDB.LiteDbDeletes.DeleteActiveFireEvent(fe);
            }
            catch (Exception)
            {
                return false;
            }        
        }

        /// <summary>
        /// Deletes a User from the database.
        /// </summary>
        /// <param name="userName">The Username of the User you want to delete.</param>
        /// <returns>Returns true if a User was deleted.</returns>
        public static bool DeleteUser(string userName)
        {
            try
            {
                // Delete from local database.
                LocalDatabase.DeleteUser(userName);

                // Delete from remote database.
                return DatabaseOperations.LiteDB.LiteDbDeletes.DeleteUser(userName);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Deletes a FireBrigade from the database.
        /// </summary>
        /// <param name="id">The id of the FireBrigade you want to delete.</param>
        /// <returns>Returns true if a FireBrigade was deleted.</returns>
        public static bool DeleteFireBrigade(int id)
        {
            try
            {
                // Delete from local database.
                LocalDatabase.DeleteFireBrigade(id);

                // Delete from remote database.
                return DatabaseOperations.LiteDB.LiteDbDeletes.DeleteFireBrigade(id);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Deletes a ServiceGroup from the database.
        /// </summary>
        /// <param name="id">The id of the ServiceGroup you want to delete.</param>
        /// <returns>Returns true if a ServiceGroup was deleted.</returns>
        public static bool DeleteServiceGroup(int id)
        {
            try
            {
                // Delete from local database.
                LocalDatabase.DeleteServiceGroup(id);

                // Delete from remote database.
                return DatabaseOperations.LiteDB.LiteDbDeletes.DeleteServiceGroup(id);
            }
            catch(Exception)
            {
                return false;
            }
        }

    }
}