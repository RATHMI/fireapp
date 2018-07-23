using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;

namespace FireApp.Service.Filter
{
    /// <summary>
    /// This class provide methods to filter FireBrigades by their properties and the UserType
    /// </summary>
    public class FireBrigadesFilter
    {
        /// <summary>
        /// filters a list of FireBrigades according to the rights of a user
        /// </summary>
        /// <param name="fireBrigades">a list of FireBrigades you want to filter</param>
        /// <param name="user">the user you want the FireBrigade to filter for</param>
        /// <returns>returns a filtered list of FireBrigades</returns>
        public static IEnumerable<FireBrigade> UserFilter(IEnumerable<FireBrigade> fireBrigades, User user)
        {
            List<FireBrigade> results = new List<FireBrigade>();
            if (fireBrigades != null && user != null)
            {
                if (user.UserType == UserTypes.admin)
                {
                    results.AddRange(fireBrigades);
                }
                if (user.UserType == UserTypes.firealarmsystem)
                {
                    foreach (int authorizedObject in user.AuthorizedObjectIds)
                    {
                        results.AddRange(fireAlarmSystemFilter(fireBrigades, authorizedObject));
                    }
                }
                if (user.UserType == UserTypes.firebrigade)
                {
                    foreach (int authorizedObject in user.AuthorizedObjectIds)
                    {
                        results.Add(fireBrigadeFilter(fireBrigades, authorizedObject));
                    }
                }
            }

            results.OrderBy(x => x.Name);
            return (IEnumerable<FireBrigade>)results;
        }

        /// <summary>
        /// only returns FireBrigades which are in the list of FireBrigades
        /// </summary>
        /// <param name="fireBrigades">a list of FireBrigades you want to filter</param>
        /// <param name="id">the id of the FireAlarmSystem</param>
        /// <returns>returns a filtered list of FireBrigades</returns>
        private static IEnumerable<FireBrigade> fireAlarmSystemFilter(IEnumerable<FireBrigade> fireBrigades, int id)
        {
            List<FireBrigade> results = new List<FireBrigade>();
            FireAlarmSystem fas = DatabaseOperations.FireAlarmSystems.GetById(id);
            if (fireBrigades != null) {
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
                throw new ArgumentNullException();
            }        
        }

        /// <summary>
        /// only returns FireBrigades with a matching id
        /// </summary>
        /// <param name="fireBrigades">a list of FireAlarmSystems you want to filter</param>
        /// <param name="id">the id of the FireBrigade</param>
        /// <returns>returns a filtered list of FireBrigades</returns>
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