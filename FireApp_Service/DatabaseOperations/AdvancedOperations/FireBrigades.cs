using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;

namespace FireApp.Service.DatabaseOperations.AdvancedOperations
{
    public static class FireBrigades
    {      
        /// <summary>
        /// Returns all FireBrigades associated with this FireAlarmSystem.
        /// </summary>
        /// <param name="fas">The FireAlarmsSystem with the Ids of the FireBrigades.</param>
        /// <returns>Returns all FireBrigades associated with this FireAlarmSystem.</returns>
        public static IEnumerable<FireBrigade> GetByFireAlarmSystem(FireAlarmSystem fas) // todo: comment
        {
            List<FireBrigade> results = new List<FireBrigade>();

            try
            {
                foreach(int id in fas.FireBrigades)
                {
                    results.Add(BasicOperations.FireBrigades.GetById(id));
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
        public static IEnumerable<User> GetUsers(int firebrigade) //todo: comment
        {
            return Users.GetByAuthorizedObject(firebrigade, UserTypes.firebrigade);
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
            foreach (FireAlarmSystem fas in BasicOperations.FireAlarmSystems.GetAll())
            {
                // If the FireAlarmSystem contains the id of the ServiceGroup.
                if (fas.FireBrigades.Contains(firebrigade))
                {
                    results.Add(fas);
                }
            }

            return results;
        }
    }
}