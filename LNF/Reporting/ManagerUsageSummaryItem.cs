using LNF.Data;

namespace LNF.Reporting
{
    public class ManagerUsageSummaryItem
    {
        public static ManagerUsageSummaryItem Create(IClientManagerLog log, IManagerUsageCharge charge)
        {
            return new ManagerUsageSummaryItem(log, charge);
        }

        private ManagerUsageSummaryItem(IClientManagerLog log, IManagerUsageCharge charge)
        {
            AccountID = log.AccountID;
            ShortCode = log.ShortCode.Trim();
            AccountNumber = log.AccountNumber;
            AccountName = log.AccountName;
            OrgID = log.OrgID;
            OrgName = log.OrgName;
            IsSubsidyOrg = log.IsSubsidyOrg;
            ClientID = log.UserClientID;
            UserName = log.UserUserName;
            LName = log.UserLName;
            FName = log.UserFName;
            Email = log.UserEmail;
            Privs = log.UserPrivs;
            TotalCharge = (charge == null) ? 0 : charge.TotalCharge;
            SubsidyDiscount = (charge == null) ? 0 : charge.SubsidyDiscount;
        }

        public int AccountID { get; }
        public string ShortCode { get; }
        public string AccountNumber { get; }
        public string AccountName { get; }
        public int OrgID { get; }
        public string OrgName { get; }
        public bool IsSubsidyOrg { get; }
        public int ClientID { get; }
        public string UserName { get; }
        public string LName { get; }
        public string FName { get; }
        public string Email { get; }
        public ClientPrivilege Privs { get; }
        public double TotalCharge { get; }
        public double SubsidyDiscount { get; }
    }
}