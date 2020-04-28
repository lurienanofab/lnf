using System;

namespace LNF.Impl.Repository.Data
{
    /// <summary>
    /// Represents an association between a Client and Org
    /// </summary>
    public class ClientOrg : ActiveDataItem
    {
        /// <summary>
        /// The unique id of a ClientOrg
        /// </summary>
        public virtual int ClientOrgID { get; set; }

        /// <summary>
        /// The related Client item
        /// </summary>
        public virtual Client Client { get; set; }

        /// <summary>
        /// The related Org item
        /// </summary>
        public virtual Org Org { get; set; }

        /// <summary>
        /// The department within the Org to which the Client belongs
        /// </summary>
        public virtual Department Department { get; set; }

        /// <summary>
        /// The role within the Org to which the Client belongs
        /// </summary>
        public virtual Role Role { get; set; }

        /// <summary>
        /// Id of the client address
        /// </summary>
        public virtual int ClientAddressID { get; set; }

        /// <summary>
        /// The phone number of a ClientOrg
        /// </summary>
        public virtual string Phone { get; set; }

        /// <summary>
        /// The email address of a ClientOrg
        /// </summary>
        public virtual string Email { get; set; }

        /// <summary>
        /// Indicates if a ClientOrg is a manager (i.e. a PI)
        /// </summary>
        public virtual bool IsManager { get; set; }

        /// <summary>
        /// Indicates if a ClientOrg is a financial manager
        /// </summary>
        public virtual bool IsFinManager { get; set; }

        /// <summary>
        /// The date when the new user subsidy began
        /// </summary>
        public virtual DateTime? SubsidyStartDate { get; set; }

        /// <summary>
        /// The date when the new faculty subsidy began
        /// </summary>
        public virtual DateTime? NewFacultyStartDate { get; set; }

        /// <summary>
        /// Indictes if a ClientOrg is currently active
        /// </summary>
        public override bool Active { get; set; }

        /// <summary>
        /// Gets the record id used in the ActiveLog
        /// </summary>
        /// <returns>A ClientOrgID integer value</returns>
        public override int Record() { return ClientOrgID; }

        /// <summary>
        /// The table name used in the ActiveLog
        /// </summary>
        /// <returns>A table name string value</returns>
        public override string TableName() { return "ClientOrg"; }

        //public virtual IQueryable<ClientAccount> ClientAccounts()
        //{
        //    return DA.Current.Query<ClientAccount>().Where(x => x.ClientOrg.ClientOrgID == ClientOrgID);
        //}

        //public virtual IQueryable<ClientManager> ClientManagersWhereIsManager()
        //{
        //    return DA.Current.Query<ClientManager>().Where(x => x.ManagerOrg.ClientOrgID == ClientOrgID);
        //}

        //public virtual IQueryable<ClientManager> ClientManagersWhereIsEmployee()
        //{
        //    return DA.Current.Query<ClientManager>().Where(x => x.ClientOrg.ClientOrgID == ClientOrgID);
        //}
    }
}
