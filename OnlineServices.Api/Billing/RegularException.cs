using LNF.Billing;
using LNF.Billing.Reports;
using System;

namespace OnlineServices.Api.Billing
{
    public class RegularException : IRegularException
    {
        public int AccountID { get; set; }
        public string AccountName { get; set; }
        public BillingCategory BillingCategory { get; set; }
        public int BillingID { get; set; }
        public int ClientID { get; set; }
        public string FName { get; set; }
        public int InviteeClientID { get; set; }
        public string InviteeFName { get; set; }
        public string InviteeLName { get; set; }
        public bool IsTemp { get; set; }
        public string LName { get; set; }
        public DateTime Period { get; set; }
        public int ReservationID { get; set; }
        public int ResourceID { get; set; }
        public string ResourceName { get; set; }
        public string ShortCode { get; set; }
    }
}
