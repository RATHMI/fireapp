using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;

namespace FireApp.Service.Filter
{
    public static class ServiceGroupsFilter
    {
        /// <summary>
        /// filters a list of ServiceGroups according to the rights of a user
        /// </summary>
        /// <param name="serviceGroups">a list of ServiceGroups you want to filter</param>
        /// <param name="user">the user you want the FireBrigade to filter for</param>
        /// <returns>returns a filtered list of ServiceGroups</returns>
        public static IEnumerable<ServiceGroup> UserFilter(IEnumerable<ServiceGroup> serviceGroup, User user)
        {
            List<ServiceGroup> results = new List<ServiceGroup>();
            if (serviceGroup != null && user != null)
            {
                if (user.UserType == UserTypes.admin)
                {
                    results.AddRange(serviceGroup);
                }
                if (user.UserType == UserTypes.firealarmsystem)
                {
                    foreach (int authorizedObject in user.AuthorizedObjectIds)
                    {
                        results.AddRange(fireAlarmSystemFilter(serviceGroup, authorizedObject));
                    }
                }
                if (user.UserType == UserTypes.servicemember)
                {
                    foreach (int authorizedObject in user.AuthorizedObjectIds)
                    {
                        results.AddRange(serviceGroupFilter(serviceGroup, authorizedObject));
                    }
                }
            }

            results.OrderBy(x => x.GroupName);
            return (IEnumerable<ServiceGroup>)results;
        }

        /// <summary>
        /// only returns ServiceGroup which are in the list of ServiceGroups
        /// </summary>
        /// <param name="serviceGroups">a list of ServiceGroups you want to filter</param>
        /// <param name="id">the id of the FireAlarmSystem</param>
        /// <returns>returns a filtered list of ServiceGroups</returns>
        private static IEnumerable<ServiceGroup> fireAlarmSystemFilter(IEnumerable<ServiceGroup> serviceGroups, int id)
        {
            List<ServiceGroup> results = new List<ServiceGroup>();
            FireAlarmSystem fas = DatabaseOperations.FireAlarmSystems.GetFireAlarmSystemById(id).First<FireAlarmSystem>();
            if (fas != null && serviceGroups != null)
            {
                foreach (ServiceGroup sg in serviceGroups)
                {
                    if (fas.ServiceGroups.Contains(sg.Id))
                    {
                        results.Add(sg);
                    }
                }
            }
            return ((IEnumerable<ServiceGroup>)new List<ServiceGroup>());
        }

        /// <summary>
        /// only returns ServiceGroups with a matching id
        /// </summary>
        /// <param name="serviceGroups">a list of ServiceGroups you want to filter</param>
        /// <param name="id">the id of the ServiceGroup</param>
        /// <returns>returns a filtered list of ServiceGroups</returns>
        private static IEnumerable<ServiceGroup> serviceGroupFilter(IEnumerable<ServiceGroup> serviceGroups, int id)
        {
            if (serviceGroups != null)
            {
                foreach (ServiceGroup sg in serviceGroups)
                {
                    if (sg.Id == id)
                    {
                        return ((IEnumerable<ServiceGroup>)sg);
                    }
                }
            }
            return ((IEnumerable<ServiceGroup>)new List<ServiceGroup>());
        }
    }
}