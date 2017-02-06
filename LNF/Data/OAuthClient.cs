using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using LNF.Repository;

namespace LNF.Data
{
    public class OAuthClient : IDataItem
    {
        public virtual int OAuthClientID { get; set; }
        public virtual string Email { get; set; }
        public virtual string Password { get; set; }
        public virtual DateTime CreatedDateTime { get; set; }
        public virtual string VerificationType { get; set; }
        public virtual string VerificationCode { get; set; }
        public virtual DateTime? VerificationDateTime { get; set; }
        public virtual bool Active { get; set; }
        public virtual bool Deleted { get; set; }
        public virtual IList<OAuthClientAudience> Audiences { get; set; }

        public OAuthClient()
        {
            Audiences = new List<OAuthClientAudience>();
        }
    }
}
