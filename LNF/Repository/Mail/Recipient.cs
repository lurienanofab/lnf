using LNF.Models.Mail;
using LNF.Repository.Data;
using System;

namespace LNF.Repository.Mail
{
    public class Recipient : IDataItem
    {
        public virtual int RecipientID { get; set; }
        public virtual Message Message { get; set; }
        public virtual int ClientID { get; set; }
        public virtual AddressType AddressType { get; set; }
        public virtual string AddressText { get; set; }
        public virtual DateTime AddressTimestamp { get; set; }

        public virtual Client GetClient()
        {
            //Note: ClientID might be zero, so this result can be null.
            return DA.Current.Single<Client>(ClientID);
        }
    }
}
