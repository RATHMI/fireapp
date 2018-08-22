using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FireApp.Domain;
using static FireApp.Domain.FireEvent;

namespace FireApp.Domain.Extensionmethods
{

    /// <summary>
    /// Is an abstract version of a message that was sent by a fire alarm system.
    /// </summary>
    public static class FireEventExtensions
    {      
        /// <summary>
        /// Makes it easier to write the object.
        /// </summary>
        /// <returns>Returns a string that describes the FireEvent.</returns>
        public static string ToString(this FireEvent fe)
        {
            return $"{fe.Id.SourceId.ToString()};{fe.Id.EventId.ToString()};{fe.TargetId.ToString()};{fe.TargetDescription};{fe.TimeStamp.ToString()};{fe.EventType.ToString()}";
        }

        /// <summary>
        /// Makes it easier to write the object.
        /// The description is shorter.
        /// </summary>
        /// <returns>Returns a string that describes the FireEvent.</returns>
        public static string ToShortString(this FireEvent fe)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(fe.Id.SourceId.ToString());
            sb.Append(";");
            sb.Append(fe.Id.EventId.ToString());
            sb.Append(";");
            sb.Append(fe.TargetId.ToString());
            sb.Append(";");
            if (fe.TargetDescription.Length <= 20)
            {
                sb.Append(fe.TargetDescription);
            }
            else
            {
                sb.Append(fe.TargetDescription.Substring(0, 20));
                sb.Append("...");
            }
            sb.Append(";");
            sb.Append(fe.TimeStamp.ToString());
            sb.Append(";");
            sb.Append(fe.EventType.ToString());

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
        public static string ToCsv(this FireEvent fe)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(fe.Id.SourceId);
            sb.Append(';');
            sb.Append(fe.Id.EventId);
            sb.Append(';');
            sb.Append(fe.TimeStamp.ToString("yyyy/MM/dd HH:mm:ss"));
            sb.Append(';');
            sb.Append(fe.TargetId);
            sb.Append(';');
            sb.Append(fe.TargetDescription);
            sb.Append(';');
            sb.Append(fe.EventType);

            return sb.ToString();
        }

        public static bool Equals(this FireEvent fe, FireEvent other)
        {
            if (fe.Id.Equals(other.Id))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static int GetHashCode(this FireEvent fe)
        {
            return fe.Id.GetHashCode();
        }
    }

    /// <summary>
    /// This class is needed because liteDB can not create a composite key itself.
    /// </summary>
    public static class FireEventIdExtensions
    {
        public static bool Equals(this FireEventId feId, FireEventId other)
        {
            if (feId.SourceId == other.SourceId && feId.EventId == other.EventId)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static int GetHashCode(this FireEventId feId)
        {
            return feId.SourceId.GetHashCode() ^ feId.EventId.GetHashCode();
        }
    }  
}
