using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireApp.Domain
{
    public class Target
    {
        public Target() { }

        public Target(TargetId id, TargetState state, DateTime timeStamp)
        {
            this.Id = id;
            this.State = state;
            this.TimeStamp = timeStamp;
        }

        public TargetId Id { get; set; }

        public TargetState State { get; set; }

        public DateTime TimeStamp { get; set; }

    }

    public class TargetId
    {
        public TargetId() { }

        public TargetId(int sourceId, string target)
        {
            this.SourceId = sourceId;
            this.Target = target;
        }

        public int SourceId { get; set; }

        public string Target { get; set; }
    }

    public enum TargetState { prealarm, alarm, disfunction, outoforder}
}
