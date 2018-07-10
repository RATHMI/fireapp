using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;
using FireApp.Service.Cache;

namespace FireApp.Service
{
    /// <summary>
    /// this class is for storing the FireEvents in a cache
    /// </summary>
    public static class LocalDatabase
    {
        // the key to retrieve the list of all FireEvents from the cache
        const string allFireEventsString = "allFireEvents";

        // the key to retrieve the list of active FireEvents from the cache
        const string activeFireEventsString = "activeFireEvents";

        // the key to retrieve the list of FireAlarmSystems from the cache
        const string fireAlarmSystemsString = "fireAlarmSystems";

        // the key to retrieve the list of FireBrigades from the cache
        const string fireBrigadesString = "fireBrigades";

        // the key to retrieve the list of FireBrigades from the cache
        const string serviceMembersString = "serviceMembers";

        static LocalDatabase(){}

        /// <summary>
        /// Queries domain objects from the database and stores them in the cache
        /// </summary>
        public static void InitializeDatabase()
        {
            List<FireEvent> events = (DatabaseOperations.QueryFireEvents()).ToList<FireEvent>();
            List<FireEvent> active = (DatabaseOperations.QueryActiveFireEvents()).ToList<FireEvent>();
            List<FireAlarmSystem> fireAlarmSystems = (DatabaseOperations.QueryFireAlarmSystems()).ToList<FireAlarmSystem>();
            List<FireBrigade> fireBrigades = (DatabaseOperations.QueryFireBrigades()).ToList<FireBrigade>();
            List<ServiceMember> serviceMembers = (DatabaseOperations.QueryServiceMembers()).ToList<ServiceMember>();

            if (events != null)     // trying to insert null into the cache creates a server error
            {
                GlobalCachingProvider.Instance.AddItem(allFireEventsString, events);
            }
            else
            {
                GlobalCachingProvider.Instance.AddItem(allFireEventsString, new List<FireEvent>());
            }

            if (active != null)  // trying to insert null into the cache creates a server error
            {
                GlobalCachingProvider.Instance.AddItem(activeFireEventsString, active);
            }
            else
            {
                GlobalCachingProvider.Instance.AddItem(activeFireEventsString, new List<FireEvent>());
            }

            if (fireAlarmSystems != null)     // trying to insert null into the cache creates a server error
            {
                GlobalCachingProvider.Instance.AddItem(fireAlarmSystemsString, fireAlarmSystems);
            }
            else
            {
                GlobalCachingProvider.Instance.AddItem(fireAlarmSystemsString, new List<FireEvent>());
            }

            if (fireBrigades != null)     // trying to insert null into the cache creates a server error
            {
                GlobalCachingProvider.Instance.AddItem(fireBrigadesString, fireBrigades);
            }
            else
            {
                GlobalCachingProvider.Instance.AddItem(fireBrigadesString, new List<FireEvent>());
            }

            if (serviceMembers != null)     // trying to insert null into the cache creates a server error
            {
                GlobalCachingProvider.Instance.AddItem(serviceMembersString, serviceMembers);
            }
            else
            {
                GlobalCachingProvider.Instance.AddItem(serviceMembersString, new List<FireEvent>());
            }
        }

        #region AllFireEvents
        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns a List of all FireEvents that are stored in the cache</returns>
        public static List<FireEvent> GetAllFireEvents()
        {
            List<FireEvent> rv = (List<FireEvent>)GlobalCachingProvider.Instance.GetItem(allFireEventsString, false);
            if(rv == null)
            {
                rv = new List<FireEvent>();
            }
            return rv;
        }

