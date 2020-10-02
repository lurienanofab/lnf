using System;
using LNF.Data;
using LNF.DataAccess;

namespace LNF.Impl.Repository.Data
{
    public class PasswordResetRequest : IPasswordResetRequest, IDataItem
    {
        public virtual int PasswordResetRequestID { get; set; }
        public virtual int ClientID { get; set; }
        public virtual DateTime RequestDateTime { get; set; }
        public virtual string ResetCode { get; set; }
        public virtual DateTime? ResetDateTime { get; set; }
    }
}
