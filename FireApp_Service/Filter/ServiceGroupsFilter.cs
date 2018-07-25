using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;

namespace FireApp.Service.Filter
{
    /// <summary>
    /// This class provide methods to filter ServiceGroups by their properties and the UserType.
    /// </summary>
    public static class ServiceGroupsFilter
    {
        /// <summary>
        /// Filters a list of ServiceGroups according to the rights of a user.
        /// </summary>
        /// <param name="serviceGroups">The list of ServiceGroups you want to filter.</param>
        /// <param name="user">The user you want the ServiceGroups to filter for.</param>
        /// <returns>Returns a filtered list of ServiceGroups.</returns>
        public static IEnumerable<ServiceGroup> UserFilter(IEnumerable<ServiceGroup> serviceGroup, User user) //todo: comment
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
                        results.Add(serviceGroupFilter(serviceGroup, authorizedObject));
                    }
                }
            }

            results.OrderBy(x => x.GroupName);
            return results;
        }

        /// <summary>
        /// Only returns ServiceGroups which are in the list of ServiceGroups of the FireAlarmSystem.
        /// </summary>
        /// <param name="serviceGroups">The list of ServiceGroups you want to filter.</param>
        /// <param name="id">The id of the FireAlarmSystem.</param>
        /// <returns>Returns a filtered list of ServiceGroups.</returns>
        private static IEnumerable<ServiceGroup> fireAlarmSystemFilter(IEnumerable<ServiceGroup> serviceGroups, int id) //todo: comment
        {
            List<ServiceGroup> results = new List<ServiceGroup>();
            FireAlarmSystem fas = DatabaseOperations.BasicOperations.FireAlarmSystems.GetById(id);
            if (serviceGroups != null)
            {
                foreach (ServiceGroup sg in serviceGroups)
                {
                    if (fas.ServiceGroups.Contains(sg.Id))
                    {
                        results.Add(sg);
                    }
                }
                return results;
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// Only returns the ServiceGroup with a matching id.
        /// </summary>
        /// <param name="serviceGroups">A list of ServiceGroups you want to filter.</param>
        /// <param name="id">The id of the ServiceGroup.</param>
        /// <returns>Returns the ServiceGroup or null</returns>
        private static ServiceGroup serviceGroupFilter(IEnumerable<ServiceGroup> serviceGroups, int id)
        {
            if (serviceGroups != null)
            {
                foreach (ServiceGroup sg in serviceGroups)
                {
                    if (sg.Id == id)
                    {
                        return sg;
                    }
                }
            }
            return null;
        }
    }
}