namespace LNF.Repository.Data
{
    /// <summary>
    /// Represents the different components of a ChartField - used to parse an account number
    /// </summary>
    public enum ChartFieldName
    {
        /// <summary>
        /// The ChartField Account
        /// </summary>
        Account = 1,

        /// <summary>
        /// The ChartField Fund
        /// </summary>
        Fund = 2,

        /// <summary>
        /// The ChartField Department
        /// </summary>
        Department = 3,

        /// <summary>
        /// The ChartField Program
        /// </summary>
        Program = 4,

        /// <summary>
        /// The ChartField Account
        /// </summary>
        Class = 5,

        /// <summary>
        /// The ChartField Project
        /// </summary>
        Project = 6,

        /// <summary>
        /// The ChartField ShortCode (note: this is not part of the account number)
        /// </summary>
        ShortCode = 7
    }
}
