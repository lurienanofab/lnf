using System;

namespace LNF.Billing.Process
{
    /// <summary>
    /// The command to execute the data table update process. This is used for daily data import
    /// </summary>
    public class UpdateCommand
    {
        /// <summary>
        /// The billing category for which to execute the command. Allowed values are [tool|room|store]
        /// </summary>
        public BillingCategory BillingTypes { get; set; }
        
        public UpdateDataType UpdateTypes { get; set; }

        public DateTime Period { get; set; }

        public int ClientID { get; set; }
    }
}