        /// <summary>
        /// Inserts or updates a FireEvent in the cache
        /// </summary>
        /// <param name="fe">FireEvent that should be stored in the cache</param>
        public static void UpsertFireEvent(FireEvent fe)
        {
            List<FireEvent> allFireEvents = GetAllFireEvents();
            FireEvent old = null;

            foreach (FireEvent f in allFireEvents)
            {
                if (f.Id.SourceId == fe.Id.SourceId && f.TargetId == fe.TargetId)
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

            GlobalCachingProvider.Instance.RemoveItem(activeFireEventsString);
            GlobalCachingProvider.Instance.AddItem(activeFireEventsString, allFireEvents);
        }
        #endregion

        #region ActiveFireEvents
        /// <summary>
        /// Inserts or updates an active FireEvent in the cache
        /// </summary>
        /// <param name="fe">active FireEvent that should be stored in the cache</param>
        public static void UpsertActiveFireEvent(FireEvent fe)
        {
            List<FireEvent> activeFireEvents = GetActiveFireEvents();
            FireEvent old = null;

            foreach (FireEvent f in activeFireEvents)
            {
                if(f.Id.SourceId == fe.Id.SourceId && f.TargetId == fe.TargetId)
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns a list of all active FireEvents from the cache</returns>
        public static List<FireEvent> GetActiveFireEvents()
        {
            List<FireEvent> rv = (List<FireEvent>)GlobalCachingProvider.Instance.GetItem(activeFireEventsString, false);
            if (rv == null)
            {
                rv = new List<FireEvent>();
            }
            return rv;
        }

        /// <summary>
        /// Deletes the active FireEvent 'fe' from the cache
        /// </summary>
        /// <param name="fe">The active FireEvent that should be deleted from the cache</param>
        public static void DeleteActiveFireEvent(FireEvent fe)
        {
            List<FireEvent> activeFireEvents = GetActiveFireEvents();
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
                activeFireEvents.Remove(fe);
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
        /// 
        /// </summary>
        /// <returns>returns a List of all FireAlarmSystems that are stored in the cache</returns>
        public static List<FireAlarmSystem> GetAllFireAlarmSystems()
        {
            List<FireAlarmSystem> rv = (List<FireAlarmSystem>)GlobalCachingProvider.Instance.GetItem(fireAlarmSystemsString, false);
            if (rv == null)
            {
                rv = new List<FireAlarmSystem>();
            }
            return rv;
        }

        /// <summary>
        /// Inserts or updates a FireAlarmSystem in the cache
        /// </summary>
        /// <param name="fireAlarmSystem">FireBrigade that should be stored in the cache</param>
        public static void UpsertFireAlarmSystem(FireAlarmSystem fireAlarmSystem)
        {
            List<FireAlarmSystem> allFireAlarmSystems = GetAllFireAlarmSystems();
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

        /// <summary>
        /// deletes a FireAlarmSystem from the cache
        /// </summary>
        /// <param name="id">id of the FireAlarmSystem you want to delete</param>
        public static void DeleteFireAlarmSystem(int id)
        {
            List<FireAlarmSystem> allFireAlarmSystems = GetAllFireAlarmSystems();
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
        /// <returns>returns a List of all FireEvents that are stored in the cache</returns>
        public static List<FireBrigade> GetAllFireBrigades()
        {
            List<FireBrigade> rv = (List<FireBrigade>)GlobalCachingProvider.Instance.GetItem(fireBrigadesString, false);
            if (rv == null)
            {
                rv = new List<FireBrigade>();
            }
            return rv;
        }

        /// <summary>
        /// Inserts or updates a FireBrigade in the cache
        /// </summary>
        /// <param name="fireBrigade">FireBrigade that should be stored in the cache</param>
        public static void UpsertFireBrigade(FireBrigade fireBrigade)
        {
            List<FireBrigade> allFireBrigades = GetAllFireBrigades();
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

        /// <summary>
        /// deletes a FireBrigade from the cache and from the lists of the FireAlarmSystems
        /// </summary>
        /// <param name="id">id of the FireBrigade you want to delete</param>
        public static void DeleteFireBrigade(int id)
        {
            List<FireBrigade> allFireBrigades = GetAllFireBrigades();
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

                foreach(FireAlarmSystem fas in LocalDatabase.GetAllFireAlarmSystems())
                {
                    if (fas.FireBrigades.Contains(id))
                    {
                        fas.FireBrigades.Remove(id);
                        LocalDatabase.UpsertFireAlarmSystem(fas);
                    }
                }
            }
        }
        #endregion

        #region ServiceMembers
        /// <summary>
        /// 
        /// </summary>
        /// <returns>returns a List of all ServiceMembers that are stored in the cache</returns>
        public static List<ServiceMember> GetAllServiceMembers()
        {
            List<ServiceMember> rv = (List<ServiceMember>)GlobalCachingProvider.Instance.GetItem(serviceMembersString, false);
            if (rv == null)
            {
                rv = new List<ServiceMember>();
            }
            return rv;
        }

        /// <summary>
        /// Inserts or updates a ServiceMember in the cache
        /// </summary>
        /// <param name="serviceMember">ServiceMember that should be stored in the cache</param>
        public static void UpsertServiceMember(ServiceMember serviceMember)
        {
            List<ServiceMember> allServiceMembers = GetAllServiceMembers();
            ServiceMember old = null;

            foreach (ServiceMember sm in allServiceMembers)
            {
                if (sm.Id == serviceMember.Id)
                {
                    old = sm;
                    break;
                }
            }

            if (old != null)
            {
                allServiceMembers.Remove(old);
                allServiceMembers.Add(serviceMember);
            }
            else
            {
                allServiceMembers.Add(serviceMember);
            }

            GlobalCachingProvider.Instance.RemoveItem(serviceMembersString);
            GlobalCachingProvider.Instance.AddItem(serviceMembersString, allServiceMembers);
        }

        /// <summary>
        /// deletes a ServiceMember from the cache and from the lists of the FireAlarmSystems
        /// </summary>
        /// <param name="id">id of the ServiceMember you want to delete</param>
        public static void DeleteServiceMember(int id)
        {
            List<ServiceMember> allServiceMembers = GetAllServiceMembers();
            ServiceMember old = null;

            foreach (ServiceMember sm in allServiceMembers)
            {
                if (sm.Id == id)
                {
                    old = sm;
                    break;
                }
            }

            if (old != null)
            {
                allServiceMembers.Remove(old);
                GlobalCachingProvider.Instance.RemoveItem(serviceMembersString);
                if (allServiceMembers != null)
                {
                    GlobalCachingProvider.Instance.AddItem(serviceMembersString, allServiceMembers);
                }

                foreach (FireAlarmSystem fas in LocalDatabase.GetAllFireAlarmSystems())
                {
                    if (fas.ServiceMembers.Contains(id))
                    {
                        fas.ServiceMembers.Remove(id);
                        LocalDatabase.UpsertFireAlarmSystem(fas);
                    }
                }
            }
          
        }
        #endregion

    }
}