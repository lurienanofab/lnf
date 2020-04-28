using System;

namespace LNF.Mail
{
    public interface IRecipient
    {
        int RecipientID { get; set; }
        int MessageID { get; set; }
        int ClientID { get; set; }
        AddressType AddressType { get; set; }
        string AddressText { get; set; }
        DateTime AddressTimestamp { get; set; }
    }
}