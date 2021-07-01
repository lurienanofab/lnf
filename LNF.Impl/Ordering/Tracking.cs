using LNF.Ordering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LNF.Impl.Ordering
{
    public class Tracking : ITracking
    {
        public int TrackingID { get; set; }
        public int TrackingCheckpointID { get; set; }
        public int POID { get; set; }
        public int ClientID { get; set; }
        public string TrackingData { get; set; }
        public DateTime TrackingDateTime { get; set; }
        public string CheckpointName { get; set; }
        public string LName { get; set; }
        public string FName { get; set; }
    }
}
