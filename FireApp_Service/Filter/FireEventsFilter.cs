using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;
using System.Net.Http.Headers;

namespace FireApp.Service.Filter
{
    public static class FireEventsFilter
    {
        private static EventTypes[] fireBrigadeFilterTypes = { EventTypes.alarm };

        private static EventTypes[] serviceGroupFilterTypes = { EventTypes.disfunction };

        /// <summary>
        /// filters a list of FireEvents according to the rights of a user
        /// </summary>
        /// <param name="fireEvents">a list of FireEvents you want to filter</param>
        /// <param name="user">the user you want the FireEvents to filter for</param>
        /// <returns>returns a filtered list of FireEvents</returns>
        public static IEnumerable<FireEvent> UserFilter(IEnumerable<FireEvent> fireEvents, User user)
        {
            List<FireEvent> results = new List<FireEvent>();
            if (fireEvents != null && user != null)
            {
                if (user.UserType == UserTypes.admin)
                {
                    results.AddRange(fireEvents);
                }
                if (user.UserType == UserTypes.firealarmsystem)
                {
                    foreach (int authorizedObject in user.AuthorizedObjectIds)
                    {
                        results.AddRange(fireAlarmSystemFilter(fireEvents, authorizedObject));
                    }
                }
                if (user.UserType == UserTypes.firebrigade)
                {
                    foreach (int authorizedObject in user.AuthorizedObjectIds)
                    {
                        results.AddRange(fireBrigadeFilter(fireEvents, authorizedObject));
                    }
                }
                if (user.UserType == UserTypes.servicemember)
                {
                    foreach (int authorizedObject in user.AuthorizedObjectIds)
                    {
                        results.AddRange(serviceGroupFilter(fireEvents, authorizedObject));
                    }
                }
            }

            results.OrderBy(x => x.EventType);
            return (IEnumerable<FireEvent>)results;
        }

        /// <summary>
        /// Filters the list of FireEvents by using the property values which were transfered in the headers
        /// </summary>
        /// <param name="fireEvents"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static IEnumerable<FireEvent> PropertyFilter(IEnumerable<FireEvent> fireEvents, HttpRequestHeaders headers)
        {
            //todo: implement method
            List<FireEvent> results = new List<FireEvent>();

            IEnumerable<string> keys = new List<string>();
            string key;
            List<string> eventTypes = new List<string>();

            if (headers.TryGetValues("eventtypes", out keys) != false)
            {
                headers.TryGetValues("token", out keys);
                key = keys.First<string>().Trim(new char[] { '"' });
                eventTypes.AddRange(key.Split(','));
            }

            return results;
        }

        /// <summary>
        /// Only returns FireEvents that have an EventType that is free for FireBrigades
        /// </summary>
        /// <param name="fireEvents">a list of FireEvents you want to filter</param>
        /// <returns>returns a filtered list of FireEvents</returns>
        private static IEnumerable<FireEvent> fireBrigadeFilter(IEnumerable<FireEvent> fireEvents)
        {            
            return baseFilter(fireEvents, fireBrigadeFilterTypes);
        }

        /// <summary>
        /// Only returns FireEvents that have an EventType that is free for FireBrigades and where this
        /// FireBrigade is in the list of FireBrigades of the FireAlarmSystem that sent the FireEvent
        /// </summary>
        /// <param name="fireEvents">a list of FireEvents you want to filter</param>
        /// <param name="fireBrigade">the id of a FireBrigade</param>
        /// <returns>returns a filtered list of FireEvents</returns>
        private static IEnumerable<FireEvent> fireBrigadeFilter(IEnumerable<FireEvent> fireEvents, int fireBrigade)
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

            return baseFilter(results, fireBrigadeFilterTypes);
        }

        /// <summary>
        /// Only returns FireEvents that have an EventType that is free for ServiceGroups
        /// </summary>
        /// <param name="fireEvents">a list of FireEvents you want to filter</param>
        /// <returns>returns a filtered list of FireEvents</returns>
        private static IEnumerable<FireEvent> serviceGroupFilter(IEnumerable<FireEvent> fireEvents)
        {
            return baseFilter(fireEvents, serviceGroupFilterTypes);
        }

        /// <summary>
        /// Only returns FireEvents that have an EventType that is free for ServiceGroups and where this
        /// ServiceGroup is in the list of ServiceGroups of the FireAlarmSystem that sent the FireEvent
        /// </summary>
        /// <param name="fireEvents">a list of FireEvents you want to filter</param>
        /// <param name="serviceGroup">the id of a ServiceGroup</param>
        /// <returns>returns a filtered list of FireEvents</returns>
        private static IEnumerable<FireEvent> serviceGroupFilter(IEnumerable<FireEvent> fireEvents, int serviceGroup)
        {
            List<FireEvent> results = new List<FireEvent>();
            List<Int32> fireAlarmSystems = new List<Int32>();

            // get all IDs of the FireAlarmSystems where the ServiceGroup is present
            foreach (FireAlarmSystem fas in LocalDatabase.GetAllFireAlarmSystems())
            {
                if (fas.CheckServiceGroup(serviceGroup))
                {
                    fireAlarmSystems.Add(fas.Id);
                }
            }

            // get all FireEvents from the FireAlarmSystems that are linked to the ServiceGroup
            foreach (FireEvent fe in fireEvents)
            {
                if (fireAlarmSystems.Contains(fe.Id.SourceId))
                {
                    results.Add(fe);
                }
            }

            return results;
            //return baseFilter(results, serviceGroupFilterTypes);
        }

        /// <summary>
        /// Only returns FireEvents from this FireAlarmSystem
        /// </summary>
        /// <param name="fireEvents">a list of FireEvents you want to filter</param>
        /// <param name="fireAlarmSystem">the id of the FireAlarmSystem</param>
        /// <returns>returns a filtered list of FireEvents</returns>
        private static IEnumerable<FireEvent> fireAlarmSystemFilter(IEnumerable<FireEvent> fireEvents, int fireAlarmSystem)
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
            if (fireEvents != null && types != null)
            {
                foreach (FireEvent fe in fireEvents)
                {
                    if (types.Contains(fe.EventType))
                    {
                        results.Add(fe);
                    }
                }
            }
            return results;
        }
    }
}