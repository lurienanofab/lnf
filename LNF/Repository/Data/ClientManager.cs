using System;

namespace LNF.Repository.Data
{
    /// <summary>
    /// Represents an association between two Clients - a user and a manager
    /// </summary>
    public class ClientManager : ActiveDataItem
    {
        /// <summary>
        /// The unique id of a ClientManager
        /// </summary>
        public virtual int ClientManagerID { get; set; }

        /// <summary>
        /// The related ClientOrg of the user
        /// </summary>
        public virtual ClientOrg ClientOrg { get; set; }

        /// <summary>
        /// The related ClientOrg of the manager
        /// </summary>
        public virtual ClientOrg ManagerOrg { get; set; }

        /// <summary>
        /// Indicates if a ClientManager association is currenlty active
        /// </summary>
        public override bool Active { get; set; }

        /// <summary>
        /// Gets the table name used in ActiveLog
        /// </summary>
        /// <returns>A table name string value</returns>
        public override string TableName() { return "ClientManager"; }

        /// <summary>
        /// Gets the record id used in ActiveLog - same as ClientManagerID
        /// </summary>
        /// <returns>A ClientManagerID integer value</returns>
        public override int Record() { return ClientManagerID; }
    }
}
