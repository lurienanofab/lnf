using System;

namespace LNF.Billing.Apportionment.Models
{
    public class RoomEntryApportionmentAccount
    {
        public DateTime Period { get; set; }
        public int ClientID { get; set; }
        public int RoomID { get; set; }
        public int AccountID { get; set; }
        public string AccountName { get; set; }
        public string ShortCode { get; set; }
        public int OrgID { get; set; }
        public string OrgName { get; set; }
        public double Entries { get; set; }
        public double DefaultPercentage { get; set; }
    }
}
