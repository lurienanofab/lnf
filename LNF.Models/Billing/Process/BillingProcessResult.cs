using System;

namespace LNF.Models.Billing.Process
{
    /// <summary>
    /// The result of the billing process. Indicates whether not the process was successful and how much time it took to complete
    /// </summary>
    public sealed class BillingProcessResult
    {
        /// <summary>
        /// Indicates which command to execute. Allowed values: [room|tool|store]
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// The ClientID for which the process was run. Can be 0 for all clients
        /// </summary>
        public int ClientID { get; set; }

        /// <summary>
        /// Indicates whether the process was succesfully completed or not
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// A short description of the process performed
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// An error message set only if an error occurred (Success = false)
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// The date/time when the process began
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// The date/time when the process completed
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// The number of seconds it took to execute the process
        /// </summary>
        public double TimeTaken { get; set; }

        /// <summary>
        /// Any log messages generated while running the process
        /// </summary>
        public string LogText { get; set; }
    }
}
