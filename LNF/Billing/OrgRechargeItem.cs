using System;

namespace LNF.Billing
{
    public class OrgRechargeItem : IOrgRecharge
    {
        public int OrgRechargeID { get; set; }
        public int OrgID { get; set; }
        public int AccountID { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime EnableDate { get; set; }
        public DateTime? DisableDate { get; set; }
    }
}
