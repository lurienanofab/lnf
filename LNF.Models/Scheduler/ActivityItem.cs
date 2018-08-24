namespace LNF.Models.Scheduler
{
    public class ActivityItem
    {
        public int ActivityID { get; set; }
        public string ActivityName { get; set; }
        public int ListOrder { get; set; }
        public bool Chargeable { get; set; }
        public bool Editable { get; set; }
        public ActivityAccountType AccountType { get; set; }
        public int UserAuth { get; set; }
        public ActivityInviteeType InviteeType { get; set; }
        public int InviteeAuth { get; set; }
        public int StartEndAuth { get; set; }
        public int NoReservFenceAuth { get; set; }
        public int NoMaxSchedAuth { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsFacilityDownTime { get; set; }
    }
}
