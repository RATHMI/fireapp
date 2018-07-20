using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;

namespace FireApp.Service.DatabaseOperations
{
    public static class FireAlarmSystems
    {
        /// <summary>
        /// inserts a FireAlarmSystem into the database or updates it if it already exists
        /// </summary>
        /// <param name="fas">The FireAlarmSystem you want to insert</param>
        /// <returns>returns true if FireAlarmSystem was inserted</returns>
        public static bool Upsert(FireAlarmSystem fas)
        {
            if (fas != null)
            {
                LocalDatabase.UpsertFireAlarmSystem(fas);
                return DatabaseOperations.DbUpserts.UpsertFireAlarmSystem(fas);
            }else
            {
                return false;
            }
        }

        /// <summary>
        /// upserts a list of FireAlarmSystems into the database
        /// </summary>
        /// <param name="fireAlarmSystems">The list of FireAlarmSystems you want to insert</param>
        /// <returns>returns the number of upserted FireAlarmSystems</returns>
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
        /// Checks if an id is already used by another FireAlarmSystem
        /// </summary>
        /// <param name="id">the id you want to check</param>
        /// <returns>returns true if id is not used by other FireAlarmSystem</returns>
        public static int CheckId(int id)
        {
            IEnumerable<FireAlarmSystem> all = LocalDatabase.GetAllFireAlarmSystems();
            int maxId = 0;
            foreach (FireAlarmSystem fas in all)
            {
                if (maxId < fas.Id)
                {
                    maxId = fas.Id;
                }
                if (fas.Id == id)
                {
                    return maxId + 1;
                }
            }
            return id;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns a list with all FireAlarmSystems</returns>
        public static IEnumerable<FireAlarmSystem> GetAll()
        {
            return (IEnumerable<FireAlarmSystem>)LocalDatabase.GetAllFireAlarmSystems().OrderBy(x => x.Company);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns a list of all FireAlarmSystems with active FireEvents</returns>
        public static IEnumerable<FireAlarmSystem> GetActiveFireAlarmSystems(User user)
        {
            IEnumerable<FireEvent> events; //todo: comment
            events = ActiveEvents.GetAll();
            events = Filter.FireEventsFilter.UserFilter(events, user);
            HashSet<FireAlarmSystem> results = new HashSet<FireAlarmSystem>();
            FireAlarmSystem fas;

            foreach(FireEvent fe in events)
            {
                try { 
                    fas = GetById(fe.Id.SourceId);
                    results.Add(fas);
                }
                catch (Exception)
                {                    
                    continue;
                }
            }

            return results;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">The id of the FireAlarmSystem you are looking for</param>
        /// <returns>returns a FireAlarmSystem with a matching id</returns>
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

            throw new KeyNotFoundException();
        }

        /// <summary>
        /// Adds a FireBrigade to the list of FireBrigades of a FireAlarmSystem
        /// </summary>
        /// <param name="id">identifier of the FireAlarmSystem</param>
        /// <param name="firebrigade">identifier of the firebrigade</param>
        /// <returns>returns true if the FireBrigade was added</returns>
        public static bool AddFireBrigade(int id, int firebrigade)
        {
            try
            {
                FireAlarmSystem fas = GetById(id);
                FireBrigade fb = DatabaseOperations.FireBrigades.GetById(firebrigade);
                fas.FireBrigades.Add(fb.Id);
                return Upsert(fas);
            }
            catch (Exception)
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Adds a ServiceGroup to the list of ServiceGroups of a FireAlarmSystem
        /// </summary>
        /// <param name="id">identifier of the FireAlarmSystem</param>
        /// <param name="serviceGroup">identifier of the ServiceGroup</param>
        /// <returns>returns true if the ServiceGroup was added</returns>
        public static bool AddServiceGroup(int id, int serviceGroup)
        {
            try { 
                FireAlarmSystem fas = GetById(id);
                ServiceGroup sg = ServiceGroups.GetById(serviceGroup);
                fas.ServiceGroups.Add(sg.Id);
                return Upsert(fas);
            }
            catch (Exception)
            {
                throw new InvalidOperationException();
            }
        }
    }
}