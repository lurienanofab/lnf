using System;

namespace LNF.Models.Billing.Reports
{
    public interface IRegularException
    {
        int AccountID { get; set; }
        string AccountName { get; set; }
        BillingCategory BillingCategory { get; set; }
        int BillingID { get; set; }
        int ClientID { get; set; }
        string FName { get; set; }
        int InviteeClientID { get; set; }
        string InviteeFName { get; set; }
        string InviteeLName { get; set; }
        bool IsTemp { get; set; }
        string LName { get; set; }
        DateTime Period { get; set; }
        int ReservationID { get; set; }
        int ResourceID { get; set; }
        string ResourceName { get; set; }
        string ShortCode { get; set; }
    }
}