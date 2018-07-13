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
            LocalDatabase.UpsertFireAlarmSystem(fas);

            //Logging.Logger.Log("upsert : " + fas.ToLog(), AppSettings.FireEventDBPath + "/_Log/log.txt");

            return DatabaseOperations.DbUpserts.UpsertFireAlarmSystem(fas);
        }

        /// <summary>
        /// Checks if an id is already used by another FireAlarmSystem
        /// </summary>
        /// <param name="id">the id you want to check</param>
        /// <returns>returns true if id is not used by other FireAlarmSystem</returns>
        public static bool CheckId(int id)
        {
            List<FireAlarmSystem> all = LocalDatabase.GetAllFireAlarmSystems();
            foreach (FireAlarmSystem fas in all)
            {
                if (fas.Id == id)
                {
                    return false;
                }
            }
            return true;
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
        /// <param name="id">The id of the FireAlarmSystem you are looking for</param>
        /// <returns>returns a FireAlarmSystem with a matching id</returns>
        public static IEnumerable<FireAlarmSystem> GetFireAlarmSystemById(int id)
        {
            List<FireAlarmSystem> fireAlarmSystems = LocalDatabase.GetAllFireAlarmSystems();
            if (fireAlarmSystems != null)
            {
                return fireAlarmSystems.FindAll(x => x.Id == id);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Adds a FireBrigade to the list of FireBrigades of a FireAlarmSystem
        /// </summary>
        /// <param name="id">identifier of the FireAlarmSystem</param>
        /// <param name="firebrigade">identifier of the firebrigade</param>
        /// <returns>returns true if the FireBrigade was added</returns>
        public static bool AddFireBrigade(int id, int firebrigade)
        {
            bool rv = false;
            FireAlarmSystem fas = DatabaseOperations.FireAlarmSystems.GetFireAlarmSystemById(id).First<FireAlarmSystem>();
            if (fas != null)
            {
                FireBrigade fb = DatabaseOperations.FireBrigades.GetFireBrigadeById(firebrigade).First<FireBrigade>();
                if (fb != null)
                {
                    fas.FireBrigades.Add(firebrigade);
                    rv = UpsertFireAlarmSystem(fas);
                }
            }

            return rv;
        }

        /// <summary>
        /// Adds a ServiceMember to the list of ServiceMembers of a FireAlarmSystem
        /// </summary>
        /// <param name="id">identifier of the FireAlarmSystem</param>
        /// <param name="serviceMember">identifier of the ServiceMember</param>
        /// <returns>returns true if the ServiceMember was added</returns>
        public static bool AddServiceMember(int id, int serviceMember)
        {
            bool rv = false;
            FireAlarmSystem fas = DatabaseOperations.FireAlarmSystems.GetFireAlarmSystemById(id).First<FireAlarmSystem>();
            if (fas != null)
            {
                ServiceMember sm = DatabaseOperations.ServiceMembers.GetServiceMemberById(serviceMember).First<ServiceMember>();
                if (sm != null)
                {
                    fas.ServiceMembers.Add(serviceMember);
                    rv = UpsertFireAlarmSystem(fas);
                }
            }

            return rv;
        }
    }
}