using LNF.Models.Data;

namespace LNF.Repository.Data
{
    /// <summary>
    /// Represents an association between a Client and Account
    /// </summary>
    public class ClientAccount : ActiveDataItem
    {
        /// <summary>
        /// The unique id of a ClientAccount
        /// </summary>
        public virtual int ClientAccountID { get; set; }

        /// <summary>
        /// The related ClientOrg item
        /// </summary>
        public virtual ClientOrg ClientOrg { get; set; }

        /// <summary>
        /// The related Account item
        /// </summary>
        public virtual Account Account { get; set; }

        /// <summary>
        /// Indicates if the user is an account manager
        /// </summary>
        public virtual bool Manager { get; set; }

        /// <summary>
        /// Indicates if a ClientAccount should be considered the default for a user
        /// </summary>
        public virtual bool? IsDefault { get; set; }

        /// <summary>
        /// Indicates if a ClientAccount is active
        /// </summary>
        public override bool Active { get; set; }

        /// <summary>
        /// The table name used in ActiveLog
        /// </summary>
        /// <returns>A table name string</returns>
        public override string TableName() { return "ClientAccount"; }

        /// <summary>
        /// The unique id used in ActiveLog - same as ClientAccountID
        /// </summary>
        /// <returns>A unqiue ClientAccountID integer</returns>
        public override int Record() { return ClientAccountID; }
    }
}
