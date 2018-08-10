using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;

namespace FireApp.Service.Filter
{
    /// <summary>
    /// This class provides methods to filter FireBrigades by their properties and the UserType.
    /// </summary>
    public class FireBrigadesFilter
    {
        /// <summary>
        /// Filters a list of FireBrigades according to the rights of a user.
        /// </summary>
        /// <param name="fireBrigades">A list of FireBrigades you want to filter.</param>
        /// <param name="user">The User you want the FireBrigade to filter for.</param>
        /// <returns>Returns a filtered list of FireBrigades.</returns>
        public static IEnumerable<FireBrigade> UserFilter(IEnumerable<FireBrigade> fireBrigades, User user)
        { 
            List<FireBrigade> results = new List<FireBrigade>();
            if (fireBrigades != null && user != null)
            {
                if (user.UserType == UserTypes.admin)
                {
                    results.AddRange(fireBrigades);
                }
                else
                {
                    if (user.UserType == UserTypes.fireSafetyEngineer)
                    {
                        foreach (int authorizedObject in user.AuthorizedObjectIds)
                        {
                            results.AddRange(fireAlarmSystemFilter(fireBrigades, authorizedObject));
                        }
                    }
                    else
                    {
                        if (user.UserType == UserTypes.fireFighter)
                        {
                            foreach (int authorizedObject in user.AuthorizedObjectIds)
                            {
                                results.Add(fireBrigadeFilter(fireBrigades, authorizedObject));
                            }
                        }
                    }
                }
            }

            results.RemoveAll(x => x == null);
            results.OrderBy(x => x.Name);
            return (IEnumerable<FireBrigade>)results.Distinct();
        }

        /// <summary>
        /// Only returns FireBrigades which are in the list of FireBrigades.
        /// </summary>
        /// <param name="fireBrigades">A list of FireBrigades you want to filter.</param>
        /// <param name="fireAlarmSystem">The id of the FireAlarmSystem.</param>
        /// <returns>Returns a filtered list of FireBrigades.</returns>
        private static IEnumerable<FireBrigade> fireAlarmSystemFilter(IEnumerable<FireBrigade> fireBrigades, int fireAlarmSystem)
        {
            List<FireBrigade> results = new List<FireBrigade>();
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

            if (fireBrigades != null)
            {
                // Only add FireBrigades to the result if the FireBrigade is contained in the list
                // of FireBrigades of the FireAlarmSystem.
                foreach (FireBrigade fb in fireBrigades)
                {
                    if (fas.FireBrigades.Contains(fb.Id))
                    {
                        results.Add(fb);
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
        /// Only returns FireBrigades with a matching id.
        /// </summary>
        /// <param name="fireBrigades">A list of FireAlarmSystems you want to filter.</param>
        /// <param name="id">The id of the FireBrigade.</param>
        /// <returns>Returns a filtered list of FireBrigades.</returns>
        private static FireBrigade fireBrigadeFilter(IEnumerable<FireBrigade> fireBrigades, int id)
        {
            if (fireBrigades != null)
            {
                foreach (FireBrigade fb in fireBrigades)
                {
                    if (fb.Id == id)
                    {
                        return fb;
                    }
                }
            }
            return null;
        } 
    }
}