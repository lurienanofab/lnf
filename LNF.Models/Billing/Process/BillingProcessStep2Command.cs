using System;

namespace LNF.Models.Billing.Process
{
    /// <summary>
    /// The result from executing BillingProcessStep2
    /// </summary>
    public class BillingProcessStep2Command : IProcessCommand
    {
        /// <summary>
        /// The command to execute. Allowed values are [room|tool|store]
        /// </summary>
        public BillingCategory BillingCategory { get; set; }

        /// <summary>
        /// The id of the client for which to run the process
        /// </summary>
        public int ClientID { get; set; }

        /// <summary>
        /// The month for which to run the process
        /// </summary>
        public DateTime Period { get; set; }
    }
}
