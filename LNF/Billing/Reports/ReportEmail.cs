using System.Collections.Generic;

namespace LNF.Billing.Reports
{
    public abstract class ReportEmail
    {
        public int ClientID { get; set; }
        public string DisplayName { get; set; }
        public string FromAddress { get; set; }
        public string[] ToAddress { get; set; }
        public string[] CcAddress { get; set; }
        public string[] BccAddress { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsHtml { get; set; }

        public IEnumerable<string> AllEmails()
        {
            var result = new List<string>();

            if (ToAddress != null)
                result.AddRange(ToAddress);

            if (CcAddress != null)
                result.AddRange(CcAddress);

            if (BccAddress != null)
                result.AddRange(BccAddress);

            return result;
        }
    }

    public class UserApportionmentReportEmail : ReportEmail { }

    public class FinancialManagerReportEmail : ReportEmail
    {
        public int ManagerOrgID { get; set; }
    }
}
