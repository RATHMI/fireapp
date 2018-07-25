using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;

namespace FireApp.Service.DatabaseOperations
{
    public static class FireAlarmSystems
    {
        /// <summary>
        /// Inserts a FireAlarmSystem into the database or updates it if it already exists.
        /// </summary>
        /// <param name="fas">The FireAlarmSystem you want to upsert.</param>
        /// <returns>Returns true if the FireAlarmSystem was inserted.</returns>
        public static bool Upsert(FireAlarmSystem fas)
        {
            if (fas != null)
            {
                LocalDatabase.UpsertFireAlarmSystem(fas);
                return DatabaseOperations.DbUpserts.UpsertFireAlarmSystem(fas);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Upserts a list of FireAlarmSystems into the database.
        /// </summary>
        /// <param name="fireAlarmSystems">The list of FireAlarmSystems you want to upsert.</param>
        /// <returns>Returns the number of upserted FireAlarmSystems.</returns>
        public static int BulkUpsert(IEnumerable<FireAlarmSystem> fireAlarmSystems)
        {
            int upserted = 0;
            if (fireAlarmSystems != null)
            {
                foreach (FireAlarmSystem fas in fireAlarmSystems)
                {
                    Upsert(fas);
                    upserted++;
                }
            }

            return upserted;
        }

        /// <summary>
        /// Checks if an id is already used by another FireAlarmSystem.
        /// </summary>
        /// <param name="id">The id you want to check.</param>
        /// <returns>Returns true if id is not used by other FireAlarmSystem.</returns>
        public static int CheckId(int id)
        {
            IEnumerable<FireAlarmSystem> all = LocalDatabase.GetAllFireAlarmSystems();

            // The highest Id of all FireAlarmSystems.
            int maxId = 0;
            int rv = id;

            foreach (FireAlarmSystem fas in all)
            {
                if (maxId < fas.Id)
                {
                    maxId = fas.Id;
                }

                if (fas.Id == id)
                {
                    rv = -1;
                }
            }

            // If the id is already used by another FireAlarmSystem
            // return a new id.
            if(rv == -1)
            {
                rv = maxId + 1;
            }

            return rv;
        }

        /// <summary>
        /// Returns all FireAlarmSystems. 
        /// </summary>
        /// <returns>Returns a list of all FireAlarmSystems.</returns>
        public static IEnumerable<FireAlarmSystem> GetAll()
        {
            return LocalDatabase.GetAllFireAlarmSystems().OrderBy(x => x.Company);
        }

        /// <summary>
        /// Returns all FireAlarmSystems with a sourceId matching the sourceId of an active FireEvent.
        /// </summary>
        /// <returns>Returns a list of all FireAlarmSystems with active FireEvents.</returns>
        public static IEnumerable<FireAlarmSystem> GetActiveFireAlarmSystems(User user)
        {
            IEnumerable<FireEvent> events;

            // Get all active FireEvents from the database.
            events = ActiveEvents.GetAll(); 

            // Filter the FireEvents according to the User.
            events = Filter.FireEventsFilter.UserFilter(events, user);

            // HashSet not other type of list because there could be several FireEvents with the same sourceId.
            HashSet<FireAlarmSystem> results = new HashSet<FireAlarmSystem>();
            FireAlarmSystem fas;

            foreach(FireEvent fe in events)
            {
                try { 
                    // Get FireAlarmSystem with matching sourceId and add to results.
                    fas = GetById(fe.Id.SourceId);
                    results.Add(fas);
                }
                catch (Exception)
                {        
                    // If there is no FireAlarmSystem with a matching sourceId.
                    // E.g. if there are FireEvents, but no FireAlarmSystem in the database.          
                    continue;
                }
            }

            return results;
        }

        /// <summary>
        /// Returns the FireAlarmSystem with a matching id.
        /// </summary>
        /// <param name="id">The id of the FireAlarmSystem you are looking for.</param>
        /// <returns>Returns a FireAlarmSystem with a matching id.</returns>
        public static FireAlarmSystem GetById(int id)
        {
            IEnumerable<FireAlarmSystem> fireAlarmSystems = LocalDatabase.GetAllFireAlarmSystems();
            foreach (FireAlarmSystem fas in fireAlarmSystems)
            {
                if (fas.Id == id)
                {
                    return fas;
                }
            }

            // If no FireAlarmSystem with a matching id was found.
            throw new KeyNotFoundException();
        }

        /// <summary>
        /// Returns a list of souceIds from FireEvents where there is no FireAlarmSystem with a matching Id.
        /// </summary>
        /// <returns>Returns a list of IDs.</returns>
        public static IEnumerable<int> GetUnregistered()
        {
            List<FireAlarmSystem> fireAlarmSystems = FireAlarmSystems.GetAll().OrderBy(x => x.Id).ToList();
            List<FireEvent> events = Events.GetAll().OrderBy(x => x.Id.SourceId).ToList();

            // Use a HashSet to prevent redundant entries.
            HashSet<int> results = new HashSet<int>();

            try
            {
                // Remove all FireEvents from the list where there is a FireAlarmSystem with a matching id.
                foreach (FireAlarmSystem fas in fireAlarmSystems)
                {
                    events.RemoveAll(x => x.Id.SourceId == fas.Id);
                }

                // "events" now contains all FireEvents with no matching FireAlarmSystem.

                // Add the sourceId of all remaining FireEvents to "results".
                foreach(FireEvent fe in events)
                {
                    results.Add(fe.Id.SourceId);
                }

                return results;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return results;
            }

        }

        public static IEnumerable<FireAlarmSystem> GetByUser(User user) //todo: comment
        {
            List<object> authorizedObjects = new List<object>();
            List<FireAlarmSystem> results = new List<FireAlarmSystem>();

            if (user.UserType == UserTypes.firebrigade)
            {
                foreach (int id in user.AuthorizedObjectIds)
                {
                    try
                    {
                        authorizedObjects.Add(DatabaseOperations.FireBrigades.GetById(id));
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }

                foreach (FireBrigade fb in authorizedObjects)
                {
                    foreach (FireAlarmSystem fas in GetAll())
                    {
                        if (fas.FireBrigades.Contains(fb.Id))
                        {
                            results.Add(fas);
                        }
                    }
                }
            }

            if (user.UserType == UserTypes.servicemember)
            {
                foreach (int id in user.AuthorizedObjectIds)
                {
                    try
                    {
                        authorizedObjects.Add(DatabaseOperations.ServiceGroups.GetById(id));
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }

                foreach (ServiceGroup sg in authorizedObjects)
                {
                    foreach (FireAlarmSystem fas in DatabaseOperations.FireAlarmSystems.GetAll())
                    {
                        if (fas.FireBrigades.Contains(sg.Id))
                        {
                            results.Add(fas);
                        }
                    }
                }
            }

            return results;
        }
    }
}