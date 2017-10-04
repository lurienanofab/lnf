using System;

namespace LNF.Models.Billing.Reports
{
    /// <summary>
    /// Options used for creating and sending the monthly Financial Manager report
    /// </summary>
    public class FinancialManagerReportOptions
    {
        /// <summary>
        /// The month for which to run the report
        /// </summary>
        public DateTime Period { get; set; }

        /// <summary>
        /// A message to insert into the normal email body
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Indicates whether or not to include the manager as a recipient. Normally this should be true, but it is possible to set to false for testing purposes.
        /// </summary>
        public bool IncludeManager { get; set; }

        public FinancialManagerReportOptions()
        {
            IncludeManager = true;
        }
    }
}
