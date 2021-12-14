using LNF.Billing;
using LNF.CommonTools;
using LNF.Data;
using LNF.Reporting.Individual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace LNF.Reporting
{
    public class ReportGenerator
    {
        private readonly ClientPrivilege _includeInmanagerUsageSummaryPriv = ClientPrivilege.LabUser | ClientPrivilege.RemoteUser;

        private readonly IProvider _provider;

        public ReportGenerator(IProvider provider)
        {
            _provider = provider;
        }

        public AggregateByOrg CreateAggregateByOrg(int clientId, DateTime period)
        {
            var result = new AggregateByOrg();

            var miscUsage = _provider.Billing.Misc.GetMiscBillingCharges(period, new[] { "Tool", "Room", "Store" }, clientId: clientId, active: true);
            var roomUsage = _provider.Billing.Room.GetRoomBilling(period, clientId);
            var toolUsage = _provider.Billing.Tool.GetToolBilling(period, clientId);

            var accounts = _provider.Data.Account.GetAccounts();

            result.RoomByOrg = roomUsage.Select(x => new RoomByOrgItem()
            {
                OrgID = x.OrgID,
                OrgName = x.OrgName,
                RoomCost = x.TotalCharge,
                MiscCarge = miscUsage.Where(m => m.SUBType == "Room" && accounts.First(a => a.AccountID == m.AccountID).OrgID == x.OrgID).Sum(z => (decimal?)(Convert.ToDecimal(z.Quantity) * z.UnitCost)).GetValueOrDefault(0),
                SubsidyDiscount = x.SubsidyDiscount + miscUsage.Where(m => m.SUBType == "Room" && accounts.First(a => a.AccountID == m.AccountID).OrgID == x.OrgID).Sum(z => (decimal?)(z.SubsidyDiscount)).GetValueOrDefault(0),
            }).ToList();

            //result.RoomByOrg = new List<RoomByOrgItem>();
            result.ToolByOrg = new List<ToolByOrgItem>();
            result.StoreByOrg = new List<StoreByOrgItem>();
            result.SubsidyByOrg = new List<SubsidyByOrgItem>();

            return result;
        }

        public ManagerUsageSummary CreateManagerUsageSummary(DateTime period, IReportingClient manager, bool includeRemote)
        {
            return CreateManagerUsageSummary(period, manager.ClientID, manager.UserName, manager.LName, manager.FName, includeRemote);
        }

        public ManagerUsageSummary CreateManagerUsageSummary(DateTime period, int clientId, string username, string lname, string fname, bool includeRemote)
        {
            var items = GetManagerUsageSummaryItems(clientId, period, includeRemote);
            return CreateManagerUsageSummary(period, clientId, username, lname, fname, items);
        }

        public ManagerUsageSummary CreateManagerUsageSummary(DateTime period, IReportingClient manager, IEnumerable<ManagerUsageSummaryItem> items)
        {
            return CreateManagerUsageSummary(period, manager.ClientID, manager.UserName, manager.LName, manager.FName, items);
        }

        public ManagerUsageSummary CreateManagerUsageSummary(DateTime period, int clientId, string username, string lname, string fname, IEnumerable<ManagerUsageSummaryItem> items)
        {
            var result = new ManagerUsageSummary
            {
                ClientID = clientId,
                UserName = username,
                LName = lname,
                FName = fname,
                Period = period
            };

            /*===== Accounts =========================================*/
            var groupByAccount = items.GroupBy(x => new
            {
                x.AccountID,
                x.ShortCode,
                x.AccountNumber,
                x.AccountName,
                x.OrgName
            }).ToList();

            var accountItems = groupByAccount.Select(x => new ManagerUsageSummaryAccountItem()
            {
                AccountID = x.Key.AccountID,
                ShortCode = x.Key.ShortCode,
                AccountNumber = x.Key.AccountNumber,
                AccountName = x.Key.AccountName,
                OrgName = x.Key.OrgName,
                TotalCharge = x.Sum(g => g.TotalCharge),
                SubsidyDiscount = x.Sum(g => g.SubsidyDiscount)
            }).ToList();

            var managerUsageSummaryAccounts = accountItems.Select(x => CreateManagerUsageSummaryAccount(x, items)).OrderBy(x => x.Sort).ToList();

            result.Accounts = managerUsageSummaryAccounts;

            /*===== Clients ==========================================*/
            var groupByClient = items.GroupBy(x => new
            {
                x.ClientID,
                x.LName,
                x.FName,
                x.Privs
            }).ToList();

            var clientItems = groupByClient.Select(x => new ManagerUsageSummaryClientItem()
            {
                ClientID = x.Key.ClientID,
                LName = x.Key.LName,
                FName = x.Key.FName,
                Privs = x.Key.Privs,
                TotalCharge = x.Sum(g => g.TotalCharge),
                SubsidyDiscount = x.Sum(g => g.SubsidyDiscount)
            }).ToList();

            var clientItemsFiltered = clientItems.Where(x => x.TotalCharge != 0 || x.SubsidyDiscount != 0 || x.Privs.HasPriv(_includeInmanagerUsageSummaryPriv)).ToList();

            var managerUsageSummaryClients = clientItemsFiltered.Select(x => CreateManagerUsageSummaryClient(x, items)).OrderBy(x => x.Sort).ToList();

            result.Clients = managerUsageSummaryClients;

            /*===== Subsidy ==========================================*/
            result.ShowSubsidyColumn = items.Any(x => x.IsSubsidyOrg);

            return result;
        }

        public ManagerUsageSummaryClient CreateManagerUsageSummaryClient(ManagerUsageSummaryClientItem args, IEnumerable<ManagerUsageSummaryItem> items)
        {
            int clientId = args.ClientID;
            string name = GetClientName(args.LName, args.FName);
            string sort = GetClientSort(args.LName, args.FName);

            var comparer = new ReportingAccountItemEqualityComparer();

            return new ManagerUsageSummaryClient()
            {
                Name = name,
                Sort = sort,
                UsageCharge = args.TotalCharge,
                Subsidy = args.SubsidyDiscount,
                Accounts = items.Where(x => x.ClientID == clientId && (x.TotalCharge > 0 || x.SubsidyDiscount > 0 || x.Privs.HasPriv(_includeInmanagerUsageSummaryPriv))).Select(x => new ReportingAccountItem()
                {
                    AccountID = x.AccountID,
                    AccountName = x.AccountName,
                    AccountNumber = x.AccountNumber,
                    ShortCode = x.ShortCode.Trim(),
                    Project = AccountNumber.Parse(x.AccountNumber).Project,
                    OrgID = x.OrgID,
                    OrgName = x.OrgName
                }).Distinct(comparer).ToArray()
            };
        }

        public IEnumerable<ManagerUsageSummaryItem> GetManagerUsageSummaryItems(int clientId, DateTime period, bool includeRemote)
        {
            var logs = _provider.Reporting.ClientManagerLog.SelectByManager(clientId, period, period.AddMonths(1));

            IEnumerable<IManagerUsageCharge> charges = _provider.Reporting.ManagerUsageCharge.SelectByManager(clientId, period, includeRemote).ToList();

            var join = logs.LeftJoin(
                inner: charges,
                outerKeySelector: o => new { o.ManagerClientID, o.UserClientID, o.AccountID },
                innerKeySelector: i => new { i.ManagerClientID, i.UserClientID, i.AccountID },
                resultSelector: (o, i) => ManagerUsageSummaryItem.Create(o, i)).ToList();

            return join;
        }

        public string GetClientName(string lname, string fname)
        {
            return Clients.GetDisplayName(lname, fname);
        }

        public string GetClientSort(string lname, string fname)
        {
            return Clients.GetDisplayName(lname, fname);
        }

        public ManagerUsageSummaryAccount CreateManagerUsageSummaryAccount(ManagerUsageSummaryAccountItem item, IEnumerable<ManagerUsageSummaryItem> items)
        {
            int accountId = item.AccountID;
            string name = GetAccountName(item.ShortCode, item.AccountNumber, item.AccountName, item.OrgName);
            string sort = GetAccountSort(item.ShortCode, item.AccountNumber, item.AccountName, item.OrgName);

            var comparer = new ReportingClientItemEqualityComparer();

            return new ManagerUsageSummaryAccount()
            {
                Name = name,
                Sort = sort,
                UsageCharge = item.TotalCharge,
                Subsidy = item.SubsidyDiscount,
                Clients = items.Where(x => x.AccountID == accountId && (x.TotalCharge > 0 || x.SubsidyDiscount > 0 || x.Privs.HasPriv(_includeInmanagerUsageSummaryPriv))).Select(x => new ReportingClientItem()
                {
                    ClientID = x.ClientID,
                    UserName = x.UserName,
                    FName = x.FName,
                    LName = x.LName,
                    Email = x.Email
                }).Distinct(comparer).ToArray()
            };
        }

        public string GetAccountName(string shortCode, string accountNumber, string accountName, string orgName)
        {
            string name;

            if (string.IsNullOrEmpty(shortCode.Trim()))
            {
                name = string.Format("{0} ({1})", accountName, orgName);
            }
            else
            {
                var proj = AccountNumber.Parse(accountNumber).Project;
                name = shortCode.Trim() + "/" + proj;
            }

            return name;
        }

        public string GetAccountSort(string shortCode, string accountNumber, string accountName, string orgName)
        {
            string sort;

            if (string.IsNullOrEmpty(shortCode.Trim()))
            {
                //accountNumber is not used at this time
                sort = string.Format("2:{2}:{1}", accountNumber, accountName, orgName);
            }
            else
            {
                sort = "1:" + shortCode.Trim();
            }

            return sort;
        }

        public UserUsageSummary CreateUserUsageSummary(DateTime period, IReportingClient client)
        {
            bool showDisclaimer = false;

            var gs = _provider.Data.GlobalSetting.GetGlobalSetting("ShowUserUsageSummaryDisclaimer");

            if (gs != null)
                showDisclaimer = bool.Parse(gs.SettingValue);

            string disclaimer = showDisclaimer ? string.Format("Please note: The data for {0:MMMM} usage has not yet been finalized.", period) : null;

            //var usage = UserUsageCharge.SelectByUser(client.ClientID, period).ToList();

            var result = new UserUsageSummary()
            {
                ClientID = client.ClientID,
                UserName = client.UserName,
                LName = client.LName,
                FName = client.FName,
                Period = period,
                Created = DateTime.Now,
                Disclaimer = disclaimer,
                AggregateByOrg = CreateAggregateByOrg(client.ClientID, period)
            };

            return result;
        }

        public string GetManagerUsageDetailJson(IEnumerable<ManagerUsageDetailItem> items)
        {
            var result = items.Select(x =>
            new
            {
                x.Period,
                BillingCategory = Utility.EnumToString(x.BillingCategory),
                x.ResourceID,
                x.ResourceName,
                x.UserName,
                x.DisplayName,
                x.Account,
                x.ChargeTypeID,
                x.TotalCharge,
                x.SubsidyDiscount,
                x.SubsidyOrg
            });

            return _provider.Utility.Serialization.Json.SerializeObject(result);
        }

        public XElement GetManagerUsageDetailXml(IEnumerable<ManagerUsageDetailItem> items)
        {
            var xdoc = new XElement("table",
                items.Select(x => new XElement("row",
                    new XElement("Period", x.Period),
                    new XElement("BillingCategory", Enum.GetName(typeof(BillingCategory), x.BillingCategory)),
                    new XElement("ResourceID", x.ResourceID),
                    new XElement("ResourceName", x.ResourceName),
                    new XElement("UserName", x.UserName),
                    new XElement("DisplayName", x.DisplayName),
                    new XElement("Account", x.Account),
                    new XElement("ChargeTypeID", x.ChargeTypeID),
                    new XElement("TotalCharge", x.TotalCharge.ToString("#,##0.00")),
                    new XElement("SubsidyDiscount", x.SubsidyDiscount.ToString("#,##0.00")),
                    new XElement("SubsidyOrg", x.SubsidyOrg)
                )));


            return xdoc;
        }

        public IEnumerable<ManagerUsageDetailItem> GetManagerUsageDetailItems(DateTime sd, DateTime ed, IClient mgr, bool remote = false)
        {
            IEnumerable<IManagerUsageCharge> charges;

            if (mgr == null)
                charges = _provider.Reporting.ManagerUsageCharge.GetManagerUsageCharges(sd, ed, remote);
            else
                charges = _provider.Reporting.ManagerUsageCharge.GetManagerUsageCharges(mgr.ClientID, sd, ed, remote);

            List<ManagerUsageDetailItem> result = new List<ManagerUsageDetailItem>();

            result.AddRange(GetManagerUsageDetailItemsByCategory(charges, BillingCategory.Room, false));
            result.AddRange(GetManagerUsageDetailItemsByCategory(charges, BillingCategory.Tool, false));
            result.AddRange(GetManagerUsageDetailItemsByCategory(charges, BillingCategory.Store, true));

            return result;
        }

        private IEnumerable<ManagerUsageDetailItem> GetManagerUsageDetailItemsByCategory(IEnumerable<IManagerUsageCharge> charges, BillingCategory billingCategory, bool aggregate)
        {
            IEnumerable<ManagerUsageDetailItem> result;

            if (aggregate)
            {
                result = charges
                    .Where(x => x.BillingCategory == billingCategory)
                    .GroupBy(x => new
                    {
                        x.Period,
                        x.BillingCategory,
                        x.ManagerUserName,
                        x.UserLName,
                        x.UserFName,
                        x.ShortCode,
                        x.AccountNumber,
                        x.AccountName,
                        x.OrgName,
                        x.ChargeTypeID,
                        x.IsSubsidyOrg
                    })
                    .ToList()
                    .Select(x => new ManagerUsageDetailItem()
                    {
                        Period = x.Key.Period,
                        BillingCategory = x.Key.BillingCategory,
                        ResourceID = 0,
                        ResourceName = $"{Enum.GetName(typeof(BillingCategory), x.Key.BillingCategory)} Charge",
                        UserName = x.Key.ManagerUserName,
                        DisplayName = Clients.GetDisplayName(x.Key.UserLName, x.Key.UserFName),
                        Account = GetAccountName(x.Key.ShortCode, x.Key.AccountNumber, x.Key.AccountName, x.Key.OrgName),
                        ChargeTypeID = x.Key.ChargeTypeID,
                        Sort = x.Key.BillingCategory + ":" + GetAccountSort(x.Key.ShortCode, x.Key.AccountNumber, x.Key.AccountName, x.Key.OrgName),
                        TotalCharge = x.Sum(g => g.TotalCharge),
                        SubsidyDiscount = x.Sum(g => g.SubsidyDiscount),
                        SubsidyOrg = x.Key.IsSubsidyOrg
                    }).ToList();
            }
            else
            {
                result = charges
                    .Where(x => x.BillingCategory == billingCategory)
                    .GroupBy(x => new
                    {
                        x.Period,
                        x.BillingCategory,
                        x.ResourceID,
                        x.ResourceName,
                        x.ManagerUserName,
                        x.UserLName,
                        x.UserFName,
                        x.ShortCode,
                        x.AccountNumber,
                        x.AccountName,
                        x.OrgName,
                        x.ChargeTypeID,
                        x.IsSubsidyOrg
                    })
                    .ToList()
                    .Select(x => new ManagerUsageDetailItem()
                    {
                        Period = x.Key.Period,
                        BillingCategory = x.Key.BillingCategory,
                        ResourceID = x.Key.ResourceID,
                        ResourceName = x.Key.ResourceName,
                        UserName = x.Key.ManagerUserName,
                        DisplayName = Clients.GetDisplayName(x.Key.UserLName, x.Key.UserFName),
                        Account = GetAccountName(x.Key.ShortCode, x.Key.AccountNumber, x.Key.AccountName, x.Key.OrgName),
                        ChargeTypeID = x.Key.ChargeTypeID,
                        Sort = x.Key.BillingCategory + ":" + GetAccountSort(x.Key.ShortCode, x.Key.AccountNumber, x.Key.AccountName, x.Key.OrgName),
                        TotalCharge = x.Sum(g => g.TotalCharge),
                        SubsidyDiscount = x.Sum(g => g.SubsidyDiscount),
                        SubsidyOrg = x.Key.IsSubsidyOrg
                    }).ToList();
            }

            return result;
        }
    }

    public class ManagerUsageDetailItem
    {
        public DateTime Period { get; set; }
        public BillingCategory BillingCategory { get; set; }
        public int ResourceID { get; set; }
        public string ResourceName { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string Account { get; set; }
        public int ChargeTypeID { get; set; }
        public string Sort { get; set; }
        public double TotalCharge { get; set; }
        public double SubsidyDiscount { get; set; }
        public bool SubsidyOrg { get; set; }
    }

    public class ManagerUsageSummaryAccountItem
    {
        public int AccountID { get; set; }
        public string ShortCode { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string OrgName { get; set; }
        public double TotalCharge { get; set; }
        public double SubsidyDiscount { get; set; }
    }

    public class ManagerUsageSummaryClientItem
    {
        public int ClientID { get; set; }
        public string LName { get; set; }
        public string FName { get; set; }
        public ClientPrivilege Privs { get; set; }
        public double TotalCharge { get; set; }
        public double SubsidyDiscount { get; set; }
    }
}
