using LNF.Mail;
using System;

namespace OnlineServices.Api.Mail
{
    public class Recipient : IRecipient
    {
        public int RecipientID { get; set; }
        public int MessageID { get; set; }
        public int ClientID { get; set; }
        public AddressType AddressType { get; set; }
        public string AddressText { get; set; }
        public DateTime AddressTimestamp { get; set; }
    }
}
