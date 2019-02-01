using LNF.Models;
using LNF.Repository;
using System;
using System.Data;

namespace LNF.CommonTools
{
    public abstract class ProcessBase<T> : IProcess<T> where T : DataProcessResult
    {
        protected T _result;

        protected abstract T CreateResult();

        public virtual T Start()
        {
            _result = CreateResult();

            _result.RowsDeleted = DeleteExisting();
           
            var dtExtract = Extract();

            _result.RowsExtracted = dtExtract.Rows.Count;

            if (_result.RowsExtracted > 0)
            {
                var dtTransform = Transform(dtExtract);

                _result.RowsLoaded = Load(dtTransform);
            }

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

        public virtual IBulkCopy CreateBulkCopy()
        {
            throw new NotImplementedException();
        }
    }
}
