namespace LNF.Repository.Data
{
    /// <summary>
    /// Determines what kind of charges can be applied to an Account
    /// </summary>
    public class AccountType : IDataItem
    {
        /// <summary>
        /// The unique id of an AccountType
        /// </summary>
        public virtual int AccountTypeID { get; set; }

        /// <summary>
        /// The name of an AccountType
        /// </summary>
        public virtual string AccountTypeName { get; set; }

        /// <summary>
        /// The description of an AccountType
        /// </summary>
        public virtual string Description { get; set; }
    }
}
