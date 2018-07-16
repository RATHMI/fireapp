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
            LocalDatabase.UpsertFireBrigade(fb);

            return DatabaseOperations.DbUpserts.UpsertFireBrigade(fb);
        }

        /// <summary>
        /// Checks if an id is already used by another FireBrigade
        /// </summary>
        /// <param name="id">the id you want to check</param>
        /// <returns>returns true if id is not used by other FireBrigade</returns>
        public static bool CheckId(int id)
        {
            List<FireBrigade> all = LocalDatabase.GetAllFireBrigades();
            foreach (FireBrigade fb in all)
            {
                if (fb.Id == id)
                {
                    return false;
                }
            }
            return true;
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
        public static IEnumerable<FireBrigade> GetFireBrigadeById(int id)
        {
            List<FireBrigade> fireBrigades = LocalDatabase.GetAllFireBrigades();
            List<FireBrigade> results = new List<FireBrigade>();
            foreach (FireBrigade fb in fireBrigades)
            {
                if (fb.Id == id)
                {
                    results.Add(fb);
                    break;
                }
            }

            return results;
        }
    }
}