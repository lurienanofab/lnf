using System;

namespace LNF.Models.Billing.Process
{
    /// <summary>
    /// The command used to initiate the Step4 (Subsidy) billing process
    /// </summary>
    public class BillingProcessStep4Command
    {
        /// <summary>
        /// The command to execute. Allowed values are <code>subsidy|finalize</code>. The finalize step will be run when Command = subsidy, but it can be run separately if needed.
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// The month for which to execute this process
        /// </summary>
        public DateTime Period { get; set; }

        /// <summary>
        /// The client id for which to execute this process - all clients will be included when this is zero
        /// </summary>
        public int ClientID { get; set; }
    }
}
