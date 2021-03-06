﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;
using FireApp.Service.Cache;

namespace FireApp.Service
{
    /// <summary>
    /// This class is for storing the FireEvents in a cache.
    /// </summary>
    public static class LocalDatabase
    {
        // The key to retrieve the list of all FireEvents from the cache.
        const string allFireEventsString = "allFireEvents";

        // The key to retrieve the list of active FireEvents from the cache.
        const string activeFireEventsString = "activeFireEvents";

        // The key to retrieve the list of FireAlarmSystems from the cache.
        const string fireAlarmSystemsString = "fireAlarmSystems";

        // The key to retrieve the list of FireBrigades from the cache.
        const string fireBrigadesString = "fireBrigades";

        // The key to retrieve the list of FireBrigades from the cache.
        const string serviceGroupsString = "serviceGroups";

        // The key to retrieve the list of Users from the cache.
        const string userString = "users";

        static LocalDatabase(){}

        /// <summary>
        /// Queries domain objects from the database and stores them in the cache.
        /// </summary>
        public static void InitializeDatabase()
        {
            IEnumerable<FireEvent> events = (DatabaseOperations.LiteDB.LiteDbQueries.QueryFireEvents());
            IEnumerable<FireEvent> active = (DatabaseOperations.LiteDB.LiteDbQueries.QueryActiveFireEvents());
            IEnumerable<FireAlarmSystem> fireAlarmSystems = (DatabaseOperations.LiteDB.LiteDbQueries.QueryFireAlarmSystems());
            IEnumerable<FireBrigade> fireBrigades = (DatabaseOperations.LiteDB.LiteDbQueries.QueryFireBrigades());
            IEnumerable<ServiceGroup> serviceGroups = (DatabaseOperations.LiteDB.LiteDbQueries.QueryServiceGroups());
            IEnumerable<User> users = (DatabaseOperations.LiteDB.LiteDbQueries.QueryUsers());

            if (events != null)
            {
                GlobalCachingProvider.Instance.AddItem(allFireEventsString, events);
            }
            else
            {
                GlobalCachingProvider.Instance.AddItem(allFireEventsString, new List<FireEvent>());
            }

            if (active != null)
            {
                GlobalCachingProvider.Instance.AddItem(activeFireEventsString, active);
            }
            else
            {
                GlobalCachingProvider.Instance.AddItem(activeFireEventsString, new List<FireEvent>());
            }

            if (fireAlarmSystems != null)
            {
                GlobalCachingProvider.Instance.AddItem(fireAlarmSystemsString, fireAlarmSystems);
            }
            else
            {
                GlobalCachingProvider.Instance.AddItem(fireAlarmSystemsString, new List<FireAlarmSystem>());
            }

            if (fireBrigades != null)
            {
                GlobalCachingProvider.Instance.AddItem(fireBrigadesString, fireBrigades);
            }
            else
            {
                GlobalCachingProvider.Instance.AddItem(fireBrigadesString, new List<FireBrigade>());
            }

            if (serviceGroups != null)
            {
                GlobalCachingProvider.Instance.AddItem(serviceGroupsString, serviceGroups);
            }
            else
            {
                GlobalCachingProvider.Instance.AddItem(serviceGroupsString, new List<ServiceGroup>());
            }

            if (users != null)
            {
                GlobalCachingProvider.Instance.AddItem(userString, users);
            }
            else
            {
                GlobalCachingProvider.Instance.AddItem(userString, new List<User>());
            }
        }

        #region AllFireEvents
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns a List of all FireEvents that are stored in the cache.</returns>
        public static IEnumerable<FireEvent> GetAllFireEvents()
        {
            IEnumerable<FireEvent> rv = (IEnumerable<FireEvent>)GlobalCachingProvider.Instance.GetItem(allFireEventsString, false);
            if(rv == null)
            {
                rv = new List<FireEvent>();
            }
            return rv;
        }

