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

            using (var db = AppData.FireBrigadeDB())
            {
                var table = db.FireBrigadeTable();
                return table.Upsert(fb);
            }
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
            if (fireBrigades != null)
            {
                return fireBrigades.FindAll(x => x.Id == id);
            }
            else
            {
                return null;
            }
        }
    }
}