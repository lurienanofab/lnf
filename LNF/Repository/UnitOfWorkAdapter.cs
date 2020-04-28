using LNF.DataAccess;
using System.Data;
using System.Data.Common;

namespace LNF.Repository
{
    /// <summary>
    /// A custom DbDataAdapter for working directly with the database server
    /// </summary>
    public abstract class UnitOfWorkAdapter : DbDataAdapter, IUnitOfWorkAdapter
    {
        /// <summary>
        /// Indicates if the table schema will be mapped when a select statement is executed
        /// </summary>
        public bool MapTableSchema { get; set; }

        void IUnitOfWorkAdapter.Fill(DataSet result)
        {
            Fill(result);
        }

        void IUnitOfWorkAdapter.Fill(DataSet result, string sourceTable)
        {
            Fill(result, sourceTable);
        }

        void IUnitOfWorkAdapter.Fill(DataTable result)
        {
            Fill(result);
        }

        void IUnitOfWorkAdapter.FillSchema(DataTable dt, SchemaType source)
        {
            FillSchema(dt, source);
        }

        void IUnitOfWorkAdapter.FillSchema(DataSet ds, SchemaType source)
        {
            FillSchema(ds, source);
        }
    }
}
