using LNF.Data;
using System;

namespace LNF.Reporting
{
    public class ClientManagerLogItem : IClientManagerLog
    {
        public int ManagerLogID { get; set; }
        public string ManagerTableName { get; set; }
        public int ManagerRecord { get; set; }
        public DateTime ManagerEnableDate { get; set; }
        public DateTime? ManagerDisableDate { get; set; }
        public int UserLogID { get; set; }
        public string UserTableName { get; set; }
        public int UserRecord { get; set; }
        public DateTime UserEnableDate { get; set; }
        public DateTime? UserDisableDate { get; set; }
        public int ManagerClientID { get; set; }
        public string ManagerUserName { get; set; }
        public string ManagerLName { get; set; }
        public string ManagerFName { get; set; }
        public string ManagerEmail { get; set; }
        public int UserClientID { get; set; }
        public string UserUserName { get; set; }
        public string UserLName { get; set; }
        public string UserFName { get; set; }
        public string UserEmail { get; set; }
        public bool ManagerIsTechnicalManager { get; set; }
        public bool ManagerIsFinancialManager { get; set; }
        public bool UserIsTechnicalManager { get; set; }
        public bool UserIsFinancialManager { get; set; }
        public bool UserManager { get; set; }
        public ClientPrivilege UserPrivs { get; set; }
        public int AccountID { get; set; }
        public string AccountName { get; set; }
        public string ShortCode { get; set; }
        public string AccountNumber { get; set; }
        public int OrgID { get; set; }
        public string OrgName { get; set; }
        public bool IsSubsidyOrg { get; set; }
        public bool IsRemote { get; set; }
        public int RemoteClientClientID { get; set; }
        public string RemoteClientUserName { get; set; }
        public string RemoteClientLName { get; set; }
        public string RemoteClientFName { get; set; }
        public string RemoteClientEmail { get; set; }
    }
}
