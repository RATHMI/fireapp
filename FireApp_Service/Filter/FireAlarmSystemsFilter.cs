﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;
using FireApp.Domain.Extensionmethods;

namespace FireApp.Service.Filter
{
    /// <summary>
    /// This class provides methods to filter FireAlarmSystems by their properties and the UserType.
    /// </summary>
    public static class FireAlarmSystemsFilter
    {
        /// <summary>
        /// Filters a list of FireAlarmSystems according to the rights of a User.
        /// </summary>
        /// <param name="fireAlarmSystems">A list of FireAlarmSystems you want to filter.</param>
        /// <param name="user">The user you want the FireAlarmSystems to filter for.</param>
        /// <returns>Returns a filtered list of FireAlarmSystems.</returns>
        public static IEnumerable<FireAlarmSystem> UserFilter(IEnumerable<FireAlarmSystem> fireAlarmSystems, User user)
        { 
            List<FireAlarmSystem> results = new List<FireAlarmSystem>();
            if (fireAlarmSystems != null && user != null)
            {
                if (user.UserType == UserTypes.admin)
                {
                    results.AddRange(fireAlarmSystems);
                }
                if (user.UserType == UserTypes.fireSafetyEngineer)
                {
                    foreach (int authorizedObject in user.AuthorizedObjectIds)
                    {
                        results.Add(fireAlarmSystemFilter(fireAlarmSystems, authorizedObject));
                    }
                }
                if (user.UserType == UserTypes.fireFighter)
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
                        results.AddRange(serviceGroupFilter(fireAlarmSystems, authorizedObject));
                    }
                }
            }

            results.RemoveAll(x => x == null);
            results.OrderBy(x => x.Company);
            return results.Distinct();
        }

        /// <summary>
        /// Only returns FireAlarmSystems with a matching id.
        /// </summary>
        /// <param name="fireAlarmSystems">A list of FireAlarmSystems you want to filter.</param>
        /// <param name="id">The id of the FireAlarmSystem.</param>
        /// <returns>Returns the FireAlarmSystem.</returns>
        private static FireAlarmSystem fireAlarmSystemFilter(IEnumerable<FireAlarmSystem> fireAlarmSystems, int id)
        {
            if (fireAlarmSystems != null)
            {
                foreach (FireAlarmSystem fas in fireAlarmSystems)
                {
                    if (fas.Id == id)
                    {
                        return fas;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Only returns FireAlarmSystems where the FireBrigade is in the list of FireBrigades.
        /// The list of ServiceMembers is removed for safty.
        /// </summary>
        /// <param name="fireAlarmSystems">A list of FireAlarmSystems you want to filter.</param>
        /// <param name="id">The id of the FireBrigade.</param>
        /// <returns>Returns a filtered list of FireAlarmSystems.</returns>
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
                return results;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Only returns FireAlarmSystems where the ServiceGroup is in the list of ServiceGroups.
        /// The list of FireBrigades is removed for safty.
        /// </summary>
        /// <param name="fireAlarmSystems">A list of FireAlarmSystems you want to filter.</param>
        /// <param name="id">The id of the ServiceGroup.</param>
        /// <returns>Returns a filtered list of FireAlarmSystems.</returns>
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
                return results;
            }
            else
            {
                return null;
            }
        }
    }
}