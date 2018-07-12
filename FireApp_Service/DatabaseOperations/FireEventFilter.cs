using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FireApp.Domain;

namespace FireApp.Service.DatabaseOperations
{
    public static class FireEventFilter
    {
        //todo: set right filter options
        private static EventTypes[] fireBrigadeFilter = { EventTypes.alarm };

        //todo: set right filter options
        private static EventTypes[] serviceMemberFilter = { EventTypes.disfunction };

        //todo: comment
        public static IEnumerable<FireEvent> FireBrigadeFilter(IEnumerable<FireEvent> fireEvents)
        {            
            return baseFilter(fireEvents, fireBrigadeFilter);
        }

        //todo: comment
        public static IEnumerable<FireEvent> FireBrigadeFilter(IEnumerable<FireEvent> fireEvents, int fireBrigade)
        {
            List<FireEvent> results = new List<FireEvent>();
            List<Int32> fireAlarmSystems = new List<Int32>();

            foreach(FireAlarmSystem fas in LocalDatabase.GetAllFireAlarmSystems())
            {
                if (fas.CheckFireBrigade(fireBrigade))
                {
                    fireAlarmSystems.Add(fas.Id);
                }
            }

            foreach(FireEvent fe in fireEvents)
            {
                if (fireAlarmSystems.Contains(fe.Id.SourceId))
                {
                    results.Add(fe);
                }
            }

            return baseFilter(results, fireBrigadeFilter);
        }

        //todo: comment
        public static IEnumerable<FireEvent> ServiceMemberFilter(IEnumerable<FireEvent> fireEvents)
        {
            return baseFilter(fireEvents, serviceMemberFilter);
        }

        //todo: comment
        public static IEnumerable<FireEvent> ServiceMemberFilter(IEnumerable<FireEvent> fireEvents, int serviceMember)
        {
            List<FireEvent> results = new List<FireEvent>();
            List<Int32> fireAlarmSystems = new List<Int32>();

            foreach (FireAlarmSystem fas in LocalDatabase.GetAllFireAlarmSystems())
            {
                if (fas.CheckServiceMember(serviceMember))
                {
                    fireAlarmSystems.Add(fas.Id);
                }
            }

            foreach (FireEvent fe in fireEvents)
            {
                if (fireAlarmSystems.Contains(fe.Id.SourceId))
                {
                    results.Add(fe);
                }
            }

            return baseFilter(results, serviceMemberFilter);
        }

        //todo: comment
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