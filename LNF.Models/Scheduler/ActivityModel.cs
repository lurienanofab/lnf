

namespace LNF.Models.Scheduler
{
    public class ActivityModel
    {
        public int ActivityID { get; set; }
        public string ActivityName { get; set; }
        public int ListOrder { get; set; }
        public bool Chargeable { get; set; }
        public bool Editable { get; set; }
        public ActivityAccountType AccountType { get; set; }
        public int UserAuth { get; set; }
        public int InviteeType { get; set; }
        public int InviteeAuth { get; set; }
        public int StartEndAuth { get; set; }
        public int NoReservFenceAuth { get; set; }
        public int NoMaxSchedAuth { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsFacilityDownTime { get; set; }

        //public static ActivityModel Create(Activity source)
        //{
        //    return new ActivityModel()
        //    {
        //        ActivityID = source.ActivityID,
        //        ActivityName = source.ActivityName,
        //        ListOrder = source.ListOrder,
        //        Chargeable = source.Chargeable,
        //        Editable = source.Editable,
        //        AccountType = source.AccountType,
        //        UserAuth = source.UserAuth,
        //        InviteeType = source.InviteeType,
        //        InviteeAuth = source.InviteeAuth,
        //        StartEndAuth = source.StartEndAuth,
        //        NoReservFenceAuth = source.NoReservFenceAuth,
        //        NoMaxSchedAuth = source.NoMaxSchedAuth,
        //        Description = source.Description,
        //        IsActive = source.IsActive,
        //        IsFacilityDownTime = source.IsFacilityDownTime
        //    };
        //}
    }
}
