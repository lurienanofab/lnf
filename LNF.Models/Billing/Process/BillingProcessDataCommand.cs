using System;

namespace LNF.Models.Billing.Process
{

    /// <summary>
    /// The command used to initiate the DataClean and Data billing processes
    /// </summary>
    public class BillingProcessDataCommand : IProcessCommand
    {
        /// <summary>
        /// Indicates which command to execute. Allowed values: [room|tool|store]
        /// </summary>
        public BillingCategory BillingCategory { get; set; }

        /// <summary>
        /// The ClientID for which to run the process. Can be 0 for all clients
        /// </summary>
        public int ClientID { get; set; }

        /// <summary>
        /// The start of the date range
        /// </summary>
        public DateTime StartPeriod { get; set; }

        /// <summary>
        /// The end of the date range
        /// </summary>
        public DateTime EndPeriod { get; set; }

        /// <summary>
        /// The record id for which to run the process. This will either be a RoomID, ResourceID, or ItemID depending on Command. Can be 0 for all records.
        /// </summary>
        public int Record { get; set; }
    }
}
