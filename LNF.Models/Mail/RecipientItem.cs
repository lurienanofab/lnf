using System;

namespace LNF.Models.Mail
{
    public class RecipientItem : IRecipient
    {
        public int RecipientID { get; set; }
        public int MessageID { get; set; }
        public int ClientID { get; set; }
        public AddressType AddressType { get; set; }
        public string AddressText { get; set; }
        public DateTime AddressTimestamp { get; set; }
    }
}
