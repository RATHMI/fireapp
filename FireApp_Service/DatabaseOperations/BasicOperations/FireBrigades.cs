﻿using FireApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FireApp.Service.DatabaseOperations.BasicOperations
{
    public class FireBrigades
    {
        /// <summary>
        /// Inserts a FireBrigade into the database or updates it if it already exists.
        /// </summary>
        /// <param name="fb">The FireBrigade you want to upsert.</param>
        /// <returns>Returns true if the insert was successful.</returns>
        public static bool Upsert(FireBrigade fb)
        {
            if (fb != null)
            {
                LocalDatabase.UpsertFireBrigade(fb);
                return DatabaseOperations.DbUpserts.UpsertFireBrigade(fb);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Upserts a list of FireBrigades into the database.
        /// </summary>
        /// <param name="fireBrigades">The list of FireBrigades you want to upsert.</param>
        /// <returns>Returns the number of upserted FireBrigades.</returns>
        public static int BulkUpsert(IEnumerable<FireBrigade> fireBrigades)
        {
            int upserted = 0;
            if (fireBrigades != null)
            {
                foreach (FireBrigade fb in fireBrigades)
                {
                    Upsert(fb);
                    upserted++;
                }
            }

            return upserted;
        }

        /// <summary>
        /// Deletes the FireBrigade from the Database and Cache.
        /// The assoziations with the Users and FireAlarmSystems are also deleted.
        /// </summary>
        /// <param name="id">The id of the FireBrigade you want to delete.</param>
        /// <returns>Returns true if FireBrigade was deleted from DB.</returns>
        public static bool Delete(int id)
        {
            bool rv = DatabaseOperations.DbDeletes.DeleteFireBrigade(id);
            if (rv != true)
            {
                // Delete from cache.
                LocalDatabase.DeleteFireBrigade(id);

                // Delete from authorizedObjectIds of Users in the cache.
                foreach (User u in Users.GetAll())
                {
                    if (u.UserType == UserTypes.firebrigade && u.AuthorizedObjectIds.Contains(id))
                    {
                        u.AuthorizedObjectIds.Remove(id);
                        Users.Upsert(u);
                    }
                }

                // Delete from List of ServiceGroups of FireAlarmSystems in the cache.
                foreach (FireAlarmSystem fas in FireAlarmSystems.GetAll())
                {
                    if (fas.FireBrigades.Contains(id))
                    {
                        fas.FireBrigades.Remove(id);
                        FireAlarmSystems.Upsert(fas);
                    }
                }

                // Delete from authorizedObjectIds of Users in the database.
                foreach (User u in DatabaseOperations.DbQueries.QueryUsers())
                {
                    if (u.UserType == UserTypes.firebrigade && u.AuthorizedObjectIds.Contains(id))
                    {
                        u.AuthorizedObjectIds.Remove(id);
                        DatabaseOperations.DbUpserts.UpsertUser(u);
                    }
                }

                // Delete from List of ServiceGroups of FireAlarmSystems in the database.
                foreach (FireAlarmSystem fas in DatabaseOperations.DbQueries.QueryFireAlarmSystems())
                {
                    if (fas.FireBrigades.Contains(id))
                    {
                        fas.FireBrigades.Remove(id);
                        DatabaseOperations.DbUpserts.UpsertFireAlarmSystem(fas);
                    }
                }

            }
            return rv;
        }

        /// <summary>
        /// Checks if an id is already used by another FireBrigade.
        /// </summary>
        /// <param name="id">The id you want to check.</param>
        /// <returns>Returns true if the id is not used by another FireBrigade.</returns>
        public static int CheckId(int id)
        {
            IEnumerable<FireBrigade> all = LocalDatabase.GetAllFireBrigades();
            // The highest Id of all FireBrigades.
            int maxId = 0;
            int rv = id;

            foreach (FireBrigade fb in all)
            {
                if (maxId < fb.Id)
                {
                    maxId = fb.Id;
                }

                if (fb.Id == id)
                {
                    rv = -1;
                }
            }

            // If the id is already used by another FireBrigade
            // return a new id.
            if (rv == -1)
            {
                rv = maxId + 1;
            }

            return rv;
        }

        /// <summary>
        /// Returns all FireBrigades.
        /// </summary>
        /// <returns>Returns a list with all FireBrigades.</returns>
        public static IEnumerable<FireBrigade> GetAll()
        {
            return LocalDatabase.GetAllFireBrigades().OrderBy(x => x.Name);
        }

        /// <summary>
        /// Returns the FireBrigade with a matching id.
        /// </summary>
        /// <param name="id">The id of the FireBrigade you are looking for.</param>
        /// <returns>Returns a FireBrigade with a matching id.</returns>
        public static FireBrigade GetById(int id)
        {
            IEnumerable<FireBrigade> fireBrigades = LocalDatabase.GetAllFireBrigades();
            foreach (FireBrigade fb in fireBrigades)
            {
                if (fb.Id == id)
                {
                    return fb;
                }
            }

            throw new KeyNotFoundException();
        }
    }
}