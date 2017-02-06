namespace LNF.Repository.Data
{
    /// <summary>
    /// An organization to which Clients and Accounts can belong
    /// </summary>
    public class Org : ActiveDataItem
    {
        /// <summary>
        /// The unique id of an Org
        /// </summary>
        public virtual int OrgID { get; set; }

        /// <summary>
        /// The type of an Org - used to determine internal/external charges
        /// </summary>
        public virtual OrgType OrgType { get; set; }

        /// <summary>
        /// The name of an Org
        /// </summary>
        public virtual string OrgName { get; set; }

        /// <summary>
        /// Id of the default client address
        /// </summary>
        public virtual int DefClientAddressID { get; set; }

        /// <summary>
        /// Id of the default billing address
        /// </summary>
        public virtual int DefBillAddressID { get; set; }

        /// <summary>
        /// Id of the default shipping address
        /// </summary>
        public virtual int DefShipAddressID { get; set; }

        /// <summary>
        /// Indicates if an Org is part of NNIN
        /// </summary>
        public virtual bool NNINOrg { get; set; }

        /// <summary>
        /// Indicates if an Org is the primary organization
        /// </summary>
        public virtual bool PrimaryOrg { get; set; }

        /// <summary>
        /// Indictes if an Org is currently active
        /// </summary>
        public override bool Active { get; set; }

        /// <summary>
        /// Gets the record id used in the ActiveLog
        /// </summary>
        /// <returns>A OrgID integer value</returns>
        public override int Record() { return OrgID; }

        /// <summary>
        /// The table name used in the ActiveLog
        /// </summary>
        /// <returns>A table name string value</returns>
        public override string TableName() { return "Org"; }
        
        /// <summary>
        /// Gets a value that indicates if an Org is internal based on the OrgType
        /// </summary>
        /// <returns>True if the Org is internal, otherwise false</returns>
        public virtual bool IsInternal()
        {
            return OrgType.ChargeType.ChargeTypeName.Equals("UMich");
        }
    }
}
