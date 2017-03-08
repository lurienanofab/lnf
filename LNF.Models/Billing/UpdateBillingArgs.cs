using System;

namespace LNF.Models.Billing
{
    public class UpdateBillingArgs
    {
        public int ClientID { get; set; }
        public int ResourceID { get; set; }
        public int RoomID { get; set; }
        public int ItemID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public BillingCategory BillingCategory { get; set; }
    }
}