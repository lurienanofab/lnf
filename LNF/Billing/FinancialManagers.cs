using LNF.Billing.Reports;
using LNF.CommonTools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Billing
{
    public class FinancialManagers
    {
        private readonly IReportRepository _report;

        public FinancialManagers(IReportRepository report)
        {
            _report = report;
        }

        public SendMonthlyUserUsageEmailsProcessResult SendMonthlyFinancialReport(FinancialManagerReportOptions options)
        {
            return SendMonthlyUserUsageEmails(options);
        }

        public IEnumerable<FinancialManagerReportEmail> GetMonthlyUserUsageEmails(FinancialManagerReportOptions options)
        {
            return _report.GetFinancialManagerReportEmails(options);
        }

        public SendMonthlyUserUsageEmailsProcessResult SendMonthlyUserUsageEmails(FinancialManagerReportOptions options)
        {
            DateTime startedAt = DateTime.Now;

            if (options == null)
                throw new ArgumentNullException("options");

            //With noEmail set to true, nothing really happens here. The appropriate users are selected and logged
            //but no email is actually sent. This is just for testing/debugging purposes.

            //Get managers list and associated clients info
            var emails = GetMonthlyUserUsageEmails(options);
            var queryCount = emails.Count();

            var data = new List<string>();

            int totalEmailsSent = 0;

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
                        ++totalEmailsSent;
                    }
                    catch (Exception ex)
                    {
                        statusMessage = ex.Message;
                    }
                }
                else
                    statusMessage = $"Email to {string.Join(",", e.ToAddress)} not sent, NoEmail = True";

                data.Add(statusMessage);
            }


            var result = new SendMonthlyUserUsageEmailsProcessResult(startedAt, data)
            {
                QueryCount = queryCount,
                TotalEmailsSent = totalEmailsSent
            };

            return result;
        }
    }
}
