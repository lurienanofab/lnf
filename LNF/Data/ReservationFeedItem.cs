using System;

namespace LNF.Data
{
    public class ReservationFeedItem : IReservationFeed
    {
        public int ReservationID { get; set; }
        public int ClientID { get; set; }
        public string LName { get; set; }
        public string FName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Invitees { get; set; }
        public int ResourceID { get; set; }
        public string ResourceName { get; set; }
        public DateTime BeginDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public DateTime? ActualBeginDateTime { get; set; }
        public DateTime? ActualEndDateTime { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastModifiedOn { get; set; }
        public int ActivityID { get; set; }
        public string ActivityName { get; set; }
        public bool IsStarted { get; set; }
        public bool IsActive { get; set; }
        public int AccountID { get; set; }
        public string ShortCode { get; set; }
        public string AccountName { get; set; }
        public int OrgID { get; set; }
        public string OrgName { get; set; }
    }
}
