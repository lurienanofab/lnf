using System;

namespace LNF.Repository.Data
{
    public class ActiveLogClientRemote : IDataItem
    {
        public virtual int LogID { get; set; }
        public virtual int Record { get; set; }
        public virtual DateTime EnableDate { get; set; }
        public virtual DateTime? DisableDate { get; set; }
        public virtual int ClientRemoteID {get;set;}
        public virtual int ClientID { get; set; }
        public virtual int AccountID { get; set; }
        public virtual int RemoteClientID { get; set; }
        public virtual string ClientLName { get; set; }
        public virtual string ClientFName { get; set; }
        public virtual string RemoteLName { get; set; }
        public virtual string RemoteFName { get; set; }
        public virtual string AccountName { get; set; }
        public virtual string AccountNumber { get; set; }
        public virtual string ShortCode { get; set; }
        public virtual string OrgName { get; set; }
    }
}
