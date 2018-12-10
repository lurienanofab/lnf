using System.Data.Common;

namespace LNF.Repository
{
    /// <summary>
    /// A custom DbDataAdapter for working directly with the database server
    /// </summary>
    public abstract class UnitOfWorkAdapter : DbDataAdapter
    {
        /// <summary>
        /// Indicates if the table schema will be mapped when a select statement is executed
        /// </summary>
        public bool MapTableSchema { get; set; }
    }
}
