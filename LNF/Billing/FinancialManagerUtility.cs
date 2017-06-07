using LNF.CommonTools;
using LNF.Logging;
using LNF.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace LNF.Billing
{
    public static class FinancialManagerUtility
    {
        public static int SendMonthlyFinancialReport(MonthlyEmailOptions options = null)
        {
            DateTime lastMonth = DateTime.Now.Date.AddMonths(-1); //last month of today
            DateTime period = lastMonth.FirstOfMonth();
            
            return SendMonthlyUserUsageEmails(period, options);
        }

        public static int SendMonthlyUserUsageEmails(DateTime period, MonthlyEmailOptions options)
        {
            int count = 0;
            int queryCount = 0;

            using (var timer = LogTaskTimer.Start("FinancialManagerUtility.SendMonthlyUserUsageEmails", "queryCount = {0}, count = {1}", () => new object[] { queryCount, count }))
            {
                if (options == null)
                    options = new MonthlyEmailOptions();

                //With noEmail set to true, nothing really happens here. The appropriate users are selected and logged
                //but no email is actually sent. This is just for testing/debugging purposes.

                //Get managers list and associated clients info
                var query = DA.Current.QueryBuilder().ApplyParameters(new { p = period }).SqlQuery("EXEC sselData.dbo.Report_MonthlyFinacialManager @Period=:p").List();

                queryCount = query.Count;

                IEnumerable<int> managerOrgIds = query
                    .Where(x => !string.IsNullOrEmpty(x["Accounts"].ToString()))
                    .Select(x => (int)x["ManagerOrgID"])
                    .Distinct();

                StringBuilder emailBodyHTML;

                foreach (int managerOrgId in managerOrgIds)
                {
                    emailBodyHTML = new StringBuilder();

                    var rows = query.Where(x => (int)x["ManagerOrgID"] == managerOrgId).ToList();

                    string[] toAddr = new string[] { rows[0]["ManagerEmail"].ToString() };
                    string managerName = rows[0]["ManagerName"].ToString();

                    emailBodyHTML.AppendLine("<html>");
                    emailBodyHTML.AppendLine("<body>");
                    emailBodyHTML.AppendLine("Dear " + managerName + ",<br /><br />");
                    if (!string.IsNullOrEmpty(options.Message))
                        emailBodyHTML.AppendLine("<p>" + options.Message + "</p>");
                    emailBodyHTML.AppendLine("<p>Below are a list of " + Providers.Email.CompanyName + " lab users who have incurred charges during " + period.Month.ToString() + "/" + period.Year.ToString() + " and the active accounts for that user (shortcode / P/G). You are receiving this email because our records indicate that you are associated with these accounts.</p>");
                    emailBodyHTML.AppendLine("<p>Exact charges are still pending and may depend on data entries from the lab users themselves.</p>");
                    emailBodyHTML.AppendLine("<ol>");
                    emailBodyHTML.AppendLine("<li>If the person in charge of, or reconciling the account is not copied to this email, please send me his/her contact information.</li>");
                    emailBodyHTML.AppendLine("<li>Please review users and accounts and let me know if any change is needed.</li>");
                    emailBodyHTML.AppendLine("<li>If a user has access to multiple accounts, please let me know how charges should be distributed between these accounts.</li>");
                    emailBodyHTML.AppendLine("<li>As a reminder, there is more detailed information about the charging system in " + Providers.Email.CompanyName + " Online Services (<a href=\"http://ssel-sched.eecs.umich.edu/sselonline\">http://ssel-sched.eecs.umich.edu/sselonline</a>).</li>");
                    emailBodyHTML.AppendLine("</ol>");

                    StringBuilder table = new StringBuilder();
                    table.AppendLine("<table border=\"1\" bgcolor=\"lightblue\">");
                    string tr;
                    foreach (var row in rows)
                    {
                        tr = "<tr><td>" + row["ClientName"].ToString() + "</td><td>" + row["Accounts"].ToString() + "</td></tr>";
                        table.AppendLine(tr);
                    }
                    table.Append("</table>");

                    emailBodyHTML.AppendLine(table.ToString());
                    emailBodyHTML.AppendLine("</body>");
                    emailBodyHTML.AppendLine("</html>");
                    string subject = Providers.Email.CompanyName + " Charges - " + period.Month.ToString() + "/" + period.Year.ToString() + " [Manager: " + managerName + "]";

                    string statusMessage = string.Empty;

                    List<string> recip = new List<string>();

                    if (options.IncludeManager)
                        recip.AddRange(toAddr);

                    if (options.Recipients != null)
                        recip.AddRange(options.Recipients);

                    if (recip.Count > 0)
                    {
                        var result = Providers.Email.SendMessage(0, "LNF.Billing.FinancialManagerUtility.SendMonthlyUserUsageEmails(DateTime period, MonthlyEmailOptions options)", subject, emailBodyHTML.ToString(), "lnf-billing@umich.edu", recip, isHtml: true);
                        if (result.Success)
                            statusMessage = string.Format("Email to {0} sent OK", toAddr);
                        else
                            statusMessage = result.Exception.Message;
                    }
                    else
                        statusMessage = string.Format("Email to {0} not sent, NoEmail = True", toAddr);

                    timer.AddData(statusMessage);
                }

                count = managerOrgIds.Count();

                return count;
            }
        }
    }

    public class MonthlyEmailOptions
    {
        public string Message { get; set; }
        public string[] Recipients { get; set; }
        public bool IncludeManager { get; set; }

        public MonthlyEmailOptions()
        {
            IncludeManager = true;
        }
    }
}
