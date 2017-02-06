using System;

namespace LNF.Models.Billing.Process
{
    /// <summary>
    /// The command to execute the finalize data tables process
    /// </summary>
    public class BillingProcessDataFinalizeCommand
    {
        public DateTime StartPeriod { get; set; }
        public DateTime EndPeriod { get; set; }
    }
}