        /// <summary>
        /// Inserts or updates a FireEvent in the cache.
        /// </summary>
        /// <param name="fe">FireEvent that should be stored in the cache.</param>
        public static void UpsertFireEvent(FireEvent fe)
        {
            if (fe != null)
            {
                List<FireEvent> allFireEvents = GetAllFireEvents().ToList();
                FireEvent old = null;

                foreach (FireEvent f in allFireEvents)
                {
                    if (f.Id.SourceId == fe.Id.SourceId && f.Id.EventId == fe.Id.EventId)
                    {
                        old = f;
                        break;
                    }
                }

                if (old != null)
                {
                    allFireEvents.Remove(old);
                    allFireEvents.Add(fe);
                }
                else
                {
                    allFireEvents.Add(fe);
                }

                GlobalCachingProvider.Instance.RemoveItem(allFireEventsString);
                GlobalCachingProvider.Instance.AddItem(allFireEventsString, allFireEvents);
            }
        }
        #endregion

        #region ActiveFireEvents
        /// <summary>
        /// Inserts or updates an active FireEvent in the cache.
        /// </summary>
        /// <param name="fe">Active FireEvent that should be stored in the cache.</param>
        public static void UpsertActiveFireEvent(FireEvent fe)
        {
            if (fe != null)
            {
                List<FireEvent> activeFireEvents = GetActiveFireEvents().ToList();
                FireEvent old = null;

                foreach (FireEvent f in activeFireEvents)
                {
                    if (f.Id.SourceId == fe.Id.SourceId && f.TargetId == fe.TargetId)
                    {
                        old = f;
                        break;
                    }
                }

                if (old != null)
                {
                    activeFireEvents.Remove(old);
                    activeFireEvents.Add(fe);
                }
                else
                {
                    activeFireEvents.Add(fe);
                }

                GlobalCachingProvider.Instance.RemoveItem(activeFireEventsString);
                GlobalCachingProvider.Instance.AddItem(activeFireEventsString, activeFireEvents);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns a list of all active FireEvents from the cache.</returns>
        public static IEnumerable<FireEvent> GetActiveFireEvents()
        {
            IEnumerable<FireEvent> rv = (IEnumerable<FireEvent>)GlobalCachingProvider.Instance.GetItem(activeFireEventsString, false);
            if (rv == null)
            {
                rv = new List<FireEvent>();
            }
            return rv;
        }

        /// <summary>
        /// Deletes the active FireEvent 'fe' from the cache.
        /// </summary>
        /// <param name="fe">The active FireEvent that should be deleted from the cache.</param>
        public static void DeleteActiveFireEvent(FireEvent fe)
        {
            IEnumerable<FireEvent> activeFireEvents = GetActiveFireEvents();
            FireEvent old = null;

            foreach (FireEvent f in activeFireEvents)
            {
                if (f.Id.SourceId == fe.Id.SourceId && f.TargetId == fe.TargetId)
                {
                    old = f;
                    break;
                }
            }

            if (old != null)
            {
                activeFireEvents.ToList().Remove(fe);
                GlobalCachingProvider.Instance.RemoveItem(activeFireEventsString);
                if (activeFireEvents != null)   // Server crashes if you try to insert null
                {
                    GlobalCachingProvider.Instance.AddItem(activeFireEventsString, activeFireEvents);
                }
            }
        }
        #endregion

        #region FireAlarmSystems
        /// <summary>
        /// Returns all FireAlarmSystems.
        /// </summary>
        /// <returns>Returns a List of all FireAlarmSystems that are stored in the cache.</returns>
        public static IEnumerable<FireAlarmSystem> GetAllFireAlarmSystems()
        {
            IEnumerable<FireAlarmSystem> rv = (IEnumerable<FireAlarmSystem>)GlobalCachingProvider.Instance.GetItem(fireAlarmSystemsString, false);
            if (rv == null)
            {
                rv = new List<FireAlarmSystem>();
            }
            return rv;
        }

        /// <summary>
        /// Inserts or updates a FireAlarmSystem in the cache.
        /// </summary>
        /// <param name="fireAlarmSystem">FireBrigade that should be stored in the cache.</param>
        public static void UpsertFireAlarmSystem(FireAlarmSystem fireAlarmSystem)
        {
            if (fireAlarmSystem != null)
            {
                List<FireAlarmSystem> allFireAlarmSystems = GetAllFireAlarmSystems().ToList();
                FireAlarmSystem old = null;

                foreach (FireAlarmSystem fas in allFireAlarmSystems)
                {
                    if (fas.Id == fireAlarmSystem.Id)
                    {
                        old = fas;
                        break;
                    }
                }

                if (old != null)
                {
                    allFireAlarmSystems.Remove(old);
                    allFireAlarmSystems.Add(fireAlarmSystem);
                }
                else
                {
                    allFireAlarmSystems.Add(fireAlarmSystem);
                }

                GlobalCachingProvider.Instance.RemoveItem(fireAlarmSystemsString);
                GlobalCachingProvider.Instance.AddItem(fireAlarmSystemsString, allFireAlarmSystems);
            }
        }

        /// <summary>
        /// Deletes a FireAlarmSystem from the cache.
        /// </summary>
        /// <param name="id">Id of the FireAlarmSystem you want to delete.</param>
        public static void DeleteFireAlarmSystem(int id)
        {
            List<FireAlarmSystem> allFireAlarmSystems = GetAllFireAlarmSystems().ToList();
            FireAlarmSystem old = null;

            foreach (FireAlarmSystem fas in allFireAlarmSystems)
            {
                if (fas.Id == id)
                {
                    old = fas;
                    break;
                }
            }

            if (old != null)
            {
                allFireAlarmSystems.Remove(old);
                GlobalCachingProvider.Instance.RemoveItem(fireAlarmSystemsString);
                if (allFireAlarmSystems != null)
                {
                    GlobalCachingProvider.Instance.AddItem(fireAlarmSystemsString, allFireAlarmSystems);
                }
            }            
        }
        #endregion

        #region FireBrigades
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns a List of all FireEvents that are stored in the cache.</returns>
        public static IEnumerable<FireBrigade> GetAllFireBrigades()
        {
            IEnumerable<FireBrigade> rv = (IEnumerable<FireBrigade>)GlobalCachingProvider.Instance.GetItem(fireBrigadesString, false);
            if (rv == null)
            {
                rv = new List<FireBrigade>();
            }
            return rv;
        }

        /// <summary>
        /// Inserts or updates a FireBrigade in the cache.
        /// </summary>
        /// <param name="fireBrigade">FireBrigade that should be stored in the cache.</param>
        public static void UpsertFireBrigade(FireBrigade fireBrigade)
        {
            if (fireBrigade != null)
            {
                List<FireBrigade> allFireBrigades = GetAllFireBrigades().ToList();
                FireBrigade old = null;

                foreach (FireBrigade fb in allFireBrigades)
                {
                    if (fb.Id == fireBrigade.Id)
                    {
                        old = fb;
                        break;
                    }
                }

                if (old != null)
                {
                    allFireBrigades.Remove(old);
                    allFireBrigades.Add(fireBrigade);
                }
                else
                {
                    allFireBrigades.Add(fireBrigade);
                }

                GlobalCachingProvider.Instance.RemoveItem(fireBrigadesString);
                GlobalCachingProvider.Instance.AddItem(fireBrigadesString, allFireBrigades);
            }
        }

        /// <summary>
        /// Deletes a FireBrigade from the cache and from the lists of the FireAlarmSystems.
        /// </summary>
        /// <param name="id">Id of the FireBrigade you want to delete.</param>
        public static void DeleteFireBrigade(int id)
        {
            List<FireBrigade> allFireBrigades = GetAllFireBrigades().ToList();
            FireBrigade old = null;

            foreach (FireBrigade fb in allFireBrigades)
            {
                if (fb.Id == id)
                {
                    old = fb;
                    break;
                }
            }

            if (old != null)
            {
                allFireBrigades.Remove(old);
                GlobalCachingProvider.Instance.RemoveItem(fireBrigadesString);
                if(allFireBrigades != null)
                {
                    GlobalCachingProvider.Instance.AddItem(fireBrigadesString, allFireBrigades);
                }               
            }
        }
        #endregion

        #region ServiceGroups
        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns a List of all ServiceGroups that are stored in the cache</returns>
        public static IEnumerable<ServiceGroup> GetAllServiceGroups()
        {
            IEnumerable<ServiceGroup> rv = (IEnumerable<ServiceGroup>)GlobalCachingProvider.Instance.GetItem(serviceGroupsString, false);
            if (rv == null)
            {
                rv = new List<ServiceGroup>();
            }
            return rv;
        }

        /// <summary>
        /// Inserts or updates a ServiceGroup in the cache
        /// </summary>
        /// <param name="serviceGroup">ServiceGroup that should be stored in the cache</param>
        public static void UpsertServiceGroup(ServiceGroup serviceGroup)
        {
            if (serviceGroup != null)
            {
                List<ServiceGroup> allServiceGroups = GetAllServiceGroups().ToList();
                ServiceGroup old = null;

                foreach (ServiceGroup sg in allServiceGroups)
                {
                    if (sg.Id == serviceGroup.Id)
                    {
                        old = sg;
                        break;
                    }
                }

                if (old != null)
                {
                    allServiceGroups.Remove(old);
                    allServiceGroups.Add(serviceGroup);
                }
                else
                {
                    allServiceGroups.Add(serviceGroup);
                }

                GlobalCachingProvider.Instance.RemoveItem(serviceGroupsString);
                GlobalCachingProvider.Instance.AddItem(serviceGroupsString, allServiceGroups);
            }
        }

        /// <summary>
        /// deletes a ServiceGroup from the cache and from the lists of the FireAlarmSystems
        /// </summary>
        /// <param name="id">id of the ServiceGroup you want to delete</param>
        public static void DeleteServiceGroup(int id)
        {
            List<ServiceGroup> allServiceGroups = GetAllServiceGroups().ToList();
            ServiceGroup old = null;

            foreach (ServiceGroup sg in allServiceGroups)
            {
                if (sg.Id == id)
                {
                    old = sg;
                    break;
                }
            }

            if (old != null)
            {
                allServiceGroups.Remove(old);
                GlobalCachingProvider.Instance.RemoveItem(serviceGroupsString);
                if (allServiceGroups != null)
                {
                    GlobalCachingProvider.Instance.AddItem(serviceGroupsString, allServiceGroups);
                }               
            }
          
        }
        #endregion

        #region Users
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns a List of all Users that are stored in the cache.</returns>
        public static IEnumerable<User> GetAllUsers()
        {
            IEnumerable<User> rv = (IEnumerable<User>)GlobalCachingProvider.Instance.GetItem(userString, false);
            if (rv == null)
            {
                rv = new List<User>();
            }
            return rv;
        }

        /// <summary>
        /// Inserts or updates a User in the cache.
        /// </summary>
        /// <param name="user">User that should be stored in the cache.</param>
        public static void UpsertUser(User user)
        {
            if (user != null)
            {
                List<User> allUsers = GetAllUsers().ToList();
                User old = null;

                foreach (User u in allUsers)
                {
                    if (u.Id == user.Id)
                    {
                        old = u;
                        break;
                    }
                }

                if (old != null)
                {
                    allUsers.Remove(old);
                    allUsers.Add(user);
                }
                else
                {
                    allUsers.Add(user);
                }

                GlobalCachingProvider.Instance.RemoveItem(userString);
                GlobalCachingProvider.Instance.AddItem(userString, allUsers);
            }
        }

        /// <summary>
        /// Deletes a User from the cache.
        /// </summary>
        /// <param name="username">Id of the User you want to delete.</param>
        public static void DeleteUser(string userName)
        {
            List<User> allUsers = GetAllUsers().ToList();
            User old = null;

            foreach (User u in allUsers)
            {
                if (u.Id == userName)
                {
                    old = u;
                    break;
                }
            }

            if (old != null)
            {
                allUsers.Remove(old);
                GlobalCachingProvider.Instance.RemoveItem(userString);
                if (allUsers != null)
                {
                    GlobalCachingProvider.Instance.AddItem(userString, allUsers);
                }               
            }

        }
        #endregion

    }
}