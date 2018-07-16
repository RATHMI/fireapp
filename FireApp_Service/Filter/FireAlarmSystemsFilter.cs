using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;

namespace FireApp.Service.Filter
{
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
                        results.AddRange(Filter.FireAlarmSystemsFilter.fireAlarmSystemFilter(fireAlarmSystems, authorizedObject));
                    }
                }
                if (user.UserType == UserTypes.firebrigade)
                {
                    foreach (int authorizedObject in user.AuthorizedObjectIds)
                    {
                        results.AddRange(Filter.FireAlarmSystemsFilter.fireBrigadeFilter(fireAlarmSystems, authorizedObject));
                    }                   
                }
                if (user.UserType == UserTypes.servicemember)
                {
                    foreach (int authorizedObject in user.AuthorizedObjectIds)
                    {
                        results.AddRange(Filter.FireAlarmSystemsFilter.serviceMemberFilter(fireAlarmSystems, authorizedObject));
                    }
                }
            }
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
                        copy.ServiceMembers.Clear();
                        results.Add(copy);
                    }
                }
                return (IEnumerable<FireAlarmSystem>)results;
            }
            return null;
        }

        /// <summary>
        /// only returns FireAlarmSystems where the ServiceMember is in the list of ServiceMembers
        /// </summary>
        /// <param name="fireAlarmSystems">a list of FireAlarmSystems you want to filter</param>
        /// <param name="id">the id of the ServiceMember</param>
        /// <returns>returns a filtered list of FireAlarmSystems</returns>
        private static IEnumerable<FireAlarmSystem> serviceMemberFilter(IEnumerable<FireAlarmSystem> fireAlarmSystems, int id)
        {
            if (fireAlarmSystems != null)
            {
                List<FireAlarmSystem> results = new List<FireAlarmSystem>();
                FireAlarmSystem copy;
                foreach (FireAlarmSystem fas in fireAlarmSystems)
                {
                    if (fas.ServiceMembers.Contains(id))
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