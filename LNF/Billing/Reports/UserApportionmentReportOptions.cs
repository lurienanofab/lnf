using System;

namespace LNF.Billing.Reports
{
    /// <summary>
    /// Options used for creating and sending the monthly User Apportionment reminder
    /// </summary>
    public class UserApportionmentReportOptions
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
        /// Indicates whether or not to actually send the emails. Normally this should be false but it is possible to generate the emails with sending them for testing purposes. The return value will indicate how many emails would be sent
        /// </summary>
        public bool NoEmail { get; set; }
    }
}
