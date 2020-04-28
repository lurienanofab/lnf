using System;
using System.Data;
using System.Data.Common;

namespace LNF.DataAccess
{
    public interface IUnitOfWorkAdapter : IDisposable
    {
        bool MapTableSchema { get; set; }
        DbCommand InsertCommand { get; set; }
        DbCommand UpdateCommand { get; set; }
        DbCommand DeleteCommand { get; set; }
        DbCommand SelectCommand { get; set; }

        void FillSchema(DataTable dt, SchemaType source);
        void FillSchema(DataSet ds, SchemaType source);
        void Fill(DataSet result);
        void Fill(DataSet result, string sourceTable);
        void Fill(DataTable result);
        int Update(DataSet ds, string srcTable);
        int Update(DataSet ds);
        int Update(DataRow[] dataRows);
        int Update(DataTable dt);
    }
}
