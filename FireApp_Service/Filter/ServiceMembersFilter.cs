using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;

namespace FireApp.Service.Filter
{
    public static class ServiceMembersFilter
    {
        /// <summary>
        /// filters a list of ServiceMembers according to the rights of a user
        /// </summary>
        /// <param name="serviceMembers">a list of ServiceMembers you want to filter</param>
        /// <param name="user">the user you want the FireBrigade to filter for</param>
        /// <returns>returns a filtered list of ServiceMembers</returns>
        public static IEnumerable<ServiceMember> UserFilter(IEnumerable<ServiceMember> serviceMembers, User user)
        {
            List<ServiceMember> results = new List<ServiceMember>();
            if (serviceMembers != null && user != null)
            {
                if (user.UserType == UserTypes.admin)
                {
                    results.AddRange(serviceMembers);
                }
                if (user.UserType == UserTypes.firealarmsystem)
                {
                    foreach (int authorizedObject in user.AuthorizedObjectIds)
                    {
                        results.AddRange(fireAlarmSystemFilter(serviceMembers, authorizedObject));
                    }
                }
                if (user.UserType == UserTypes.firebrigade)
                {
                    foreach (int authorizedObject in user.AuthorizedObjectIds)
                    {
                        results.AddRange(serviceMemberFilter(serviceMembers, authorizedObject));
                    }
                }
            }
            return (IEnumerable<ServiceMember>)results;
        }

        /// <summary>
        /// only returns ServiceMember which are in the list of ServiceMembers
        /// </summary>
        /// <param name="serviceMembers">a list of ServiceMembers you want to filter</param>
        /// <param name="id">the id of the FireAlarmSystem</param>
        /// <returns>returns a filtered list of ServiceMembers</returns>
        private static IEnumerable<ServiceMember> fireAlarmSystemFilter(IEnumerable<ServiceMember> serviceMembers, int id)
        {
            List<ServiceMember> results = new List<ServiceMember>();
            FireAlarmSystem fas = DatabaseOperations.FireAlarmSystems.GetFireAlarmSystemById(id).First<FireAlarmSystem>();
            if (fas != null && serviceMembers != null)
            {
                foreach (ServiceMember sm in serviceMembers)
                {
                    if (fas.ServiceMembers.Contains(sm.Id))
                    {
                        results.Add(sm);
                    }
                }
            }
            return ((IEnumerable<ServiceMember>)new List<ServiceMember>());
        }

        /// <summary>
        /// only returns ServiceMembers with a matching id
        /// </summary>
        /// <param name="serviceMembers">a list of ServiceMembers you want to filter</param>
        /// <param name="id">the id of the ServiceMember</param>
        /// <returns>returns a filtered list of ServiceMembers</returns>
        private static IEnumerable<ServiceMember> serviceMemberFilter(IEnumerable<ServiceMember> serviceMembers, int id)
        {
            if (serviceMembers != null)
            {
                foreach (ServiceMember sm in serviceMembers)
                {
                    if (sm.Id == id)
                    {
                        return ((IEnumerable<ServiceMember>)sm);
                    }
                }
            }
            return ((IEnumerable<ServiceMember>)new List<ServiceMember>());
        }
    }
}