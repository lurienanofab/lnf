using LNF.Billing;
using LNF.Billing.Reports;
using LNF.Billing.Reports.ServiceUnitBilling;
using LNF.CommonTools;
using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Billing;
using LNF.Impl.Repository.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace LNF.Impl.Billing.Report
{
    public class ReportRepository : RepositoryBase, IReportRepository
    {
        protected IBillingTypeRepository BillingType { get; }
        protected IApportionmentRepository Apportionment { get; }

        public ReportRepository(ISessionManager mgr, IBillingTypeRepository billingType, IApportionmentRepository apportionment) : base(mgr)
        {
            BillingType = billingType;
            Apportionment = apportionment;
        }

        public IEnumerable<IBillingSummary> GetBillingSummary(DateTime sd, DateTime ed, bool includeRemote = false, int clientId = 0)
        {
            ChargeType[] chargeTypes = Session.Query<ChargeType>().ToArray();

            //get all usage during the date range
            var toolUsage = Session.Query<ToolBilling>().Where(x => x.Period >= sd && x.Period < ed).ToArray();
            var roomUsage = Session.Query<RoomBilling>().Where(x => x.Period >= sd && x.Period < ed).ToArray();
            var storeUsage = Session.Query<StoreBilling>().Where(x => x.Period >= sd && x.Period < ed).ToArray();
            var miscUsage = Session.Query<MiscBillingCharge>().Where(x => x.Period >= sd && x.Period < ed).ToArray();

            var accounts = Session.Query<AccountInfo>().ToList();

            var result = chargeTypes.Select(x =>
            {
                decimal total = 0;

                total += toolUsage.Where(u => u.ChargeTypeID == x.ChargeTypeID && (u.BillingTypeID != BillingTypes.Remote || includeRemote)).Sum(s => BillingType.GetLineCost(s.CreateModel<IToolBilling>()));
                total += roomUsage.Where(u => u.ChargeTypeID == x.ChargeTypeID && (u.BillingTypeID != BillingTypes.Remote || includeRemote)).Sum(s => BillingType.GetLineCost(s.CreateModel<IToolBilling>()));
                total += storeUsage.Where(u => u.ChargeTypeID == x.ChargeTypeID).Sum(s => s.GetLineCost());
                total += miscUsage.Where(u => accounts.First(a => a.AccountID == u.AccountID).ChargeTypeID == x.ChargeTypeID).Sum(s => s.GetLineCost());

                var item = new BillingSummaryItem()
                {
                    StartDate = sd,
                    EndDate = ed,
                    ClientID = clientId,
                    ChargeTypeID = x.ChargeTypeID,
                    ChargeTypeName = x.ChargeTypeName,
                    IncludeRemote = includeRemote
                };

                item.TotalCharge = total;

                return item;
            }).ToArray();

            return result;
        }

        public IEnumerable<IRegularException> GetRegularExceptions(DateTime period, int clientId = 0)
        {
            IQueryable<RegularException> query;

            if (clientId > 0)
                query = Session.Query<RegularException>().Where(x => x.Period == period && x.ClientID == clientId);
            else
                query = Session.Query<RegularException>().Where(x => x.Period == period);

            var result = query.ToList();

            return result;
        }

        public RoomJU GetRoomJU(DateTime sd, DateTime ed, string type, int id = 0)
        {
            RoomJU report = new RoomJU { StartPeriod = sd, EndPeriod = ed, ClientID = id, JournalUnitType = ReportUtility.StringToEnum<JournalUnitTypes>(type) };
            RoomJournalUnitGenerator.Create(Session, report).Generate();
            return report;
        }

        public RoomSUB GetRoomSUB(DateTime sd, DateTime ed, int id = 0)
        {
            RoomSUB report = new RoomSUB { StartPeriod = sd, EndPeriod = ed, ClientID = id };
            RoomServiceUnitBillingGenerator.Create(Session, report).Generate();
            return report;
        }

        public StoreSUB GetStoreSUB(DateTime sd, DateTime ed, int id = 0, string option = null)
        {
            var twoCreditAccounts = false;

            if (!string.IsNullOrEmpty(option))
                twoCreditAccounts = option == "two-credit-accounts";

            StoreSUB report = new StoreSUB { StartPeriod = sd, EndPeriod = ed, ClientID = id, TwoCreditAccounts = twoCreditAccounts };
            StoreServiceUnitBillingGenerator.Create(Session, report).Generate();
            return report;
        }

        public ToolJU GetToolJU(DateTime sd, DateTime ed, string type, int id = 0)
        {
            ToolJU report = new ToolJU { StartPeriod = sd, EndPeriod = ed, ClientID = id, JournalUnitType = ReportUtility.StringToEnum<JournalUnitTypes>(type) };
            ToolJournalUnitGenerator.Create(Session, report).Generate();
            return report;
        }

        public ToolSUB GetToolSUB(DateTime sd, DateTime ed, int id = 0)
        {
            ToolSUB report = new ToolSUB { StartPeriod = sd, EndPeriod = ed, ClientID = id };
            ToolServiceUnitBillingGenerator.Create(Session, report).Generate();
            return report;
        }

        public IEnumerable<FinancialManagerReportEmail> GetFinancialManagerReportEmails(FinancialManagerReportOptions options)
        {
            var result = new List<FinancialManagerReportEmail>();

            string[] ccAddr = GetFinancialManagerReportRecipients();

            //Get managers list and associated clients info
            var dt = Session.Command()
                .Param("Period", options.Period)
                .Param("ClientID", options.ClientID > 0, options.ClientID)
                .Param("ManagerOrgID", options.ManagerOrgID > 0, options.ManagerOrgID)
                .FillDataTable("dbo.Report_MonthlyFinacialManager");

            var managerOrgIds = dt.AsEnumerable()
                .Where(x => !string.IsNullOrEmpty(x.Field<string>("Accounts")))
                .Select(x => x.Field<int>("ManagerOrgID"))
                .Distinct().ToList();

            StringBuilder bodyHtml;

            string companyName = Utility.GetGlobalSetting("CompanyName");

            foreach (int moid in managerOrgIds)
            {
                bodyHtml = new StringBuilder();

                var rows = dt.AsEnumerable().Where(x => x.Field<int>("ManagerOrgID") == moid).ToList();

                string[] toAddr = new string[] { rows[0].Field<string>("ManagerEmail") };
                string managerName = rows[0].Field<string>("ManagerName");

                bodyHtml.AppendLine("<html>");
                bodyHtml.AppendLine("<body>");
                bodyHtml.AppendLine($"Dear {managerName},<br /><br />");

                if (!string.IsNullOrEmpty(options.Message))
                    bodyHtml.AppendLine($"<p>{options.Message}</p>");

                bodyHtml.AppendLine($"<p>Below are a list of {companyName} lab users who have incurred charges during {options.Period:M/yyyy} and the active accounts for that user (shortcode / P/G). You are receiving this email because our records indicate that you are associated with these accounts.</p>");
                bodyHtml.AppendLine("<p>Exact charges are still pending and may depend on data entries from the lab users themselves.</p>");
                bodyHtml.AppendLine("<ol>");
                bodyHtml.AppendLine("<li>If the person in charge of, or reconciling the account is not copied to this email, please send me his/her contact information.</li>");
                bodyHtml.AppendLine("<li>Please review users and accounts and let me know if any change is needed.</li>");
                bodyHtml.AppendLine("<li>If a user has access to multiple accounts, please let me know how charges should be distributed between these accounts.</li>");
                bodyHtml.AppendLine($"<li>As a reminder, there is more detailed information about the charging system in {companyName} Online Services (<a href=\"http://ssel-sched.eecs.umich.edu/sselonline\">http://ssel-sched.eecs.umich.edu/sselonline</a>).</li>");
                bodyHtml.AppendLine("</ol>");

                StringBuilder table = new StringBuilder();
                string tr;

                table.AppendLine("<table border=\"1\" bgcolor=\"lightblue\">");

                foreach (var row in rows)
                {
                    tr = $"<tr><td>{row["ClientName"]}</td><td>{row["Accounts"]}</td></tr>";
                    table.AppendLine(tr);
                }

                table.Append("</table>");

                bodyHtml.AppendLine(table.ToString());
                bodyHtml.AppendLine("</body>");
                bodyHtml.AppendLine("</html>");

                string subj = $"{companyName} Charges - {options.Period:M/yyyy} [Manager: {managerName}]";

                result.Add(new FinancialManagerReportEmail
                {
                    ClientID = rows[0].Field<int>("ClientID"),
                    ManagerOrgID = rows[0].Field<int>("ManagerOrgID"),
                    DisplayName = managerName,
                    FromAddress = "lnf-billing@umich.edu",
                    ToAddress = toAddr,
                    CcAddress = ccAddr,
                    Subject = subj,
                    Body = bodyHtml.ToString(),
                    IsHtml = true
                });
            }

            return result;
        }

        public SendMonthlyUserUsageEmailsProcessResult SendFinancialManagerReport(FinancialManagerReportOptions options)
        {
            return FinancialManagers.SendMonthlyUserUsageEmails(options);
        }

        public IEnumerable<UserApportionmentReportEmail> GetUserApportionmentReportEmails(UserApportionmentReportOptions options)
        {
            var result = new List<UserApportionmentReportEmail>();

            string[] ccAddr = GetApportionmentReminderRecipients();

            var query = Apportionment.SelectApportionmentClients(options.Period, options.Period.AddMonths(1));

            StringBuilder bodyHtml;

            var companyName = Utility.GetGlobalSetting("CompanyName");

            foreach (IApportionmentClient ac in query)
            {
                string subj = $"Please apportion your {companyName} lab usage time";

                bodyHtml = new StringBuilder();
                bodyHtml.AppendLine($"{ac.DisplayName}:<br /><br />");

                if (!string.IsNullOrEmpty(options.Message))
                    bodyHtml.AppendLine($"<p>{options.Message}</p>");

                bodyHtml.AppendLine($"As can best be determined, you need to apportion your {companyName} lab time. This is necessary because you had access to multiple accounts and have entered one or more {companyName} rooms this billing period.<br /><br />");
                bodyHtml.AppendLine("This matter must be resolved by the close of the third business day of this month.");
                bodyHtml.AppendLine("For more information about how to apportion your time, please check the “Apportionment Instructions” file in the LNF Online Services > Help > User Fees section.");
                string[] toAddr = ac.Emails.Split(',');

                result.Add(new UserApportionmentReportEmail
                {
                    ClientID = ac.ClientID,
                    DisplayName = ac.DisplayName,
                    FromAddress = "lnf-billing@umich.edu",
                    ToAddress = toAddr,
                    CcAddress = ccAddr,
                    Subject = subj,
                    Body = bodyHtml.ToString(),
                    IsHtml = true
                });
            }

            return result;

            //return Provider.Billing.Apportionment.GetMonthlyApportionmentEmails(options);
        }

        public SendMonthlyApportionmentEmailsProcessResult SendUserApportionmentReport(UserApportionmentReportOptions options)
        {
            var result = new SendMonthlyApportionmentEmailsProcessResult();

            //With noEmail set to true, nothing happens here. The appropriate users are selected and logged
            //but no email is actually sent. This is for testing/debugging purposes.
            var emails = GetUserApportionmentReportEmails(options);

            result.ApportionmentClientCount = emails.Count();

            foreach (var e in emails)
            {
                if (e.ToAddress.Length > 0)
                {
                    if (!options.NoEmail)
                        SendEmail.Send(0, "LNF.Billing.ApportionmentUtility.SendMonthlyApportionmentEmails", e.Subject, e.Body, e.FromAddress, e.ToAddress, e.CcAddress, e.BccAddress, e.IsHtml);

                    // Always increment result even if noEmail == true so we can at least return how many emails would be sent.
                    // Note this is not incremented unless an email was found for the user, even when there are recipients included.
                    result.TotalEmailsSent += 1;

                    result.Data.Add($"Needs apportionment: {string.Join(",", e.ToAddress)}");
                }
                else
                    result.Data.Add($"Needs apportionment: no email found for {e.DisplayName}");
            }

            return result;
        }

        private string[] GetApportionmentReminderRecipients()
        {
            var gs = Session.Query<GlobalSettings>().FirstOrDefault(x => x.SettingName == "ApportionmentReminder_MonthlyEmailRecipients");

            if (gs == null || string.IsNullOrEmpty(gs.SettingValue))
                return null;

            return gs.SettingValue.Split(',');
        }

        private string[] GetFinancialManagerReportRecipients()
        {
            var gs = Session.Query<GlobalSettings>().FirstOrDefault(x => x.SettingName == "FinancialManagerReport_MonthlyEmailRecipients");

            if (gs == null || string.IsNullOrEmpty(gs.SettingValue))
                return null;

            return gs.SettingValue.Split(',');
        }
    }
}
