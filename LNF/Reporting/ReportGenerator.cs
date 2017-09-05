using LNF.CommonTools;
using LNF.Data;
using LNF.Models.Data;
using LNF.Models.Reporting;
using LNF.Models.Reporting.Individual;
using LNF.Repository;
using LNF.Repository.Billing;
using LNF.Repository.Data;
using LNF.Repository.Reporting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace LNF.Reporting
{
    public static class ReportGenerator
    {
        private static readonly ClientPrivilege _includeInmanagerUsageSummaryPriv = ClientPrivilege.LabUser | ClientPrivilege.RemoteUser;

        public static AggregateByOrg CreateAggregateByOrg(int clientId, DateTime period)
        {
            var result = new AggregateByOrg();

            var miscUsage = DA.Current.Query<MiscBillingCharge>().Where(x => x.Client.ClientID == clientId && x.Period == period && x.Active);

            var roomUsage = DA.Current.Query<RoomBillingByOrg>().Where(x => x.ClientID == clientId && x.Period == period);
            var toolUsage = DA.Current.Query<ToolBillingByOrg>().Where(x => x.ClientID == clientId && x.Period == period);

            result.RoomByOrg = roomUsage.Select(x => new RoomByOrgItem()
            {
                OrgID = x.OrgID,
                OrgName = x.OrgName,
                RoomCost = x.TotalCharge,
                MiscCarge = miscUsage.Where(z => z.SUBType == "Room" && z.Account.Org.OrgID == x.OrgID).Sum(z => (decimal?)(z.Quantity * z.UnitCost)).GetValueOrDefault(0),
                SubsidyDiscount = x.SubsidyDiscount + miscUsage.Where(z => z.SUBType == "Room" && z.Account.Org.OrgID == x.OrgID).Sum(z => (decimal?)(z.SubsidyDiscount)).GetValueOrDefault(0),
            }).ToList();

            //result.RoomByOrg = new List<RoomByOrgItem>();
            result.ToolByOrg = new List<ToolByOrgItem>();
            result.StoreByOrg = new List<StoreByOrgItem>();
            result.SubsidyByOrg = new List<SubsidyByOrgItem>();

            return result;
        }

        public static ManagerUsageSummary CreateManagerUsageSummary(DateTime period, ClientItem client, bool includeRemote)
        {
            return CreateManagerUsageSummary(period, client.ClientID, client.UserName, client.LName, client.FName, includeRemote);
        }

        public static IEnumerable<ManagerUsageSummaryItem> GetManagerUsageSummaryItems(int clientId, DateTime period, bool includeRemote)
        {
            var charges = ManagerUsageCharge.SelectByManager(clientId, period, includeRemote).ToList();

            var logs = ClientManagerLog.SelectByManager(clientId, period, period.AddMonths(1)).ToList();

            var join = logs.LeftJoin(
                inner: charges,
                outerKeySelector: o => new { o.ManagerClientID, o.UserClientID, o.AccountID },
                innerKeySelector: i => new { i.ManagerClientID, UserClientID = i.ClientID, i.AccountID },
                resultSelector: (o, i) => ManagerUsageSummaryItem.Create(o, i));

            return join;
        }

        public static ManagerUsageSummary CreateManagerUsageSummary(DateTime period, int clientId, string userName, string lname, string fname, bool includeRemote)
        {
            var model = new ManagerUsageSummary();
            model.ClientID = clientId;
            model.UserName = userName;
            model.LName = lname;
            model.FName = fname;
            model.Period = period;

            var items = GetManagerUsageSummaryItems(model.ClientID, model.Period, includeRemote);

            model.Accounts = items
                .GroupBy(x => new
                {
                    x.AccountID,
                    x.ShortCode,
                    x.AccountNumber,
                    x.AccountName,
                    x.OrgName
                })
                .Select(x => new
                {
                    x.Key.AccountID,
                    x.Key.ShortCode,
                    x.Key.AccountNumber,
                    x.Key.AccountName,
                    x.Key.OrgName,
                    TotalCharge = x.Sum(g => g.TotalCharge),
                    SubsidyDiscount = x.Sum(g => g.SubsidyDiscount)
                })
                .ToList()
                .Select(x => CreateManagerUsageSummaryAccount(x, items))
                .OrderBy(x => x.Sort)
                .ToList();

            model.Clients = items
                .GroupBy(x => new
                {
                    x.ClientID,
                    x.LName,
                    x.FName,
                    x.Privs
                })
                .Select(x => new
                {
                    x.Key.ClientID,
                    x.Key.LName,
                    x.Key.FName,
                    x.Key.Privs,
                    TotalCharge = x.Sum(g => g.TotalCharge),
                    SubsidyDiscount = x.Sum(g => g.SubsidyDiscount)
                })
                .ToList()
                .Where(x => x.TotalCharge > 0 || x.SubsidyDiscount > 0 || x.Privs.HasPriv(_includeInmanagerUsageSummaryPriv))
                .Select(x => CreateManagerUsageSummaryClient(x, items))
                .OrderBy(x => x.Sort)
                .ToList();

            model.ShowSubsidyColumn = items.Any(x => x.IsSubsidyOrg);

            return model;
        }

        public static ManagerUsageSummaryClient CreateManagerUsageSummaryClient(dynamic args, IEnumerable<ManagerUsageSummaryItem> items)
        {
            int clientId = args.ClientID;
            string name = GetClientName(args.LName, args.FName);
            string sort = GetClientSort(args.LName, args.FName);

            var comparer = new AccountItemEqualityComparer();

            return new ManagerUsageSummaryClient()
            {
                Name = name,
                Sort = sort,
                UsageCharge = args.TotalCharge,
                Subsidy = args.SubsidyDiscount,
                Accounts = items.Where(x => x.ClientID == clientId && (x.TotalCharge > 0 || x.SubsidyDiscount > 0 || x.Privs.HasPriv(_includeInmanagerUsageSummaryPriv))).Select(x => new AccountItem()
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

        public static string GetClientName(string lname, string fname)
        {
            return ClientModel.GetDisplayName(lname, fname);
        }

        public static string GetClientSort(string lname, string fname)
        {
            return ClientModel.GetDisplayName(lname, fname);
        }

        public static ManagerUsageSummaryAccount CreateManagerUsageSummaryAccount(dynamic args, IEnumerable<ManagerUsageSummaryItem> items)
        {
            int accountId = args.AccountID;
            string name = GetAccountName(args.ShortCode, args.AccountNumber, args.AccountName, args.OrgName);
            string sort = GetAccountSort(args.ShortCode, args.AccountNumber, args.AccountName, args.OrgName);

            var comparer = new ClientItemEqualityComparer();

            return new ManagerUsageSummaryAccount()
            {
                Name = name,
                Sort = sort,
                UsageCharge = args.TotalCharge,
                Subsidy = args.SubsidyDiscount,
                Clients = items.Where(x => x.AccountID == accountId && (x.TotalCharge > 0 || x.SubsidyDiscount > 0 || x.Privs.HasPriv(_includeInmanagerUsageSummaryPriv))).Select(x => new ClientItem()
                {
                    ClientID = x.ClientID,
                    UserName = x.UserName,
                    FName = x.FName,
                    LName = x.LName,
                    Email = x.Email
                }).Distinct(comparer).ToArray()
            };
        }

        public static string GetAccountName(string shortCode, string accountNumber, string accountName, string orgName)
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

        public static string GetAccountSort(string shortCode, string accountNumber, string accountName, string orgName)
        {
            string sort;

            if (string.IsNullOrEmpty(shortCode.Trim()))
            {
                sort = string.Format("2:{0}:{1}", orgName, accountName);
            }
            else
            {
                sort = "1:" + shortCode.Trim();
            }

            return sort;
        }

        public static UserUsageSummary CreateUserUsageSummary(DateTime period, ClientItem client)
        {
            bool showDisclaimer = false;

            var gs = DA.Current.Query<GlobalSettings>().FirstOrDefault(x => x.SettingName == "ShowUserUsageSummaryDisclaimer");

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

        public static XElement GetManagerUsageDetail(DateTime sd, DateTime ed, Client mgr, bool remote = false)
        {
            var charges = DA.Current.Query<ManagerUsageCharge>().Where(x => x.Period >= sd && x.Period < ed && x.ManagerClientID == mgr.ClientID && (!x.IsRemote || remote));

            var result = charges
                .GroupBy(x => new { x.Period, x.BillingCategory, x.LName, x.FName, x.ShortCode, x.AccountNumber, x.AccountName, x.OrgName, x.IsSubsidyOrg })
                .ToList()
                .Select(x => new
                {
                    Period = x.Key.Period,
                    BillingCategory = x.Key.BillingCategory,
                    DisplayName = ClientModel.GetDisplayName(x.Key.LName, x.Key.FName),
                    Account = GetAccountName(x.Key.ShortCode, x.Key.AccountNumber, x.Key.AccountName, x.Key.OrgName),
                    Sort = x.Key.BillingCategory + ":" + GetAccountSort(x.Key.ShortCode, x.Key.AccountNumber, x.Key.AccountName, x.Key.OrgName),
                    TotalCharge = x.Sum(g => g.TotalCharge),
                    SubsidyDiscount = x.Sum(g => g.SubsidyDiscount),
                    SubsidyOrg = x.Key.IsSubsidyOrg
                })
                .OrderBy(x => x.DisplayName)
                .ThenBy(x => x.Sort)
                .ToList();

            var xdoc = new XElement("table",
                result.Select(x => new XElement("row",
                    new XElement("Period", x.Period),
                    new XElement("BillingCategory", x.BillingCategory),
                    new XElement("DisplayName", x.DisplayName),
                    new XElement("Account", x.Account),
                    new XElement("TotalCharge", x.TotalCharge.ToString("#,##0.00")),
                    new XElement("SubsidyDiscount", x.SubsidyDiscount.ToString("#,##0.00")),
                    new XElement("SubsidyOrg", x.SubsidyOrg)
                )));


            return xdoc;
        }
    }
}
