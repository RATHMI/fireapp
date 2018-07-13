using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;

namespace FireApp.Service.Filter
{
    public static class FireEventFilter
    {
        //todo: set right filter options
        private static EventTypes[] fireBrigadeFilter = { EventTypes.alarm };

        //todo: set right filter options
        private static EventTypes[] serviceMemberFilter = { EventTypes.disfunction };

        /// <summary>
        /// filters a list of FireEvents according to the rights of a user
        /// </summary>
        /// <param name="fireEvents">a list of FireEvents you want to filter</param>
        /// <param name="user">the user you want the FireEvents to filter for</param>
        /// <returns>returns a filtered list of FireEvents</returns>
        public static IEnumerable<FireEvent> UserFilter(IEnumerable<FireEvent> fireEvents, User user)
        {
            if (user.UserType == UserTypes.admin)
            {
                return fireEvents;
            }
            if (user.UserType == UserTypes.firealarmsystem)
            {
                return Filter.FireEventFilter.FireAlarmSystemFilter(fireEvents, user.AuthorizedObjectId);
            }
            if (user.UserType == UserTypes.firebrigade)
            {
                return Filter.FireEventFilter.FireBrigadeFilter(fireEvents, user.AuthorizedObjectId);
            }
            if (user.UserType == UserTypes.servicemember)
            {
                return Filter.FireEventFilter.ServiceMemberFilter(fireEvents, user.AuthorizedObjectId);
            }

            return null;
        }

        /// <summary>
        /// Only returns FireEvents that have an EventType that is free for FireBrigades
        /// </summary>
        /// <param name="fireEvents">a list of FireEvents you want to filter</param>
        /// <returns>returns a filtered list of FireEvents</returns>
        public static IEnumerable<FireEvent> FireBrigadeFilter(IEnumerable<FireEvent> fireEvents)
        {            
            return baseFilter(fireEvents, fireBrigadeFilter);
        }

        /// <summary>
        /// Only returns FireEvents that have an EventType that is free for FireBrigades and where this
        /// FireBrigade is in the list of FireBrigades of the FireAlarmSystem that sent the FireEvent
        /// </summary>
        /// <param name="fireEvents">a list of FireEvents you want to filter</param>
        /// <param name="fireBrigade">the id of a FireBrigade</param>
        /// <returns>returns a filtered list of FireEvents</returns>
        public static IEnumerable<FireEvent> FireBrigadeFilter(IEnumerable<FireEvent> fireEvents, int fireBrigade)
        {
            List<FireEvent> results = new List<FireEvent>();
            List<Int32> fireAlarmSystems = new List<Int32>();

            // get all IDs of the FireAlarmSystems where the FireBrigade is present
            foreach(FireAlarmSystem fas in LocalDatabase.GetAllFireAlarmSystems())
            {
                if (fas.CheckFireBrigade(fireBrigade))
                {
                    fireAlarmSystems.Add(fas.Id);
                }
            }

            // get all FireEvents from the FireAlarmSystems that are linked to the FireBrigade
            foreach (FireEvent fe in fireEvents)
            {
                if (fireAlarmSystems.Contains(fe.Id.SourceId))
                {
                    results.Add(fe);
                }
            }

            return baseFilter(results, fireBrigadeFilter);
        }

        /// <summary>
        /// Only returns FireEvents that have an EventType that is free for ServiceMembers
        /// </summary>
        /// <param name="fireEvents">a list of FireEvents you want to filter</param>
        /// <returns>returns a filtered list of FireEvents</returns>
        public static IEnumerable<FireEvent> ServiceMemberFilter(IEnumerable<FireEvent> fireEvents)
        {
            return baseFilter(fireEvents, serviceMemberFilter);
        }

        /// <summary>
        /// Only returns FireEvents that have an EventType that is free for ServiceMembers and where this
        /// ServiceMember is in the list of ServiceMembers of the FireAlarmSystem that sent the FireEvent
        /// </summary>
        /// <param name="fireEvents">a list of FireEvents you want to filter</param>
        /// <param name="serviceMember">the id of a ServiceMember</param>
        /// <returns>returns a filtered list of FireEvents</returns>
        public static IEnumerable<FireEvent> ServiceMemberFilter(IEnumerable<FireEvent> fireEvents, int serviceMember)
        {
            List<FireEvent> results = new List<FireEvent>();
            List<Int32> fireAlarmSystems = new List<Int32>();

            // get all IDs of the FireAlarmSystems where the ServiceMember is present
            foreach (FireAlarmSystem fas in LocalDatabase.GetAllFireAlarmSystems())
            {
                if (fas.CheckServiceMember(serviceMember))
                {
                    fireAlarmSystems.Add(fas.Id);
                }
            }

            // get all FireEvents from the FireAlarmSystems that are linked to the ServiceMember
            foreach (FireEvent fe in fireEvents)
            {
                if (fireAlarmSystems.Contains(fe.Id.SourceId))
                {
                    results.Add(fe);
                }
            }

            return baseFilter(results, serviceMemberFilter);
        }

        /// <summary>
        /// Only returns FireEvents from this FireAlarmSystem
        /// </summary>
        /// <param name="fireEvents">a list of FireEvents you want to filter</param>
        /// <param name="fireAlarmSystem">the id of the FireAlarmSystem</param>
        /// <returns>returns a filtered list of FireEvents</returns>
        public static IEnumerable<FireEvent> FireAlarmSystemFilter(IEnumerable<FireEvent> fireEvents, int fireAlarmSystem)
        {
            List<FireEvent> results = new List<FireEvent>();
            foreach (FireEvent fe in fireEvents)
            {
                if (fe.Id.SourceId == fireAlarmSystem)
                {
                    results.Add(fe);
                }
            }

            return results;
        }

        /// <summary>
        /// Only returns FireEvents that have an EventType that is in the given array of EventTypes
        /// </summary>
        /// <param name="fireEvents">a list of FireEvents you want to filter</param>
        /// <param name="types">an array of EventTypes you want the filtered FireEvents to have</param>
        /// <returns>returns a filtered list of FireEvents</returns>
        private static IEnumerable<FireEvent> baseFilter(IEnumerable<FireEvent> fireEvents, EventTypes[] types)
        {
            List<FireEvent> results = new List<FireEvent>();
            foreach (FireEvent fe in fireEvents)
            {
                if (types.Contains(fe.EventType))
                {
                    results.Add(fe);
                }
            }

            return results;
        }
    }
}