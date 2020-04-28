namespace LNF.Data
{
    public interface IAccountType
    {
        /// <summary>
        /// The unique id of an AccountType
        /// </summary>
        int AccountTypeID { get; set; }

        /// <summary>
        /// The name of an AccountType
        /// </summary>
        string AccountTypeName { get; set; }

        /// <summary>
        /// The description of an AccountType
        /// </summary>
        string Description { get; set; }
    }
}