using LNF.CommonTools;
using System;
using System.Data;
using System.Data.SqlClient;

namespace LNF.Impl.Billing
{
    public abstract class ProcessConfig
    {
        public SqlConnection Connection { get; set; }
        public string Context { get; set; }
        public int ClientID { get; set; }
    }

    public abstract class RangeProcessConfig : ProcessConfig
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public abstract class ProcessBase<T> : IProcess<T> where T : DataProcessResult
    {
        protected T _result;

        private readonly ProcessConfig _config;

        protected SqlConnection Connection => _config.Connection;
        protected string Context => _config.Context;
        public int ClientID => _config.ClientID;

        public ProcessBase(ProcessConfig cfg)
        {
            _config = cfg;
        }

        protected abstract T CreateResult();

        public T Start()
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

            _result.SetEndedAt();

            return _result;
        }

        public abstract string ProcessName { get; }

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

        protected void AddParameter(SqlCommand cmd, string name, SqlDbType dbType, int size, string sourceColumn)
        {
            cmd.Parameters.Add(name, dbType, size, sourceColumn);
        }

        protected void AddParameter(SqlCommand cmd, string name, object value)
        {
            cmd.Parameters.AddWithValue(name, value);
        }

        protected void AddParameter(SqlCommand cmd, string name, object value, SqlDbType dbType)
        {
            cmd.Parameters.AddWithValue(name, value, dbType);
        }

        protected void AddParameter(SqlCommand cmd, string name, object value, SqlDbType dbType, int size)
        {
            cmd.Parameters.AddWithValue(name, value, dbType, size);
        }

        protected void AddParameterIf(SqlCommand cmd, string name, bool test, object value)
        {
            if (test)
                cmd.Parameters.AddWithValue(name, value);
        }

        protected void AddParameterIf(SqlCommand cmd, string name, bool test, object value, SqlDbType dbType)
        {
            if (test)
                cmd.Parameters.AddWithValue(name, value, dbType);
        }

        protected void AddParameterIf(SqlCommand cmd, string name, bool test, object value, SqlDbType dbType, int size)
        {
            if (test)
                cmd.Parameters.AddWithValue(name, value, dbType, size);
        }
    }

    public abstract class PeriodProcessConfig : ProcessConfig
    {
        public DateTime Period { get; set; }
    }

    public abstract class PeriodProcessBase<T> : ProcessBase<T> where T : DataProcessResult
    {
        public PeriodProcessBase(PeriodProcessConfig cfg) : base(cfg)
        {
            Period = cfg.Period;
            ValidPeriodCheck();
        }

        public DateTime Period { get; private set; }

        protected void ValidPeriodCheck()
        {
            // sanity check
            if (Period.Day != 1 || Period.Hour != 0 || Period.Minute != 0 || Period.Second != 0)
            {
                SendEmail.SendDeveloperEmail("LNF.Impl.Billing.PeriodProcessBase<T>.ValidPeriodCheck", $"Invalid period detected in {ProcessName} Import [run at {DateTime.Now:yyyy-MM-dd HH:mm:ss}]", $"Invalid period used - not midnight or 1st of month. Period = '{Period:yyyy-MM-dd HH:mm:ss}', ClientID = {ClientID}");
                Period = new DateTime(Period.Year, Period.Month, 1, 0, 0, 0);
                //throw new Exception($"Period is not midnight on the 1st of the month. Import: {ProcessName}, Period: {Period:yyyy-MM-dd HH:mm:ss}, ClientID: {ClientID}");
            }
        }
    }
}
