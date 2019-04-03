using System;

namespace LNF.Repository.Data
{
    /// <summary>
    /// A readonly view of a ClientAccount - all related tables are joined to retrieve all information with one query
    /// </summary>
    public class ClientAccountInfo : ClientAccountInfoBase, IDataItem
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
    }
}
