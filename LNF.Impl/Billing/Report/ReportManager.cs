using LNF.Billing;
using LNF.CommonTools;
using LNF.Models.Billing;
using LNF.Models.Billing.Reports;
using LNF.Models.Billing.Reports.ServiceUnitBilling;
using LNF.Repository;
using LNF.Repository.Billing;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Impl.Billing.Report
{
    public class ReportManager : ManagerBase, IReportManager
    {
        public ReportManager(IProvider provider) : base(provider) { }

        public IEnumerable<IBillingSummary> GetBillingSummary(DateTime sd, DateTime ed, bool includeRemote = false, int clientId = 0)
        {
            ChargeType[] chargeTypes = DA.Current.Query<ChargeType>().ToArray();

            //get all usage during the date range
            var toolUsage = Session.Query<ToolBilling>().Where(x => x.Period >= sd && x.Period < ed).ToArray();
            var roomUsage = Session.Query<RoomBilling>().Where(x => x.Period >= sd && x.Period < ed).ToArray();
            var storeUsage = Session.Query<StoreBilling>().Where(x => x.Period >= sd && x.Period < ed).ToArray();
            var miscUsage = Session.Query<MiscBillingCharge>().Where(x => x.Period >= sd && x.Period < ed).ToArray();

            var result = chargeTypes.Select(x =>
            {
                decimal total = 0;

                total += toolUsage.Where(u => u.ChargeTypeID == x.ChargeTypeID && (u.BillingTypeID != Provider.Billing.BillingType.Remote.BillingTypeID || includeRemote)).Sum(s => Provider.Billing.BillingType.GetLineCost(s.CreateModel<Models.Billing.IToolBilling>()));
                total += roomUsage.Where(u => u.ChargeTypeID == x.ChargeTypeID && (u.BillingTypeID != Provider.Billing.BillingType.Remote.BillingTypeID || includeRemote)).Sum(s => Provider.Billing.BillingType.GetLineCost(s.CreateModel<Models.Billing.IToolBilling>()));
                total += storeUsage.Where(u => u.ChargeTypeID == x.ChargeTypeID).Sum(s => s.GetLineCost());
                total += miscUsage.Where(u => u.Account.Org.OrgType.ChargeType.ChargeTypeID == x.ChargeTypeID).Sum(s => s.GetLineCost());

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

            var result = query.CreateModels<IRegularException>();

            return result;
        }

        public RoomJU GetRoomJU(DateTime sd, DateTime ed, string type, int id = 0)
        {
            RoomJU report = new RoomJU() { StartPeriod = sd, EndPeriod = ed, ClientID = id, JournalUnitType = ReportUtility.StringToEnum<JournalUnitTypes>(type) };
            RoomJournalUnitGenerator.Create(report).Generate();
            return report;
        }

        public RoomSUB GetRoomSUB(DateTime sd, DateTime ed, int id = 0)
        {
            RoomSUB report = new RoomSUB() { StartPeriod = sd, EndPeriod = ed, ClientID = id };
            RoomServiceUnitBillingGenerator.Create(report).Generate();
            return report;
        }

        public StoreSUB GetStoreSUB(DateTime sd, DateTime ed, int id = 0, string option = null)
        {
            var twoCreditAccounts = false;

            if (!string.IsNullOrEmpty(option))
                twoCreditAccounts = option == "two-credit-accounts";

            StoreSUB report = new StoreSUB() { StartPeriod = sd, EndPeriod = ed, ClientID = id, TwoCreditAccounts = twoCreditAccounts };
            StoreServiceUnitBillingGenerator.Create(report).Generate();
            return report;
        }

        public ToolJU GetToolJU(DateTime sd, DateTime ed, string type, int id = 0)
        {
            ToolJU report = new ToolJU() { StartPeriod = sd, EndPeriod = ed, ClientID = id, JournalUnitType = ReportUtility.StringToEnum<JournalUnitTypes>(type) };
            ToolJournalUnitGenerator.Create(report).Generate();
            return report;
        }

        public ToolSUB GetToolSUB(DateTime sd, DateTime ed, int id = 0)
        {
            ToolSUB report = new ToolSUB() { StartPeriod = sd, EndPeriod = ed, ClientID = id };
            ToolServiceUnitBillingGenerator.Create(report).Generate();
            return report;
        }

        public IEnumerable<FinancialManagerReportEmail> GetFinancialManagerReportEmails(FinancialManagerReportOptions options)
        {
            return FinancialManagerUtility.GetMonthlyUserUsageEmails(options);
        }

        public SendMonthlyUserUsageEmailsProcessResult SendFinancialManagerReport(FinancialManagerReportOptions options)
        {
            return FinancialManagerUtility.SendMonthlyUserUsageEmails(options);
        }

        public IEnumerable<UserApportionmentReportEmail> GetUserApportionmentReportEmails(UserApportionmentReportOptions options)
        {
            var result = new List<UserApportionmentReportEmail>();

            string[] ccAddr = GetApportionmentReminderRecipients();

            var query = Provider.Billing.Apportionment.SelectApportionmentClients(options.Period, options.Period.AddMonths(1));

            StringBuilder bodyHtml;

            var companyName = Utility.GetGlobalSetting("CompanyName");

            foreach (IApportionmentClient ac in query)
            {
                string subj = $"Please apportion your {Utility.GetGlobalSetting("CompanyName")} lab usage time";

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
    }
}
