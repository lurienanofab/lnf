using System;

namespace LNF.Billing
{
    public class OrgRechargeItem : IOrgRecharge
    {
        public int OrgRechargeID { get; set; }
        public int OrgID { get; set; }
        public string OrgName { get; set; }
        public bool OrgActive { get; set; }
        public int AccountID { get; set; }
        public string AccountName { get; set; }
        public bool AccountActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime EnableDate { get; set; }
        public DateTime? DisableDate { get; set; }
    }
}
