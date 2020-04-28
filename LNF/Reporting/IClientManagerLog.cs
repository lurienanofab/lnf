using LNF.Data;
using System;

namespace LNF.Reporting
{
    public interface IClientManagerLog
    {
        int ManagerLogID { get; set; }
        string ManagerTableName { get; set; }
        int ManagerRecord { get; set; }
        DateTime ManagerEnableDate { get; set; }
        DateTime? ManagerDisableDate { get; set; }
        int UserLogID { get; set; }
        string UserTableName { get; set; }
        int UserRecord { get; set; }
        DateTime UserEnableDate { get; set; }
        DateTime? UserDisableDate { get; set; }
        int ManagerClientID { get; set; }
        string ManagerUserName { get; set; }
        string ManagerLName { get; set; }
        string ManagerFName { get; set; }
        string ManagerEmail { get; set; }
        int UserClientID { get; set; }
        string UserUserName { get; set; }
        string UserLName { get; set; }
        string UserFName { get; set; }
        string UserEmail { get; set; }
        bool ManagerIsTechnicalManager { get; set; }
        bool ManagerIsFinancialManager { get; set; }
        bool UserIsTechnicalManager { get; set; }
        bool UserIsFinancialManager { get; set; }
        bool UserManager { get; set; }
        ClientPrivilege UserPrivs { get; set; }
        int AccountID { get; set; }
        string AccountName { get; set; }
        string ShortCode { get; set; }
        string AccountNumber { get; set; }
        int OrgID { get; set; }
        string OrgName { get; set; }
        bool IsSubsidyOrg { get; set; }
        bool IsRemote { get; set; }
        int RemoteClientClientID { get; set; }
        string RemoteClientUserName { get; set; }
        string RemoteClientLName { get; set; }
        string RemoteClientFName { get; set; }
        string RemoteClientEmail { get; set; }
    }
}
