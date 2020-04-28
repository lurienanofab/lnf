using LNF.DataAccess;

namespace LNF.Impl.Repository.Data
{
    /// <summary>
    /// A readonly view of an Org - all related tables are joined to retrieve all information with one query
    /// </summary>
    public class OrgInfo : OrgInfoBase, IDataItem
    {
        // This class is identical to the base class. The difference between OrgInfoBase subclasses is how they are selected from the database (different views).

        /// <summary>
        /// Gets a value that indicates if the item is active, as well as any related items
        /// </summary>
        /// <returns>True if the Org is active, otherwise false</returns>
        public override bool IsActive()
        {
            return OrgActive;
        }
    }
}
