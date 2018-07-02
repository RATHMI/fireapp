using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireApp.Domain {
    public class FireEvent {

       
        public FireEventId Id { get; set; } // a composite primary key consisting of source and event id

        public DateTime TimeStamp { get; set; } // Time when the FireEvent accored

        public string TargetId { get; set; } // name of the Fire detector (e.g. MG 13/5)

        public string TargetDescription { get; set; } // Description of the Location/Fire Detector (e.g. Melder Büro)
        
        public EventTypes EventType { get; set; }
    }


    // help class to use a composite primary key 
    public class FireEventId {
        public int SourceId { get; set; } // Id of the BMA (Brandmeldeanlage)
        public int EventId { get; set; } // incrementing id of the events which are raised by the BMA
    }

    public enum EventTypes { alarm, disfunction}; // ... todo
}
