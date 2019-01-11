using LNF.CommonTools;
using LNF.Models.Billing;
using LNF.Models.Billing.Reports;
using LNF.Repository;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;

namespace LNF.Billing
{
    public static class FinancialManagerUtility
    {
        public static SendMonthlyUserUsageEmailsProcessResult SendMonthlyFinancialReport(int clientId = 0, int managerOrgId = 0, MonthlyEmailOptions options = null)
        {
            DateTime lastMonth = DateTime.Now.Date.AddMonths(-1); //last month of today
            DateTime period = lastMonth.FirstOfMonth();

            return SendMonthlyUserUsageEmails(period, clientId, managerOrgId, options);
        }

        private static string[] GetFinancialManagerReportRecipients()
        {
            var gs = DA.Current.Query<GlobalSettings>().FirstOrDefault(x => x.SettingName == "FinancialManagerReport_MonthlyEmailRecipients");

            if (gs == null || string.IsNullOrEmpty(gs.SettingValue))
                return null;

            return gs.SettingValue.Split(',');
        }

        public static IEnumerable<FinancialManagerReportEmail> GetMonthlyUserUsageEmails(DateTime period, int clientId, int managerOrgId, string message)
        {
            var result = new List<FinancialManagerReportEmail>();

            string[] ccAddr = GetFinancialManagerReportRecipients();

            //Get managers list and associated clients info
            var dt = DA.Command()
                .Param("Period", period)
                .Param("ClientID", clientId > 0, clientId)
                .Param("ManagerOrgID", managerOrgId > 0, managerOrgId)
                .FillDataTable("dbo.Report_MonthlyFinacialManager");

            var managerOrgIds = dt.AsEnumerable()
                .Where(x => !string.IsNullOrEmpty(x.Field<string>("Accounts")))
                .Select(x => x.Field<int>("ManagerOrgID"))
                .Distinct().ToList();

            StringBuilder bodyHtml;

            foreach (int moid in managerOrgIds)
            {
                bodyHtml = new StringBuilder();

                var rows = dt.AsEnumerable().Where(x => x.Field<int>("ManagerOrgID") == moid).ToList();

                string[] toAddr = new string[] { rows[0].Field<string>("ManagerEmail") };
                string managerName = rows[0].Field<string>("ManagerName");

                bodyHtml.AppendLine("<html>");
                bodyHtml.AppendLine("<body>");
                bodyHtml.AppendLine($"Dear {managerName},<br /><br />");

                if (!string.IsNullOrEmpty(message))
                    bodyHtml.AppendLine($"<p>{message}</p>");

                bodyHtml.AppendLine($"<p>Below are a list of {ServiceProvider.Current.Email.CompanyName} lab users who have incurred charges during {period:M/yyyy} and the active accounts for that user (shortcode / P/G). You are receiving this email because our records indicate that you are associated with these accounts.</p>");
                bodyHtml.AppendLine("<p>Exact charges are still pending and may depend on data entries from the lab users themselves.</p>");
                bodyHtml.AppendLine("<ol>");
                bodyHtml.AppendLine("<li>If the person in charge of, or reconciling the account is not copied to this email, please send me his/her contact information.</li>");
                bodyHtml.AppendLine("<li>Please review users and accounts and let me know if any change is needed.</li>");
                bodyHtml.AppendLine("<li>If a user has access to multiple accounts, please let me know how charges should be distributed between these accounts.</li>");
                bodyHtml.AppendLine($"<li>As a reminder, there is more detailed information about the charging system in {ServiceProvider.Current.Email.CompanyName} Online Services (<a href=\"http://ssel-sched.eecs.umich.edu/sselonline\">http://ssel-sched.eecs.umich.edu/sselonline</a>).</li>");
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

                string subj = $"{ServiceProvider.Current.Email.CompanyName} Charges - {period:M/yyyy} [Manager: {managerName}]";

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

        public static SendMonthlyUserUsageEmailsProcessResult SendMonthlyUserUsageEmails(DateTime period, int clientId, int managerOrgId, MonthlyEmailOptions options)
        {
            var result = new SendMonthlyUserUsageEmailsProcessResult();

            if (options == null)
                options = new MonthlyEmailOptions();

            //With noEmail set to true, nothing really happens here. The appropriate users are selected and logged
            //but no email is actually sent. This is just for testing/debugging purposes.

            //Get managers list and associated clients info
            var emails = GetMonthlyUserUsageEmails(period, clientId, managerOrgId, options.Message);

            result.QueryCount = emails.Count();

            int totalSent = 0;

            foreach (var e in emails)
            {
                string statusMessage = string.Empty;

                var to = new List<string>();
                var cc = new List<string>();
                var bcc = new List<string>();

                if (e.ToAddress != null && options.IncludeManager)
                    to.AddRange(e.ToAddress);

                if (e.CcAddress != null)
                    cc.AddRange(e.CcAddress);

                if (e.BccAddress != null)
                    bcc.AddRange(e.BccAddress);

                if (to.Count + cc.Count + bcc.Count > 0)
                {
                    try
                    {
                        ServiceProvider.Current.Email.SendMessage(0, "LNF.Billing.FinancialManagerUtility.SendMonthlyUserUsageEmails", e.Subject, e.Body, e.FromAddress, to, cc, bcc, isHtml: e.IsHtml);
                        statusMessage = $"Email to {e.ToAddress} sent OK";
                        ++totalSent;
                    }
                    catch (Exception ex)
                    {
                        statusMessage = ex.Message;
                    }
                }
                else
                    statusMessage = $"Email to {string.Join(",", e.ToAddress)} not sent, NoEmail = True";

                result.Data.Add(statusMessage);
            }

            result.TotalEmailsSent = totalSent;

            return result;
        }
    }

    public class MonthlyEmailOptions
    {
        /// <summary>
        /// A special message to prepend to the email body. 
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Indicates if the manager should receive the email, or only addresses specified by the Recipients AppSetting value (for testing purposes).
        /// </summary>
        public bool IncludeManager { get; set; }

        public MonthlyEmailOptions()
        {
            // default to true
            IncludeManager = true;
        }
    }
}
