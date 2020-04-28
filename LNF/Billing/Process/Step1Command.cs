namespace LNF.Billing.Process
{
    /// <summary>
    /// Indicates which command to execute. Allowed values: [room|tool|store]
    /// </summary>
    public class Step1Command : DataCommand
    {
        /// <summary>
        /// Determines if the data should be deleted first or not. This should usually be true
        /// </summary>
        public bool Delete { get; set; }

        /// <summary>
        /// Determines if the billing temp tables will be used (current month) or not (prior months)
        /// </summary>
        public bool IsTemp { get; set; }
    }
}
