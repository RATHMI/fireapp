using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;

namespace FireApp.Service.DatabaseOperations
{
    /// <summary>
    /// This class is for upserting objects into the database.
    /// </summary>
    public static class DbUpserts
    {
        /// <summary>
        /// Upserts an active FireEvent into the database.
        /// </summary>
        /// <param name="fe">The FireEvent you want to upsert.</param>
        /// <returns>Returns true if the FireEvent was upserted.</returns>
        public static bool UpsertActiveFireEvent(FireEvent fe)
        {         
            try
            {
                // Insert into local database.
                LocalDatabase.UpsertActiveFireEvent(fe);

                // Insert into remote database.                
                return DatabaseOperations.LiteDB.LiteDbUpserts.UpsertActiveFireEvent(fe);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Upserts a FireEvent into the database.
        /// </summary>
        /// <param name="fe">The FireEvent you want to upsert.</param>
        /// <returns>Returns true if the FireEvent was upserted.</returns>
        public static bool UpsertFireEvent(FireEvent fe)
        {
            try
            {
                // Insert into local database.
                LocalDatabase.UpsertFireEvent(fe);

                // Insert into remote database.                
                return DatabaseOperations.LiteDB.LiteDbUpserts.UpsertFireEvent(fe);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Upserts a FireAlarmSystem into the database.
        /// </summary>
        /// <param name="fas">The FireAlarmSystem you want to upsert.</param>
        /// <returns>Returns true if the FireAlarmSystem was upserted.</returns>
        public static bool UpsertFireAlarmSystem(FireAlarmSystem fas)
        {
            try
            {
                // Insert into local database.
                LocalDatabase.UpsertFireAlarmSystem(fas);

                // Insert into remote database.                
                return DatabaseOperations.LiteDB.LiteDbUpserts.UpsertFireAlarmSystem(fas);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Upserts a FireBrigade into the database.
        /// </summary>
        /// <param name="fb">the FireBrigade you want to upsert.</param>
        /// <returns>Returns true if the FireBrigade was upserted.</returns>
        public static bool UpsertFireBrigade(FireBrigade fb)
        {
            try
            {
                // Insert into local database.
                LocalDatabase.UpsertFireBrigade(fb);

                // Insert into remote database.                
                return DatabaseOperations.LiteDB.LiteDbUpserts.UpsertFireBrigade(fb);
            }
            catch (Exception)
            {
                return false;
            }      
        }

        /// <summary>
        /// Upserts a ServiceGroup into the database.
        /// </summary>
        /// <param name="sg">The ServiceGroup you want to upsert.</param>
        /// <returns>Returns true if the ServiceGroup was upserted.</returns>
        public static bool UpsertServiceGroup(ServiceGroup sg)
        {
            try
            {
                // Insert into local database.
                LocalDatabase.UpsertServiceGroup(sg);

                // Insert into remote database.                
                return DatabaseOperations.LiteDB.LiteDbUpserts.UpsertServiceGroup(sg);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Upserts a User into the database.
        /// </summary>
        /// <param name="user">The User you want to upsert.</param>
        /// <returns>Returns true if the User was upserted.</returns>
        public static bool UpsertUser(User user)
        {
            try
            {     
                // Insert into local database.
                LocalDatabase.UpsertUser(user);

                // Insert into remote database.                
                return DatabaseOperations.LiteDB.LiteDbUpserts.UpsertUser(user);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}