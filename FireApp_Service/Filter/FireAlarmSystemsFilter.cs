using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;

namespace FireApp.Service.Filter
{
    /// <summary>
    /// This class provide methods to filter FireAlarmSystems by their properties and the UserType
    /// </summary>
    public static class FireAlarmSystemsFilter
    {
        /// <summary>
        /// filters a list of FireAlarmSystems according to the rights of a user
        /// </summary>
        /// <param name="fireAlarmSystems">a list of FireAlarmSystems you want to filter</param>
        /// <param name="user">the user you want the FireAlarmSystems to filter for</param>
        /// <returns>returns a filtered list of FireAlarmSystems</returns>
        public static IEnumerable<FireAlarmSystem> UserFilter(IEnumerable<FireAlarmSystem> fireAlarmSystems, User user)
        {
            List<FireAlarmSystem> results = new List<FireAlarmSystem>();
            if (fireAlarmSystems != null && user != null)
            {
                if (user.UserType == UserTypes.admin)
                {
                    results.AddRange(fireAlarmSystems);
                }
                if (user.UserType == UserTypes.firealarmsystem)
                {
                    foreach (int authorizedObject in user.AuthorizedObjectIds)
                    {
                        results.AddRange(fireAlarmSystemFilter(fireAlarmSystems, authorizedObject));
                    }
                }
                if (user.UserType == UserTypes.firebrigade)
                {
                    foreach (int authorizedObject in user.AuthorizedObjectIds)
                    {
                        results.AddRange(fireBrigadeFilter(fireAlarmSystems, authorizedObject));
                    }                   
                }
                if (user.UserType == UserTypes.servicemember)
                {
                    foreach (int authorizedObject in user.AuthorizedObjectIds)
                    {
                        results.AddRange(.serviceGroupFilter(fireAlarmSystems, authorizedObject));
                    }
                }
            }

            results.OrderBy(x => x.Company);
            return (IEnumerable<FireAlarmSystem>)results;
        }

        /// <summary>
        /// only returns FireAlarmSystems with a matching id
        /// </summary>
        /// <param name="fireAlarmSystems">a list of FireAlarmSystems you want to filter</param>
        /// <param name="id">the id of the FireAlarmSystem</param>
        /// <returns>returns a filtered list of FireAlarmSystems</returns>
        private static IEnumerable<FireAlarmSystem> fireAlarmSystemFilter(IEnumerable<FireAlarmSystem> fireAlarmSystems, int id)
        {
            if (fireAlarmSystems != null)
            {
                foreach (FireAlarmSystem fas in fireAlarmSystems)
                {
                    if (fas.Id == id)
                    {
                        return (IEnumerable<FireAlarmSystem>)fas;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// only returns FireAlarmSystems where the FireBrigade is in the list of FireBrigades
        /// </summary>
        /// <param name="fireAlarmSystems">a list of FireAlarmSystems you want to filter</param>
        /// <param name="id">the id of the FireBrigade</param>
        /// <returns>returns a filtered list of FireAlarmSystems</returns>
        private static IEnumerable<FireAlarmSystem> fireBrigadeFilter(IEnumerable<FireAlarmSystem> fireAlarmSystems, int id)
        {
            if (fireAlarmSystems != null)
            {
                List<FireAlarmSystem> results = new List<FireAlarmSystem>();
                FireAlarmSystem copy;
                foreach (FireAlarmSystem fas in fireAlarmSystems)
                {
                    if (fas.FireBrigades.Contains(id))
                    {
                        copy = (FireAlarmSystem)fas.Clone();
                        copy.ServiceGroups.Clear();
                        results.Add(copy);
                    }
                }
                return (IEnumerable<FireAlarmSystem>)results;
            }
            return null;
        }

        /// <summary>
        /// only returns FireAlarmSystems where the ServiceGroup is in the list of ServiceGroups
        /// </summary>
        /// <param name="fireAlarmSystems">a list of FireAlarmSystems you want to filter</param>
        /// <param name="id">the id of the ServiceGroup</param>
        /// <returns>returns a filtered list of FireAlarmSystems</returns>
        private static IEnumerable<FireAlarmSystem> serviceGroupFilter(IEnumerable<FireAlarmSystem> fireAlarmSystems, int id)
        {
            if (fireAlarmSystems != null)
            {
                List<FireAlarmSystem> results = new List<FireAlarmSystem>();
                FireAlarmSystem copy;
                foreach (FireAlarmSystem fas in fireAlarmSystems)
                {
                    if (fas.ServiceGroups.Contains(id))
                    {
                        copy = (FireAlarmSystem)fas.Clone();
                        copy.FireBrigades.Clear();
                        results.Add(copy);
                    }
                }
                return (IEnumerable<FireAlarmSystem>)results;
            }
            return null;
        }
    }
}