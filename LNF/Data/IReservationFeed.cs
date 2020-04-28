using System;

namespace LNF.Data
{
    public interface IReservationFeed
    {
        int AccountID { get; set; }
        string AccountName { get; set; }
        int ActivityID { get; set; }
        string ActivityName { get; set; }
        DateTime? ActualBeginDateTime { get; set; }
        DateTime? ActualEndDateTime { get; set; }
        DateTime BeginDateTime { get; set; }
        int ClientID { get; set; }
        DateTime CreatedOn { get; set; }
        string Email { get; set; }
        DateTime EndDateTime { get; set; }
        string FName { get; set; }
        string Invitees { get; set; }
        bool IsActive { get; set; }
        bool IsStarted { get; set; }
        DateTime LastModifiedOn { get; set; }
        string LName { get; set; }
        int OrgID { get; set; }
        string OrgName { get; set; }
        int ReservationID { get; set; }
        int ResourceID { get; set; }
        string ResourceName { get; set; }
        string ShortCode { get; set; }
        string UserName { get; set; }
    }
}