using LNF.Data;
using LNF.DataAccess;
using System;

namespace LNF.Impl.Repository.Data
{
    public class ReservationFeed : IDataItem
    {
        public virtual int ReservationID { get; set; }
        public virtual int ClientID { get; set; }
        public virtual string LName { get; set; }
        public virtual string FName { get; set; }
        public virtual string UserName { get; set; }
        public virtual string Email { get; set; }
        public virtual string Invitees { get; set; }
        public virtual int ResourceID { get; set; }
        public virtual string ResourceName { get; set; }
        public virtual DateTime BeginDateTime { get; set; }
        public virtual DateTime EndDateTime { get; set; }
        public virtual DateTime? ActualBeginDateTime { get; set; }
        public virtual DateTime? ActualEndDateTime { get; set; }
        public virtual DateTime CreatedOn { get; set; }
        public virtual DateTime LastModifiedOn { get; set; }
        public virtual int ActivityID { get; set; }
        public virtual string ActivityName { get; set; }
        public virtual bool IsStarted { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual int AccountID { get; set; }
        public virtual string ShortCode { get; set; }
        public virtual string AccountName { get; set; }
        public virtual int OrgID { get; set; }
        public virtual string OrgName { get; set; }

        public virtual string GetClientDisplayName() => Clients.GetDisplayName(LName, FName);
    }
}
