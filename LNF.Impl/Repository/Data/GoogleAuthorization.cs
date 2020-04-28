using LNF.DataAccess;
using System;

namespace LNF.Impl.Repository.Data
{
    public class GoogleAuthorization : IDataItem
    {
        public virtual int GoogleAuthorizationID { get; set; }
        public virtual Client Client { get; set; }
        public virtual string AccessToken { get; set; }
        public virtual DateTime? AccessTokenExpirationUtc { get; set; }
        public virtual DateTime? AccessTokenIssueDateUtc { get; set; }
        public virtual string Callback { get; set; }
        public virtual string RefreshToken { get; set; }
    }
}
