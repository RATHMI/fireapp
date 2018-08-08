using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireApp.Domain {
    public class FireEvent : IEquatable<FireEvent>
    {
        private FireEvent(){}

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

        // Description of the Location/Fire Detector (e.g. Melder Büro).
        public string TargetDescription { get; set; } 

        // Type of the event that ocurred.
        public EventTypes EventType { get; set; } 

        /// <summary>
        /// Makes it easier to write the object.
        /// </summary>
        /// <returns>Returns a string that describes the FireEvent.</returns>
        public override string ToString()
        {
            return $"{this.Id.SourceId.ToString()};{this.Id.EventId.ToString()};{this.TargetId.ToString()};{this.TargetDescription};{this.TimeStamp.ToString()};{this.EventType.ToString()}";
        }

        /// <summary>
        /// Makes it easier to write the object.
        /// The description is shorter.
        /// </summary>
        /// <returns>Returns a string that describes the FireEvent.</returns>
        public string ToShortString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.Id.SourceId.ToString());
            sb.Append(";");
            sb.Append(this.Id.EventId.ToString());
            sb.Append(";");
            sb.Append(this.TargetId.ToString());
            sb.Append(";");
            if (this.TargetDescription.Length <= 20)
            {
                sb.Append(this.TargetDescription);
            }
            else
            {
                sb.Append(this.TargetDescription.Substring(0, 20));
                sb.Append("...");
            }
            sb.Append(";");
            sb.Append(this.TimeStamp.ToString());
            sb.Append(";");
            sb.Append(this.EventType.ToString());

            return sb.ToString();
        }

        /// <summary>
        /// Use the return value as headers of a CSV file.
        /// </summary>
        /// <returns>Returns a string with the names of the CSV values.</returns>
        public static string GetCsvHeader()
        {
            return "source ID;event ID;timestamp;target ID; target description;event type";
        }

        /// <summary>
        /// Turns this FireEvent into a CSV line.
        /// </summary>
        /// <returns>Returns a CSV line with the values of this FireEvent.</returns>
        public string ToCsv()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Id.SourceId);
            sb.Append(';');
            sb.Append(Id.EventId);
            sb.Append(';');
            sb.Append(TimeStamp.ToString("yyyy/MM/dd HH:mm:ss"));
            sb.Append(';');
            sb.Append(TargetId);
            sb.Append(';');
            sb.Append(TargetDescription);
            sb.Append(';');
            sb.Append(EventType);

            return sb.ToString();
        }

        public bool Equals(FireEvent other)
        {
            if (this.Id.Equals(other.Id))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }

    /// <summary>
    /// This class is needed because liteDB can not create a composite key itself.
    /// </summary>
    public class FireEventId : IEquatable<FireEventId>
    {
        public FireEventId(){}
        public FireEventId (int sourceId, int eventId)
        {
            this.SourceId = sourceId;
            this.EventId = eventId;
        }

        // Id of the FireAlarmSystem.
        public int SourceId { get; set; }

        // This id distinguishes this FireEvent from FireEvents of the same FireAlarmSystem.
        public int EventId { get; set; }

        public bool Equals(FireEventId other)
        {
            if (this.SourceId == other.SourceId && this.EventId == other.EventId)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Makes it easier to determine the type of a FireEvent.
    /// </summary>
    public enum EventTypes {
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
