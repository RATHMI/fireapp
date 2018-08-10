﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;

namespace FireApp.Service.DatabaseOperations
{
    public static class FireBrigades
    {
        /// <summary>
        /// Inserts a FireBrigade into the database or updates it if it already exists.
        /// </summary>
        /// <param name="fb">The FireBrigade you want to upsert.</param>
        /// <returns>Returns true if the insert was successful.</returns>
        public static bool Upsert(FireBrigade fb, User user)
        {
            if (fb != null && fb.Id != 0)
            {
                bool ok = DatabaseOperations.DbUpserts.UpsertFireBrigade(fb);
                if (ok)
                {
                    LocalDatabase.UpsertFireBrigade(fb);
                    Logging.Logger.Log("upsert", user.GetUserDescription(), fb);
                }
                return ok;
            }else
            {
                return false;
            }
        }

        /// <summary>
        /// Upserts a list of FireBrigades into the database.
        /// </summary>
        /// <param name="fireBrigades">The list of FireBrigades you want to upsert.</param>
        /// <returns>Returns the number of upserted FireBrigades.</returns>
        public static int BulkUpsert(IEnumerable<FireBrigade> fireBrigades, User user)
        {
            int upserted = 0;
            if (fireBrigades != null)
            {
                foreach (FireBrigade fb in fireBrigades)
                {
                    if (Upsert(fb, user) == true)
                    {
                        upserted++;
                    }            
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
        public static bool Delete(int id, User user)
        {
            bool ok = DatabaseOperations.DbDeletes.DeleteFireBrigade(id);
            if (ok)
            {
                // Write log message.
                FireBrigade old = GetById(id);
                Logging.Logger.Log("delete", user.GetUserDescription(), old);

                // Delete from cache.
                LocalDatabase.DeleteFireBrigade(id);

                // Delete from authorizedObjectIds of Users in the cache.
                foreach (User u in DatabaseOperations.Users.GetAll())
                {
                    if (u.UserType == UserTypes.fireFighter && u.AuthorizedObjectIds.Contains(id))
                    {
                        u.AuthorizedObjectIds.Remove(id);
                        DatabaseOperations.Users.Upsert(u, user);
                    }
                }

                // Delete from List of ServiceGroups of FireAlarmSystems in the cache.
                foreach (FireAlarmSystem fas in DatabaseOperations.FireAlarmSystems.GetAll())
                {
                    if (fas.FireBrigades.Contains(id))
                    {
                        fas.FireBrigades.Remove(id);
                        DatabaseOperations.FireAlarmSystems.Upsert(fas, user);
                    }
                }

                // Delete from authorizedObjectIds of Users in the database.
                foreach (User u in DatabaseOperations.DbQueries.QueryUsers())
                {
                    if (u.UserType == UserTypes.fireFighter && u.AuthorizedObjectIds.Contains(id))
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
            return ok;
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

                if (fb.Id == id || id == 0)
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

        /// <summary>
        /// Returns all FireBrigades associated with this FireAlarmSystem.
        /// </summary>
        /// <param name="fas">The FireAlarmsSystem with the Ids of the FireBrigades.</param>
        /// <returns>Returns all FireBrigades associated with this FireAlarmSystem.</returns>
        public static IEnumerable<FireBrigade> GetByFireAlarmSystem(FireAlarmSystem fas)
        {
            List<FireBrigade> results = new List<FireBrigade>();

            try
            {
                foreach(int id in fas.FireBrigades)
                {
                    results.Add(GetById(id));
                }

                return results;
            }
            catch (Exception)
            {
                return new List<FireBrigade>();
            }
        }

        /// <summary>
        /// Returns all Users that are associated with this FireBrigade.
        /// </summary>
        /// <param name="firebrigade">The FireBrigade you want to get the Users of.</param>
        /// <returns>Returns all Users that are associated with this FireBrigade.</returns>
        public static IEnumerable<User> GetUsers(int firebrigade)
        {
            return DatabaseOperations.Users.GetByAuthorizedObject(firebrigade, UserTypes.fireFighter);
        }

        /// <summary>
        /// Returns all FireAlarmSystems that are associated with this FireBrigade.
        /// </summary>
        /// <param name="firebrigade">The FireBrigade you want to get the FireAlarmSystems of.</param>
        /// <returns>Returns all FireAlarmSystems that are associated with this FireBrigade.</returns>
        public static IEnumerable<FireAlarmSystem> GetFireAlarmSystems(int firebrigade)
        {
            List<FireAlarmSystem> results = new List<FireAlarmSystem>();

            // Find all FireAlarmSystems where the id of the FireBrigade is contained
            // in the list of FireBrigades.
            foreach (FireAlarmSystem fas in DatabaseOperations.FireAlarmSystems.GetAll())
            {
                // If the FireAlarmSystem contains the id of the FireBrigade.
                if (fas.FireBrigades.Contains(firebrigade))
                {
                    results.Add(fas);
                }
            }

            return results;
        }
    }
}