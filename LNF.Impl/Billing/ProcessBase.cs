using LNF.CommonTools;
using System;
using System.Data;
using System.Data.SqlClient;

namespace LNF.Impl.Billing
{
    public abstract class ProcessBase<T> : IProcess<T> where T : DataProcessResult
    {
        protected T _result;

        protected SqlConnection Connection { get; }

        public ProcessBase(SqlConnection conn)
        {
            Connection = conn;
        }

        protected abstract T CreateResult();

        public virtual T Start()
        {
            _result = CreateResult();
            _result.RowsDeleted = DeleteExisting();

            var dtExtract = Extract();
            _result.RowsExtracted = dtExtract.Rows.Count;

            var dtTransform = Transform(dtExtract);

            if (dtTransform.Rows.Count > 0)
                _result.RowsLoaded = Load(dtTransform);
            else
                _result.RowsLoaded = 0;

            return _result;
        }

        public abstract int DeleteExisting();

        public abstract DataTable Extract();

        public virtual DataTable Transform(DataTable dtExtract)
        {
            // passing through
            return dtExtract;
        }

        public virtual int Load(DataTable dtTransform)
        {
            using (var bcp = CreateBulkCopy())
            {
                bcp.WriteToServer(dtTransform);
                return dtTransform.Rows.Count;
            }
        }

        public virtual LNF.DataAccess.IBulkCopy CreateBulkCopy()
        {
            throw new NotImplementedException();
        }

        protected void AddParameterIf(SqlCommand cmd, string name, bool test, object value)
        {
            if (test)
                cmd.Parameters.AddWithValue(name, value);
        }
    }
}
