using LNF.DataAccess;
using System;

namespace LNF.Mail
{
    public class Recipient : IDataItem
    {
        public virtual int RecipientID { get; set; }
        public virtual int MessageID { get; set; }
        public virtual int ClientID { get; set; }
        public virtual AddressType AddressType { get; set; }
        public virtual string AddressText { get; set; }
        public virtual DateTime AddressTimestamp { get; set; }
    }
}
