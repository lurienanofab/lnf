using LNF.Billing.Reports;
using LNF.CommonTools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Billing
{
    public static class FinancialManagers
    {
        public static SendMonthlyUserUsageEmailsProcessResult SendMonthlyFinancialReport(FinancialManagerReportOptions options)
        {
            DateTime lastMonth = DateTime.Now.Date.AddMonths(-1); //last month of today
            DateTime period = lastMonth.FirstOfMonth();

            return SendMonthlyUserUsageEmails(options);
        }

        public static IEnumerable<FinancialManagerReportEmail> GetMonthlyUserUsageEmails(FinancialManagerReportOptions options)
        {
            return ServiceProvider.Current.Billing.Report.GetFinancialManagerReportEmails(options);
        }

        public static SendMonthlyUserUsageEmailsProcessResult SendMonthlyUserUsageEmails(FinancialManagerReportOptions options)
        {
            if (options == null)
                throw new ArgumentNullException("options");

            var result = new SendMonthlyUserUsageEmailsProcessResult();

            //With noEmail set to true, nothing really happens here. The appropriate users are selected and logged
            //but no email is actually sent. This is just for testing/debugging purposes.

            //Get managers list and associated clients info
            var emails = GetMonthlyUserUsageEmails(options);

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
                        SendEmail.Send(0, "LNF.Billing.FinancialManagerUtility.SendMonthlyUserUsageEmails", e.Subject, e.Body, e.FromAddress, to, cc, bcc, e.IsHtml);
                        statusMessage = $"Email to {string.Join(",", e.ToAddress)} sent OK";
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
}
