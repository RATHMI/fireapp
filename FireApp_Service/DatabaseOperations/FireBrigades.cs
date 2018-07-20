using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;

namespace FireApp.Service.DatabaseOperations
{
    public static class FireBrigades
    {
        /// <summary>
        /// inserts a FireBrigade into the database or updates it if it already exists
        /// </summary>
        /// <param name="fb"></param>
        /// <returns>returns true if the insert was successful</returns>
        public static bool UpsertFireBrigade(FireBrigade fb)
        {
            if (fb != null)
            {
                LocalDatabase.UpsertFireBrigade(fb);
                return DatabaseOperations.DbUpserts.UpsertFireBrigade(fb);
            }else
            {
                return false;
            }
        }

        /// <summary>
        /// upserts a list of FireBrigades into the database
        /// </summary>
        /// <param name="fireBrigades">The list of FireBrigades you want to insert</param>
        /// <returns>returns the number of upserted FireBrigades</returns>
        public static int UpsertFireBrigades(IEnumerable<FireBrigade> fireBrigades)
        {
            int upserted = 0;
            if (fireBrigades != null)
            {
                foreach (FireBrigade fb in fireBrigades)
                {
                    UpsertFireBrigade(fb);           
                    upserted++;
                }
            }

            return upserted;
        }

        /// <summary>
        /// Deletes the FireBrigade from the Database and Cache
        /// The assoziations with the users and FireAlarmSystems are also deleted
        /// </summary>
        /// <param name="id">the id of the FireBrigade you want to delete</param>
        /// <returns>returns true if FireBrigade was deleted from DB</returns>
        public static bool DeleteFireBrigade(int id)
        {
            bool rv = DatabaseOperations.DbDeletes.DeleteFireBrigade(id);
            if (rv != true)
            {
                // delete local
                LocalDatabase.DeleteFireBrigade(id);

                // delete from authorizedObjectIds of users local
                foreach (User u in DatabaseOperations.Users.GetAllUsers())
                {
                    if (u.UserType == UserTypes.firebrigade && u.AuthorizedObjectIds.Contains(id))
                    {
                        u.AuthorizedObjectIds.Remove(id);
                        DatabaseOperations.Users.UpsertUser(u);
                    }
                }

                // delete from List of ServiceGroups of FireAlarmSystems local
                foreach (FireAlarmSystem fas in DatabaseOperations.FireAlarmSystems.GetAllFireAlarmSystems())
                {
                    if (fas.FireBrigades.Contains(id))
                    {
                        fas.FireBrigades.Remove(id);
                        DatabaseOperations.FireAlarmSystems.UpsertFireAlarmSystem(fas);
                    }
                }

                // delete from authorizedObjectIds of users in DB
                foreach (User u in DatabaseOperations.DbQueries.QueryUsers())
                {
                    if (u.UserType == UserTypes.firebrigade && u.AuthorizedObjectIds.Contains(id))
                    {
                        u.AuthorizedObjectIds.Remove(id);
                        DatabaseOperations.DbUpserts.UpsertUser(u);
                    }
                }

                // delete from List of ServiceGroups of FireAlarmSystems in DB
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
        /// Checks if an id is already used by another FireBrigade
        /// </summary>
        /// <param name="id">the id you want to check</param>
        /// <returns>returns true if id is not used by other FireBrigade</returns>
        public static int CheckId(int id)
        {
            IEnumerable<FireBrigade> all = LocalDatabase.GetAllFireBrigades();
            int maxId = 0;
            foreach (FireBrigade fb in all)
            {
                if (maxId < fb.Id)
                {
                    maxId = fb.Id;
                }
                if (fb.Id == id)
                {
                    return maxId + 1;
                }
            }
            return id;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns a list with all FireBrigades</returns>
        public static IEnumerable<FireBrigade> GetAllFireBrigades()
        {
            return (IEnumerable<FireBrigade>)LocalDatabase.GetAllFireBrigades().OrderBy(x => x.Name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">The id of the FireBrigade you are looking for</param>
        /// <returns>returns a FireBrigade with a matching id</returns>
        public static FireBrigade GetFireBrigadeById(int id)
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