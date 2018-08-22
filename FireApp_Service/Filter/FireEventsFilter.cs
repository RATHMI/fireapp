using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;
using System.Net.Http.Headers;
using static FireApp.Domain.FireEvent;
using static FireApp.Domain.User;
using FireApp.Domain.Extensionmethods;

namespace FireApp.Service.Filter
{
    /// <summary>
    /// This class provide methods to filter FireEvents by their properties and the UserType
    /// </summary>
    public static class FireEventsFilter
    {
        private static EventTypes[] fireBrigadeFilterTypes = { EventTypes.alarm };

        private static EventTypes[] serviceGroupFilterTypes = { EventTypes.disfunction };

        /// <summary>
        /// Filters a list according to the values of the headers
        /// </summary>
        /// <param name="events">a list of FireEvents you want to filter</param>
        /// <param name="headers">the headers of a HttpRequest</param>
        /// <returns>returns the filtered list of FireEvents</returns>
        public static IEnumerable<FireEvent> HeadersFilter(IEnumerable<FireEvent> events, HttpRequestHeaders headers)
        {
            // Date1 used for filter by timestamp.
            DateTime date1 = DateTime.MaxValue;

            // Date2 used for filter by timestamp.
            DateTime date2 = DateTime.MaxValue;

            // Datestring is used for filter by timestamp.
            // Is for containing the split string of a date.
            string[] datestring = null;

            // EventTypes used for filter by EventType.
            List<EventTypes> eventTypes = new List<EventTypes>();

            // Types is used for filter by EventType.
            // Is for containing the split string of EventTypes.
            string[] types = null;

            // Key is for storing the value of a header.
            IEnumerable<string> key = null;

            // SourceId is used for filter by sourceId.
            // Contains the sourceId of the FireEvents you want as output.
            int sourceId = -1;

            try
            {
                // Filter events by timestamp.
                // *************************************************************************************************************
                if (headers.TryGetValues("startDate", out key) != false 
                    && headers.TryGetValues("endDate", out key) != false)
                {
                    try
                    {
                        // Get first date from the header and convert it to a new DateTime object.
                        headers.TryGetValues("startDate", out key);
                        datestring = key.First<string>().Trim(new char[] { '"' }).Split('-');
                        date1 = new DateTime(
                            Convert.ToInt32(datestring[0]), 
                            Convert.ToInt32(datestring[1]), 
                            Convert.ToInt32(datestring[2]));

                        // Get second date from the header and convert it to a new DateTime object.
                        headers.TryGetValues("endDate", out key);
                        datestring = key.First<string>().Trim(new char[] { '"' }).Split('-');
                        date2 = new DateTime(
                            Convert.ToInt32(datestring[0]), 
                            Convert.ToInt32(datestring[1]), 
                            Convert.ToInt32(datestring[2]));

                        // Check if the dates were changed.
                        if (date1 < DateTime.MaxValue && date2 < DateTime.MaxValue)
                        {
                            events = dateFilter(events, date1, date2);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

                // Filter events by EventType.
                // *************************************************************************************************************
                if (headers.TryGetValues("events", out key) != false)
                {
                    headers.TryGetValues("events", out key);

                    // Trim unwanted chars that could cause a problem when splitting the string.
                    // Split string with EventTypes into seperate strings.
                    types = (key.First<string>().Trim(new char[] { '"', ',' })).Split(',');

                    // If types is not null and is not an empty array.
                    if (types != null && types.Count() > 0) 
                    {
                        // Skip if you want all FireEvents or types is an empty string.
                        if (types[0] != "All" && types[0] != "")    
                        {
                            foreach (string s in types)
                            {
                                switch (s)
                                {
                                    case "Activation": eventTypes.Add(EventTypes.activation); break;
                                    case "Alarm": eventTypes.Add(EventTypes.alarm); break;
                                    case "Deactivated": eventTypes.Add(EventTypes.deactivated); break;
                                    case "Disfunction": eventTypes.Add(EventTypes.disfunction); break;
                                    case "Info": eventTypes.Add(EventTypes.info); break;
                                    case "Prealarm": eventTypes.Add(EventTypes.prealarm); break;
                                    case "Reset": eventTypes.Add(EventTypes.reset); break;
                                    case "Test": eventTypes.Add(EventTypes.test); break;
                                }
                            }
                            events = eventTypeFilter(events, eventTypes.ToArray<EventTypes>());
                        }
                    }
                }

                // Filter events by sourceId.
                // *************************************************************************************************************
                if (headers.TryGetValues("sourceId", out key) != false)
                {
                    try
                    {
                        // Get sourceId from the header.
                        headers.TryGetValues("sourceId", out key);
                        sourceId = Convert.ToInt32(key.First<string>().Trim('"'));
                        if (sourceId != -1)
                        {
                            events = fireAlarmSystemFilter(events, sourceId);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

                // To prevent optimisation.
                Console.WriteLine(key);  
                
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return events.Distinct();
        }

        /// <summary>
        /// This method returns FireEvents with an EventType that is matching an EventType of types.
        /// </summary>
        /// <param name="fireEvents">A list of FireEvents you want to filter.</param>
        /// <param name="types">An array of EventType.</param>
        /// <returns>Returns a filtered list of FireEvents.</returns>
        private static IEnumerable<FireEvent> eventTypeFilter(IEnumerable<FireEvent> fireEvents, EventTypes[] types)
        {
            return baseFilter(fireEvents, types).Distinct();
        }

        /// <summary>
        /// This method returns FireEvents where the date of the timestamp is between date1 and date2.
        /// </summary>
        /// <param name="fireEvents">A list of FireEvents you want to filter.</param>
        /// <param name="date1">Startdate or enddate.</param>
        /// <param name="date2">Startdate or enddate.</param>
        /// <returns>Returns a filtered list of FireEvents.</returns>
        private static IEnumerable<FireEvent> dateFilter(IEnumerable<FireEvent> fireEvents, DateTime date1, DateTime date2)
        {
            List<FireEvent> results = new List<FireEvent>();
            if (fireEvents != null && date1 != null && date2 != null)
            {
                DateTime newest;
                DateTime oldest;
                if(date1 < date2)
                {
                    oldest = date1.Date;
                    newest = date2.Date;
                }
                else
                {
                    oldest = date2.Date;
                    newest = date1.Date;
                }

                foreach (FireEvent fe in fireEvents)
                {
                    // Used property date, because if date1 and date2 have the same value there would be no match.
                    if (fe.TimeStamp.Date <= newest.Date && fe.TimeStamp.Date >= oldest.Date)
                    {
                        results.Add(fe);
                    }
                }
            }
            else
            {
                throw new ArgumentNullException();
            }
            return results.Distinct();
        }

        /// <summary>
        /// Filters a list of FireEvents according to the rights of a User.
        /// </summary>
        /// <param name="fireEvents">A list of FireEvents you want to filter.</param>
        /// <param name="user">The user you want the FireEvents to filter for.</param>
        /// <returns>Returns a filtered list of FireEvents.</returns>
        public static IEnumerable<FireEvent> UserFilter(IEnumerable<FireEvent> fireEvents, User user)
        {
            List<FireEvent> results = new List<FireEvent>();
            if (fireEvents != null && user != null)
            {
                if (user.UserType == UserTypes.admin)
                {
                    results.AddRange(fireEvents);
                }
                if (user.UserType == UserTypes.fireSafetyEngineer)
                {
                    foreach (int authorizedObject in user.AuthorizedObjectIds)
                    {
                        results.AddRange(fireAlarmSystemFilter(fireEvents, authorizedObject));
                    }
                }
                if (user.UserType == UserTypes.fireFighter)
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

            results.RemoveAll(x => x == null);
            results.OrderBy(x => x.EventType);
            return (IEnumerable<FireEvent>)results.Distinct();
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
        /// Only returns FireEvents that have an EventType that ServiceGroups are allowed to see and where this
        /// ServiceGroup is in the list of ServiceGroups of the FireAlarmSystem that sent the FireEvent.
        /// </summary>
        /// <param name="fireEvents">A list of FireEvents you want to filter.</param>
        /// <param name="serviceGroup">The id of a ServiceGroup.</param>
        /// <returns>Returns a filtered list of FireEvents.</returns>
        private static IEnumerable<FireEvent> serviceGroupFilter(IEnumerable<FireEvent> fireEvents, int serviceGroup)
        {
            List<FireEvent> results = new List<FireEvent>();
            List<Int32> fireAlarmSystems = new List<Int32>();

            // Get all IDs of the FireAlarmSystems where the ServiceGroup is in the list of ServiceGroups.
            foreach (FireAlarmSystem fas in LocalDatabase.GetAllFireAlarmSystems())
            {
                if (fas.CheckServiceGroup(serviceGroup))
                {
                    fireAlarmSystems.Add(fas.Id);
                }
            }

            // Get all FireEvents from the FireAlarmSystems that are linked to the FireAlarmSystem.
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
        /// Only returns FireEvents from this FireAlarmSystem.
        /// </summary>
        /// <param name="fireEvents">A list of FireEvents you want to filter.</param>
        /// <param name="fireAlarmSystem">The id of the FireAlarmSystem.</param>
        /// <returns>Returns a filtered list of FireEvents.</returns>
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
        /// Only returns FireEvents that have an EventType that is in the given array of EventTypes.
        /// </summary>
        /// <param name="fireEvents">A list of FireEvents you want to filter.</param>
        /// <param name="types">An array of EventTypes you want the filtered FireEvents to have.</param>
        /// <returns>Returns a filtered list of FireEvents.</returns>
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