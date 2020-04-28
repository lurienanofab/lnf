using System;

namespace LNF.Billing.Process
{
    /// <summary>
    /// The command to execute the finalize data tables process
    /// </summary>
    public class FinalizeCommand
    {
        public DateTime Period { get; set; }
    }
}
