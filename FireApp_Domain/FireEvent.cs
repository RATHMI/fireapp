using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireApp.Domain {
    public class FireEvent {
        public FireEvent(){}

        public FireEvent(FireEventId id, DateTime time, string targetId, string targetDescription, EventTypes eventType)
        {
            this.Id = id;
            this.TimeStamp = time;
            this.TargetId = targetId;
            this.TargetDescription = targetDescription;
            this.EventType = eventType;
        }

        // a composite primary key consisting of source and event id
        public FireEventId Id { get; set; } 

        // Time when the FireEvent accored
        public DateTime TimeStamp { get; set; } 

        // name of the Fire detector (e.g. MG 13/5)
        public string TargetId { get; set; } 

        // Description of the Location/Fire Detector (e.g. Melder Büro)
        public string TargetDescription { get; set; } 

        // type of the event that ocurred
        public EventTypes EventType { get; set; } 

        /// <summary>
        /// makes it easier to access the values and print them out
        /// </summary>
        /// <returns>returns a string that describes the FireEvent</returns>
        public override string ToString()
        {
            return $"{this.Id.SourceId.ToString()};{this.Id.EventId.ToString()};{this.TargetId.ToString()};{this.TargetDescription};{this.TimeStamp.ToString()};{this.EventType.ToString()}";
        }
    }


    /// <summary>
    /// This class is needed because liteDB can not create a composite key itself
    /// </summary>
    public class FireEventId {
        public FireEventId(){}
        public FireEventId (int sourceId, int eventId)
        {
            this.SourceId = sourceId;
            this.EventId = eventId;
        }

        // Id of the FireAlarmSystem
        public int SourceId { get; set; }

        // this id distinguishes this FireEvent from FireEvents of the same FireAlarmSystem
        public int EventId { get; set; } 
    }

    /// <summary>
    /// makes it easier to determine the type of a FireEvent
    /// </summary>
    public enum EventTypes {
        alarm = 1,
        disfunction = 2,
        test = 3,
        reset = 4,
        info = 5,
        outoforder = 6,
        prealarm = 7,
        activation = 8
    };
}
