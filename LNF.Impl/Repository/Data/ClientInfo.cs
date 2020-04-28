namespace LNF.Impl.Repository.Data
{
    /// <summary>
    /// A readonly view of a Client (system user) - all related tables are joined to retrieve all information with one query
    /// </summary>
    public class ClientInfo : ClientOrgInfoBase
    {
        // This class is identical to the base class. The difference between ClientOrgInfoBase subclasses is how they are selected from the database (different views).

        /// <summary>
        /// Gets a value that indicates if the item is active, as well as any related items
        /// </summary>
        /// <returns>True if the Org and Client are both active, otherwise false</returns>
        public override bool IsActive()
        {
            return OrgActive && ClientActive;
        }
    }
}
