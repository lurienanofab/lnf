using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Control;

namespace LNF.Repository.Control
{
    public class UnloadReservation : IDataItem, IControlInstance
    {
        public virtual int ReservationID { get; set; }
        public virtual int ResourceID { get; set; }
        public virtual string ResourceName { get; set; }
        public virtual int UnloadTime { get; set; }
        public virtual int Index { get; set; }
        public virtual int Point { get; set; }
        public virtual int ClientID { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual int AccountID { get; set; }
        public virtual string AccountName { get; set; }
        public virtual int ActivityID { get; set; }
        public virtual string ActivityName { get; set; }
        public virtual DateTime BeginDateTime { get; set; }
        public virtual DateTime EndDateTime { get; set; }
        public virtual DateTime? ActualBeginDateTime { get; set; }
        public virtual DateTime? ActualEndDateTime { get; set; }
        public virtual DateTime? NextReservationBeginDateTime { get; set; }

        public virtual int ActionID
        {
            get { return ResourceID; }
            set { ResourceID = value; }
        }

        public virtual string Name
        {
            get { return ResourceName; }
            set { ResourceName = value; }
        }
    }
}
