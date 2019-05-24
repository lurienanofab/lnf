using System;

namespace LNF.Models.Mail
{
    public interface IRecipient
    {
        string AddressText { get; set; }
        DateTime AddressTimestamp { get; set; }
        AddressType AddressType { get; set; }
        int ClientID { get; set; }
        int MessageID { get; set; }
        int RecipientID { get; set; }
    }
}