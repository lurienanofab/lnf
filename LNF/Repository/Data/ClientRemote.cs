namespace LNF.Repository.Data
{
    /// <summary>
    /// Represents an association between two Clients - a staff user and a remote user
    /// </summary>
    public class ClientRemote : ActiveDataItem
    {
        /// <summary>
        /// The unqiue id of a ClientRemote
        /// </summary>
        public virtual int ClientRemoteID { get; set; }

        /// <summary>
        /// The related staff Client item
        /// </summary>
        public virtual Client Client { get; set; }

        /// <summary>
        /// The related Account item - belonging to the remote user
        /// </summary>
        public virtual Account Account { get; set; }

        /// <summary>
        /// The related remote Client item
        /// </summary>
        public virtual Client RemoteClient { get; set; }

        /// <summary>
        /// Indicates if a ClientRemote association is currently active (cannot be set because ClientRemote items are never disabled, they are deleted)
        /// </summary>
        public override bool Active { get { return true; } set { } } //there's an exception for every rule

        /// <summary>
        /// Gets the record id used in the ActiveLog
        /// </summary>
        /// <returns>A ClientRemoteID integer value</returns>
        public override string TableName() { return "ClientRemote"; }

        /// <summary>
        /// The table name used in the ActiveLog
        /// </summary>
        /// <returns>A table name string value</returns>
        /// <returns></returns>
        public override int Record() { return ClientRemoteID; }
    }
}
