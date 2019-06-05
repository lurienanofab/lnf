using LNF.Models.Data;
using System;

namespace LNF.Models.Scheduler
{
    public class ReservationStateItem
    {
        public ReservationState State { get; set; }
        public int ReservationID { get; set; }
        public int ResourceID { get; set; }
        public string ResourceName { get; set; }
        public IClient Reserver { get; set; }
        public DateTime BeginDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public DateTime? ActualBeginDateTime { get; set; }
        public DateTime? ActualEndDateTime { get; set; }
        public IClient CurrentUser { get; set; }
        public bool IsToolEngineer { get; set; }
        public bool IsReserver { get; set; }
        public bool IsInvited { get; set; }
        public bool IsAuthorized { get; set; }
        public bool BeforeMinCancelTime { get; set; }
    }
}
