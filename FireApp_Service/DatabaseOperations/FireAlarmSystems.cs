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
        public static bool UpsertFireAlarmSystem(FireAlarmSystem fas)
        {
            if (fas != null)
            {
                LocalDatabase.UpsertFireAlarmSystem(fas);

                Logging.Logger.Log("upsert", fas);

                return DatabaseOperations.DbUpserts.UpsertFireAlarmSystem(fas);
            }else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if an id is already used by another FireAlarmSystem
        /// </summary>
        /// <param name="id">the id you want to check</param>
        /// <returns>returns true if id is not used by other FireAlarmSystem</returns>
        public static int CheckId(int id)
        {
            List<FireAlarmSystem> all = LocalDatabase.GetAllFireAlarmSystems();
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
        public static IEnumerable<FireAlarmSystem> GetAllFireAlarmSystems()
        {
            return (IEnumerable<FireAlarmSystem>)LocalDatabase.GetAllFireAlarmSystems().OrderBy(x => x.Company);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns a list of all FireAlarmSystems with active FireEvents</returns>
        public static IEnumerable<FireAlarmSystem> GetActiveFireAlarmSystems(User user)
        {
            List<FireEvent> events = Filter.FireEventsFilter
                .UserFilter((List<FireEvent>)DatabaseOperations.ActiveEvents
                .GetAllActiveFireEvents(), user).ToList<FireEvent>();
            HashSet<FireAlarmSystem> results = new HashSet<FireAlarmSystem>();

            foreach(FireEvent fe in events)
            {
                results.Add(DatabaseOperations.FireAlarmSystems.GetFireAlarmSystemById(fe.Id.SourceId).First<FireAlarmSystem>());
            }

            return (IEnumerable<FireAlarmSystem>)results;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">The id of the FireAlarmSystem you are looking for</param>
        /// <returns>returns a FireAlarmSystem with a matching id</returns>
        public static IEnumerable<FireAlarmSystem> GetFireAlarmSystemById(int id)
        {
            List<FireAlarmSystem> fireAlarmSystems = LocalDatabase.GetAllFireAlarmSystems();
            List<FireAlarmSystem> results = new List<FireAlarmSystem>();
            foreach (FireAlarmSystem fas in fireAlarmSystems)
            {
                if (fas.Id == id)
                {
                    results.Add(fas);
                    break;
                }
            }

            return results;
        }

        /// <summary>
        /// Adds a FireBrigade to the list of FireBrigades of a FireAlarmSystem
        /// </summary>
        /// <param name="id">identifier of the FireAlarmSystem</param>
        /// <param name="firebrigade">identifier of the firebrigade</param>
        /// <returns>returns true if the FireBrigade was added</returns>
        public static bool AddFireBrigade(int id, int firebrigade)
        {
            FireAlarmSystem fas = GetFireAlarmSystemById(id).First<FireAlarmSystem>();
            FireBrigade fb = DatabaseOperations.FireBrigades.GetFireBrigadeById(firebrigade).First<FireBrigade>();
            fas.FireBrigades.Add(firebrigade);
            return UpsertFireAlarmSystem(fas);  
        }

        /// <summary>
        /// Adds a ServiceGroup to the list of ServiceGroups of a FireAlarmSystem
        /// </summary>
        /// <param name="id">identifier of the FireAlarmSystem</param>
        /// <param name="serviceGroup">identifier of the ServiceGroup</param>
        /// <returns>returns true if the ServiceGroup was added</returns>
        public static bool AddServiceGroup(int id, int serviceGroup)
        {
            FireAlarmSystem fas = DatabaseOperations.FireAlarmSystems.GetFireAlarmSystemById(id).First<FireAlarmSystem>();
            ServiceGroup sg = DatabaseOperations.ServiceGroups.GetServiceGroupById(serviceGroup).First<ServiceGroup>();
            fas.ServiceGroups.Add(serviceGroup);
            return UpsertFireAlarmSystem(fas);
        }
    }
}