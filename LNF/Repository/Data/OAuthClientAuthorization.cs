using System;

namespace LNF.Repository.Data
{
    public class OAuthClientAuthorization : IDataItem
    {
        public virtual int OAuthClientAuthorizationID { get; set; }
        public virtual OAuthClientAudience OAuthClientAudience { get; set; }
        public virtual Client Client { get; set; }
        public virtual string AuthorizationCode { get; set; }
        public virtual string RedirectUri { get; set; }
        public virtual string State { get; set; }
        public virtual DateTime Expires { get; set; }
        public virtual bool IsExchanged { get; set; }
        public virtual DateTime? ExchangedOn { get; set; }
    }
}
