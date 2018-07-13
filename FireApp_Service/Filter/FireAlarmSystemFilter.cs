using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;

namespace FireApp.Service.Filter
{
    public static class FireAlarmSystemFilter
    {
        /// <summary>
        /// filters a list of FireEvents according to the rights of a user
        /// </summary>
        /// <param name="fireEvents">a list of FireEvents you want to filter</param>
        /// <param name="user">the user you want the FireEvents to filter for</param>
        /// <returns>returns a filtered list of FireEvents</returns>
        //public static IEnumerable<FireAlarmSystem> UserFilter(IEnumerable<FireAlarmSystem> fireAlarmSystems, User user)
        //{
        //    if (user.UserType == UserTypes.admin)
        //    {
        //        return fireAlarmSystems;
        //    }
        //    if (user.UserType == UserTypes.firealarmsystem)
        //    {
        //        return Filter.FireAlarmSystemFilter.FireAlarmSystemFilter(fireAlarmSystems, user.AuthorizedObjectId);
        //    }
        //    if (user.UserType == UserTypes.firebrigade)
        //    {
        //        return Filter.FireAlarmSystemFilter.FireBrigadeFilter(fireAlarmSystems, user.AuthorizedObjectId);
        //    }
        //    if (user.UserType == UserTypes.servicemember)
        //    {
        //        return Filter.FireAlarmSystemFilter.ServiceMemberFilter(fireAlarmSystems, user.AuthorizedObjectId);
        //    }

        //    return null;
        //}

        //public static IEnumerable<FireAlarmSystem> FireAlarmSystemFilter(IEnumerable<FireAlarmSystem> fireAlarmSystems, int id)
        //{
        //    foreach(FireAlarmSystem fas in fireAlarmSystems)
        //    {
        //        if(fas.Id == id)
        //        {
        //            return (IEnumerable<)fas;
        //        }
        //    }
        //}
    }
}