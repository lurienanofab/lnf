using HandlebarsDotNet;
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
using System.Configuration;
using System.Data;
using System.IO;
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
            RoomJU report = new RoomJU { StartPeriod = sd, EndPeriod = ed, ClientID = id, JournalUnitType = Utility.StringToEnum<JournalUnitTypes>(type) };
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
            ToolJU report = new ToolJU { StartPeriod = sd, EndPeriod = ed, ClientID = id, JournalUnitType = Utility.StringToEnum<JournalUnitTypes>(type) };
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
                .Distinct().ToArray();

            var tpl = GetTemplate("financial-manager-email.handlebars");

            var companyName = GlobalSettings.Current.CompanyName;
            var period = options.Period;
            var message = string.IsNullOrEmpty(options.Message) ? null : options.Message;

            foreach (int moid in managerOrgIds)
            {
                var rows = dt.Select($"ManagerOrgID = {moid}");

                // there is at least one row becuase managerOrgIds is created from the same DataTable

                var managerName = rows[0].Field<string>("ManagerName");
                var clients = rows.Select(x => new
                {
                    clientName = x.Field<string>("ClientName"),
                    accounts = x.Field<string>("Accounts")
                }).ToArray();

                var body = tpl(new { companyName, period, message, managerName, clients });

                var toAddr = new string[] { rows[0].Field<string>("ManagerEmail") };
                var subj = $"{companyName} Charges - {options.Period:M/yyyy} [Manager: {managerName}]";

                result.Add(new FinancialManagerReportEmail
                {
                    ClientID = rows[0].Field<int>("ClientID"),
                    ManagerOrgID = rows[0].Field<int>("ManagerOrgID"),
                    DisplayName = managerName,
                    FromAddress = "lnf-billing@umich.edu",
                    ToAddress = toAddr,
                    CcAddress = ccAddr,
                    Subject = subj,
                    Body = body,
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

            var tpl = GetTemplate("user-apportionment-email.handlebars");

            var companyName = GlobalSettings.Current.CompanyName;
            var message = options.Message;

            var subj = $"Please apportion your {companyName} lab usage time";

            foreach (IApportionmentClient ac in query)
            {
                var displayName = ac.DisplayName;

                var body = tpl(new { companyName, message, displayName });

                var toAddr = ac.Emails.Split(',');

                result.Add(new UserApportionmentReportEmail
                {
                    ClientID = ac.ClientID,
                    DisplayName = ac.DisplayName,
                    FromAddress = "lnf-billing@umich.edu",
                    ToAddress = toAddr,
                    CcAddress = ccAddr,
                    Subject = subj,
                    Body = body,
                    IsHtml = true
                });
            }

            return result;
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
            var gs = Session.Query<Repository.Data.GlobalSettings>().FirstOrDefault(x => x.SettingName == "ApportionmentReminder_MonthlyEmailRecipients");

            if (gs == null || string.IsNullOrEmpty(gs.SettingValue))
                return null;

            return gs.SettingValue.Split(',');
        }

        private string[] GetFinancialManagerReportRecipients()
        {
            var gs = Session.Query<Repository.Data.GlobalSettings>().FirstOrDefault(x => x.SettingName == "FinancialManagerReport_MonthlyEmailRecipients");

            if (gs == null || string.IsNullOrEmpty(gs.SettingValue))
                return null;

            return gs.SettingValue.Split(',');
        }

        private HandlebarsTemplate<object, object> GetTemplate(string fileName)
        {
            var securePath = ConfigurationManager.AppSettings["SecurePath"];
            var templatePath = Path.Combine(securePath, "billing", "templates", fileName);
            var result = HandlebarsUtility.GetTemplate(templatePath);
            return result;
        }
    }
}
