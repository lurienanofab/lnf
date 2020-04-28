using System;

namespace LNF.Billing.Reports
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

        /// <summary>
        /// The unique ClientID for the lab user, zero for all lab users.
        /// </summary>
        public int ClientID { get; set; }

        /// <summary>
        /// The unique ClientOrgID for the manager, zero for all managers.
        /// </summary>
        public int ManagerOrgID { get; set; }

        public FinancialManagerReportOptions()
        {
            IncludeManager = true;
        }
    }
}
