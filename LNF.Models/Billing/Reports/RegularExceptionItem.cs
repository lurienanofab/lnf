using System;

namespace LNF.Models.Billing.Reports
{
    public class RegularExceptionItem
    {
        public int BillingID { get; set; }
        public DateTime Period { get; set; }
        public BillingCategory BillingCategory { get; set; }
        public int ReservationID { get; set; }
        public int ClientID { get; set; }
        public string LName { get; set; }
        public string FName { get; set; }
        public int InviteeClientID { get; set; }
        public string InviteeLName { get; set; }
        public string InviteeFName { get; set; }
        public int ResourceID { get; set; }
        public string ResourceName { get; set; }
        public int AccountID { get; set; }
        public string AccountName { get; set; }
        public string ShortCode { get; set; }
        public bool IsTemp { get; set; }
    }
}
