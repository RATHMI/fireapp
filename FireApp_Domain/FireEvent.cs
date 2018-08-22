using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireApp.Domain {

    /// <summary>
    /// Is an abstract version of a message that was sent by a fire alarm system.
    /// </summary>
    public class FireEvent
    {
        private FireEvent() { }

        public FireEvent(FireEventId id, DateTime time, string targetId, string targetDescription, EventTypes eventType)
        {
            this.Id = id;
            this.TimeStamp = time;
            this.TargetId = targetId;
            this.TargetDescription = targetDescription;
            this.EventType = eventType;
        }

        // A composite primary key consisting of sourceId and eventId.
        public FireEventId Id { get; set; }

        // Time when the FireEvent accored.
        public DateTime TimeStamp { get; set; }

        // Name of the Fire detector (e.g. MG 13/5).
        public string TargetId { get; set; }

        // Description/location of the fire detector (e.g. Melder Büro).
        public string TargetDescription { get; set; }

        // Type of the event that ocurred.
        public EventTypes EventType { get; set; }

        /// <summary>
        /// This class is needed because liteDB can not create a composite key itself.
        /// </summary>
        public class FireEventId
        {
            public FireEventId() { }
            public FireEventId(int sourceId, int eventId)
            {
                this.SourceId = sourceId;
                this.EventId = eventId;
            }

            // Id of the FireAlarmSystem.
            public int SourceId { get; set; }

            // This id distinguishes this FireEvent from FireEvents of the same FireAlarmSystem.
            public int EventId { get; set; }
        }
    }

    /// <summary>
    /// Makes it easier to determine the type of a FireEvent.
    /// </summary>
    public enum EventTypes
    {
        alarm = 1,
        disfunction = 2,
        test = 3,
        reset = 4,
        info = 5,
        deactivated = 6,
        prealarm = 7,
        activation = 8
    };
}
