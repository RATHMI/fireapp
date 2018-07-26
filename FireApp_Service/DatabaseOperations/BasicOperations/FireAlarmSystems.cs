using FireApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FireApp.Service.DatabaseOperations.BasicOperations
{
    public class FireAlarmSystems
    {
        /// <summary>
        /// Inserts a FireAlarmSystem into the database or updates it if it already exists.
        /// </summary>
        /// <param name="fas">The FireAlarmSystem you want to upsert.</param>
        /// <returns>Returns true if the FireAlarmSystem was inserted.</returns>
        public static bool Upsert(FireAlarmSystem fas)
        {
            if (fas != null)
            {
                LocalDatabase.UpsertFireAlarmSystem(fas);
                return DatabaseOperations.DbUpserts.UpsertFireAlarmSystem(fas);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Upserts a list of FireAlarmSystems into the database.
        /// </summary>
        /// <param name="fireAlarmSystems">The list of FireAlarmSystems you want to upsert.</param>
        /// <returns>Returns the number of upserted FireAlarmSystems.</returns>
        public static int BulkUpsert(IEnumerable<FireAlarmSystem> fireAlarmSystems)
        {
            int upserted = 0;
            if (fireAlarmSystems != null)
            {
                foreach (FireAlarmSystem fas in fireAlarmSystems)
                {
                    Upsert(fas);
                    upserted++;
                }
            }

            return upserted;
        }

        /// <summary>
        /// Checks if an id is already used by another FireAlarmSystem.
        /// </summary>
        /// <param name="id">The id you want to check.</param>
        /// <returns>Returns true if id is not used by other FireAlarmSystem.</returns>
        public static int CheckId(int id)
        {
            IEnumerable<FireAlarmSystem> all = LocalDatabase.GetAllFireAlarmSystems();

            // The highest Id of all FireAlarmSystems.
            int maxId = 0;
            int rv = id;

            foreach (FireAlarmSystem fas in all)
            {
                if (maxId < fas.Id)
                {
                    maxId = fas.Id;
                }

                if (fas.Id == id)
                {
                    rv = -1;
                }
            }

            // If the id is already used by another FireAlarmSystem
            // return a new id.
            if (rv == -1)
            {
                rv = maxId + 1;
            }

            return rv;
        }

        /// <summary>
        /// Returns all FireAlarmSystems. 
        /// </summary>
        /// <returns>Returns a list of all FireAlarmSystems.</returns>
        public static IEnumerable<FireAlarmSystem> GetAll()
        {
            return LocalDatabase.GetAllFireAlarmSystems().OrderBy(x => x.Company);
        }

        /// <summary>
        /// Returns the FireAlarmSystem with a matching id.
        /// </summary>
        /// <param name="id">The id of the FireAlarmSystem you are looking for.</param>
        /// <returns>Returns a FireAlarmSystem with a matching id.</returns>
        public static FireAlarmSystem GetById(int id)
        {
            IEnumerable<FireAlarmSystem> fireAlarmSystems = LocalDatabase.GetAllFireAlarmSystems();
            foreach (FireAlarmSystem fas in fireAlarmSystems)
            {
                if (fas.Id == id)
                {
                    return fas;
                }
            }

            // If no FireAlarmSystem with a matching id was found.
            throw new KeyNotFoundException();
        }
    }
}