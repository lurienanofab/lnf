using LNF.Models.Data;

namespace LNF.Repository.Data
{
    public class ClientRemoteInfo : IDataItem
    {
        public virtual int ClientRemoteID { get; set; }
        public virtual int ClientID { get; set; }
        public virtual string UserName { get; set; }
        public virtual string LName { get; set; }
        public virtual string FName { get; set; }
        public virtual string MName { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual ClientPrivilege Privs { get; set; }
        public virtual int Communities { get; set; }
        public virtual string Email { get; set; }
        public virtual string Phone { get; set; }
        public virtual bool ClientActive { get; set; }
        public virtual int RemoteClientID { get; set; }
        public virtual string RemoteUserName { get; set; }
        public virtual string RemoteLName { get; set; }
        public virtual string RemoteFName { get; set; }
        public virtual string RemoteMName { get; set; }
        public virtual string RemoteDisplayName { get; set; }
        public virtual ClientPrivilege RemotePrivs { get; set; }
        public virtual int RemoteCommunities { get; set; }
        public virtual string RemoteEmail { get; set; }
        public virtual string RemotePhone { get; set; }
        public virtual bool RemoteClientActive { get; set; }
        public virtual int AccountID { get; set; }
        public virtual string AccountName { get; set; }
        public virtual string ShortCode { get; set; }
        public virtual string Number { get; set; }
        public virtual int OrgID { get; set; }
        public virtual string OrgName { get; set; }
        public virtual bool OrgActive { get; set; }
        public virtual int ChargeTypeID { get; set; }
        public virtual string ChargeTypeName { get; set; }
        public virtual int AccountTypeID { get; set; }
        public virtual string AccountTypeName { get; set; }
        public virtual bool AccountActive { get; set; }
    }
}
