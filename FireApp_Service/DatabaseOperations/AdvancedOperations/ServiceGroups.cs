using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;

namespace FireApp.Service.DatabaseOperations.AdvancedOperations
{
    public static class ServiceGroups
    {
        /// <summary>
        /// Returns all ServiceGroups that are in the list of ServiceGroups of the FireAlarmSystem.
        /// </summary>
        /// <param name="fas">The FireAlarmSystem you want to get the ServiceGroups of.</param>
        /// <returns>Returns all ServiceGroups that are assoziated with this FireAlarmSystem.</returns>
        public static IEnumerable<ServiceGroup> GetByFireAlarmSystem(FireAlarmSystem fas)
        {
            List<ServiceGroup> results = new List<ServiceGroup>();

            try
            {
                foreach (int id in fas.FireBrigades)
                {
                    results.Add(BasicOperations.ServiceGroups.GetById(id));
                }

                return results;
            }
            catch (Exception)
            {
                return new List<ServiceGroup>();
            }
        }

        /// <summary>
        /// Returns all Users that are associated with this ServiceGroup.
        /// </summary>
        /// <param name="servicegroup">The ServiceGroup you want to get the Users of.</param>
        /// <returns>Returns all Users whose AuthorizedObjectIds contains "servicegroup".</returns>
        public static IEnumerable<User> GetUsers(int servicegroup)
        {
            return Users.GetByAuthorizedObject(servicegroup, UserTypes.servicemember);
        }

        /// <summary>
        /// Returns all FireAlarmSystems that are associated with this ServiceGroup.
        /// </summary>
        /// <param name="servicegroup">The ServiceGroup you want to get the FireAlarmSystems of.</param>
        /// <returns>Returns all FireAlarmSystems that are associated with this ServiceGroup.</returns>
        public static IEnumerable<FireAlarmSystem> GetFireAlarmSystems(int servicegroup)
        {
            List<FireAlarmSystem> results = new List<FireAlarmSystem>();

            // Find all FireAlarmSystems where the id of the FireAlarmSystem is contained
            // in the list of FireAlarmSystems.
            foreach (FireAlarmSystem fas in BasicOperations.FireAlarmSystems.GetAll())
            {
                // If the FireAlarmSystem contains the id of the ServiceGroup.
                if (fas.ServiceGroups.Contains(servicegroup))
                {
                    results.Add(fas);
                }
            }

            return results;
        }
    }
}