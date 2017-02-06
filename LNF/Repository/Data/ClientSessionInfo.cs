using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace LNF.Repository.Data
{
    public class ClientSessionInfo : IDataItem
    {
        public virtual int ClientID { get; set; }
        public virtual string FName { get; set; }
        public virtual string MName { get; set; }
        public virtual string LName { get; set; }
        public virtual string UserName { get; set; }
        public virtual string Password { get; set; }
        public virtual string PasswordHash { get; set; }
        public virtual int DemCitizenID { get; set; }
        public virtual int DemGenderID { get; set; }
        public virtual int DemRaceID { get; set; }
        public virtual int DemEthnicID { get; set; }
        public virtual int DemDisabilityID { get; set; }
        public virtual int Privs { get; set; }
        public virtual int Communities { get; set; }
        public virtual int TechnicalInterestID { get; set; }
        public virtual bool Active { get; set; }
        public virtual bool? isChecked { get; set; }
        public virtual bool? isSafetyTest { get; set; }
        public virtual int OrgID { get; set; }
        public virtual string Email { get; set; }
        public virtual string Phone { get; set; }
        public virtual int? MaxChargeTypeID { get; set; }
        public virtual string DisplayName { get; set; }
    }
}
