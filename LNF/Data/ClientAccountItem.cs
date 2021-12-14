using System;

namespace LNF.Data
{
    public class ClientAccountItem : IClientAccount
    {
        public int ClientAccountID { get; set; }
        public bool IsDefault { get; set; }
        public bool Manager { get; set; }
        public bool ClientAccountActive { get; set; }
        public int AccountID { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string ShortCode { get; set; }
        public int BillAddressID { get; set; }
        public int ShipAddressID { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceLine1 { get; set; }
        public string InvoiceLine2 { get; set; }
        public DateTime? PoEndDate { get; set; }
        public decimal? PoInitialFunds { get; set; }
        public decimal? PoRemainingFunds { get; set; }
        public bool AccountActive { get; set; }
        public int FundingSourceID { get; set; }
        public string FundingSourceName { get; set; }
        public int TechnicalFieldID { get; set; }
        public string TechnicalFieldName { get; set; }
        public int SpecialTopicID { get; set; }
        public string SpecialTopicName { get; set; }
        public int AccountTypeID { get; set; }
        public string AccountTypeName { get; set; }
        public int Communities { get; set; }
        public bool IsChecked { get; set; }
        public bool IsSafetyTest { get; set; }
        public bool RequirePasswordReset { get; set; }
        public bool ClientActive { get; set; }
        public int TechnicalInterestID { get; set; }
        public string TechnicalInterestName { get; set; }
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public int MaxChargeTypeID { get; set; }
        public string MaxChargeTypeName { get; set; }
        public long EmailRank { get; set; }
        public int ClientID { get; set; }
        public string UserName { get; set; }
        public string LName { get; set; }
        public string MName { get; set; }
        public string FName { get; set; }
        public string DisplayName => Clients.GetDisplayName(LName, FName);
        public ClientPrivilege Privs { get; set; }
        public int ClientOrgID { get; set; }
        public int ClientAddressID { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool IsManager { get; set; }
        public bool IsFinManager { get; set; }
        public DateTime? SubsidyStartDate { get; set; }
        public DateTime? NewFacultyStartDate { get; set; }
        public bool ClientOrgActive { get; set; }
        public int OrgID { get; set; }
        public string OrgName { get; set; }
        public int DefClientAddressID { get; set; }
        public int DefBillAddressID { get; set; }
        public int DefShipAddressID { get; set; }
        public bool NNINOrg { get; set; }
        public bool PrimaryOrg { get; set; }
        public bool OrgActive { get; set; }
        public int OrgTypeID { get; set; }
        public string OrgTypeName { get; set; }
        public int ChargeTypeID { get; set; }
        public string ChargeTypeName { get; set; }
        public int ChargeTypeAccountID { get; set; }
        public string Project => Accounts.GetProject(AccountNumber);
        public string NameWithShortCode => Accounts.GetNameWithShortCode(AccountName, ShortCode);
        public string FullAccountName => Accounts.GetFullAccountName(AccountName, ShortCode, OrgName);
        public bool IsRegularAccountType => Accounts.GetIsRegularAccountType(AccountTypeID);
        public bool IsStaff() => AsPrivileged().IsStaff();
        IPrivileged AsPrivileged() => this;
    }
}
