using System;

namespace LNF.Models.Billing.Process
{
    public class BillingProcessDataCleanCommand : IProcessCommand
    {
        /// <summary>
        /// Indicates which command to execute. Allowed values: [room|tool|store].
        /// </summary>
        public BillingCategory BillingCategory { get; set; }

        /// <summary>
        /// The ClientID for which to run the process. Can be 0 for all clients.
        /// </summary>
        public int ClientID { get; set; }

        /// <summary>
        /// The start of the date range to process.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// The end of the date range to process.
        /// </summary>
        public DateTime EndDate { get; set; }
    }
}
