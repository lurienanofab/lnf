using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Repository.Data
{
    public class DryBoxClient : IDataItem
    {
        public virtual int ClientAccountID { get; set; }
        public virtual int ClientOrgID { get; set; }
        public virtual int ClientID { get; set; }
        public virtual int AccountID { get; set; }
        public virtual int OrgID { get; set; }
        public virtual string LName { get; set; }
        public virtual string FName { get; set; }
        public virtual string Email { get; set; }
        public virtual string OrgName { get; set; }
        public virtual string AccountName { get; set; }
        public virtual string ShortCode { get; set; }
        public virtual bool OrgManager { get; set; }
        public virtual bool OrgFinManager { get; set; }
        public virtual bool AccountManager { get; set; }
        public virtual bool ClientActive { get; set; }
        public virtual bool ClientOrgActive { get; set; }
        public virtual bool ClientAccountActive { get; set; }
        public virtual bool AccountActive { get; set; }
        public virtual bool OrgActive { get; set; }
        public virtual bool HasDryBox { get; set; }
        public virtual DateTime? ApprovedDate { get; set; }

        public virtual string GetDisplayName()
        {
            return string.Format("{0}, {1}", FName, LName);
        }
    }
}
