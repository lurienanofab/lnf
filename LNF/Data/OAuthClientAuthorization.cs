using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Data;

namespace LNF.Data
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
