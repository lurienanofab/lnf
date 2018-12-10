using System;

namespace LNF.Models.Billing.Process
{
    /// <summary>
    /// The command to execute the finalize data tables process
    /// </summary>
    public class BillingProcessDataFinalizeCommand
    {
        public DateTime Period { get; set; }
    }
}
