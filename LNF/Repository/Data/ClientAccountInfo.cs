using System;

namespace LNF.Repository.Data
{
    /// <summary>
    /// A readonly view of a ClientAccount - all related tables are joined to retrieve all information with one query
    /// </summary>
    public class ClientAccountInfo : ClientAccountInfoBase, IDataItem, IActiveDataItem
    {
        // This class is identical to the base class. The difference between ClientAccountInfoBase subclasses is how they are selected from the database (different views).

        /// <summary>
        /// Gets a value that indicates if the item is active, as well as any related items
        /// </summary>
        /// <returns>True if the ClientOrg and ClientAccount are both active, otherwise false</returns>
        public override bool IsActive()
        {
            return ClientOrgActive && ClientAccountActive;
        }

        public virtual bool Active
        {
            get { return ClientAccountActive; }
            set {/*nothing to do here*/}
        }

        public virtual int Record()
        {
            return ClientAccountID;
        }

        public virtual string TableName()
        {
            return "ClientAccount";
        }
    }
}
