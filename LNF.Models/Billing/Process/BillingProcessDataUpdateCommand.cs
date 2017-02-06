namespace LNF.Models.Billing.Process
{
    /// <summary>
    /// The command to execute the data table update process. This is used for daily data import
    /// </summary>
    public class BillingProcessDataUpdateCommand
    {
        /// <summary>
        /// The billing category for which to execute the command. Allowed values are [tool|room|store]
        /// </summary>
        public BillingCategory BillingCategory { get; set; }

        /// <summary>
        /// Indicates whether or not this is a daily data import. The affects the date range used
        /// </summary>
        public bool IsDailyImport { get; set; }
    }
}
