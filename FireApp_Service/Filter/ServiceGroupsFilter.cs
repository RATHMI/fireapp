using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;

namespace FireApp.Service.Filter
{
    /// <summary>
    /// This class provides methods to filter ServiceGroups by their properties and the UserType.
    /// </summary>
    public static class ServiceGroupsFilter
    {
        /// <summary>
        /// Filters a list of ServiceGroups according to the rights of a User.
        /// </summary>
        /// <param name="serviceGroups">The list of ServiceGroups you want to filter.</param>
        /// <param name="user">The user you want the ServiceGroups to filter for.</param>
        /// <returns>Returns a filtered list of ServiceGroups.</returns>
        public static IEnumerable<ServiceGroup> UserFilter(IEnumerable<ServiceGroup> serviceGroups, User user)
        {
            List<ServiceGroup> results = new List<ServiceGroup>();
            if (serviceGroups != null && user != null)
            {
                if (user.UserType == UserTypes.admin)
                {
                    foreach(ServiceGroup sg in serviceGroups)
                    {
                        results.Add(sg);
                    }
                }
                if (user.UserType == UserTypes.firealarmsystem)
                {
                    foreach (int authorizedObject in user.AuthorizedObjectIds)
                    {
                        foreach (ServiceGroup sg in fireAlarmSystemFilter(serviceGroups, authorizedObject))
                        {
                            results.Add(sg);
                        }
                    }
                }
                if (user.UserType == UserTypes.servicemember)
                {
                    foreach (int authorizedObject in user.AuthorizedObjectIds)
                    {
                        results.Add(serviceGroupFilter(serviceGroups, authorizedObject));
                    }
                }
            }

            results.RemoveAll(x => x == null);
            results.OrderBy(x => x.GroupName);
            return results.Distinct();
        }

        /// <summary>
        /// Only returns ServiceGroups which are in the list of ServiceGroups of the FireAlarmSystem.
        /// </summary>
        /// <param name="serviceGroups">The list of ServiceGroups you want to filter.</param>
        /// <param name="fireAlarmSystem">The id of the FireAlarmSystem.</param>
        /// <returns>Returns a filtered list of ServiceGroups.</returns>
        private static IEnumerable<ServiceGroup> fireAlarmSystemFilter(IEnumerable<ServiceGroup> serviceGroups, int fireAlarmSystem)
        {
            List<ServiceGroup> results = new List<ServiceGroup>();
            FireAlarmSystem fas = null;

            try
            {
                // Get the FireAlarmSystem by its id.
                fas = DatabaseOperations.FireAlarmSystems.GetById(fireAlarmSystem);
            }
            catch (Exception)
            {
                return null;
            }

            if (serviceGroups != null)
            {
                // Only add ServiceGroups to the result if the ServiceGroup is contained in the list
                // of ServiceGroups of the FireAlarmSystem.
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
                return null;
            }
        }

        /// <summary>
        /// Only returns the ServiceGroup with a matching id.
        /// </summary>
        /// <param name="serviceGroups">A list of ServiceGroups you want to filter.</param>
        /// <param name="id">The id of the ServiceGroup.</param>
        /// <returns>Returns the ServiceGroup or null.</returns>
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