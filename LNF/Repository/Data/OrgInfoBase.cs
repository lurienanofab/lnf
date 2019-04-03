namespace LNF.Repository.Data
{
    /// <summary>
    /// A base class for subclasses that share common Org data
    /// </summary>
    public abstract class OrgInfoBase : IDataItem
    {
        /// <summary>
        /// The unique id of an Org
        /// </summary>
        public virtual int OrgID { get; set; }

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
        public virtual bool OrgActive { get; set; }

        /// <summary>
        /// The unique id of an OrgType
        /// </summary>
        public virtual int OrgTypeID { get; set; }

        /// <summary>
        /// The name of an OrgType
        /// </summary>
        public virtual string OrgTypeName { get; set; }

        /// <summary>
        /// The unique id of a ChargeType - used to determine charge tier
        /// </summary>
        public virtual int ChargeTypeID { get; set; }

        /// <summary>
        /// The name of a ChargeType
        /// </summary>
        public virtual string ChargeTypeName { get; set; }

        /// <summary>
        /// The unique id of the Account assigned to a ChargeType
        /// </summary>
        public virtual int ChargeTypeAccountID { get; set; }

        /// <summary>
        /// Gets a value that indicates if the item is active, as well as any related items
        /// </summary>
        /// <returns>True if the item is active, otherwise false</returns>
        public abstract bool IsActive();
    }
}