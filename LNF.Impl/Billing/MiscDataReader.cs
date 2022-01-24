using System;
using System.Data;
using System.Data.SqlClient;

namespace LNF.Impl.Billing
{
    public class MiscDataReader : ReaderBase
    {
        public MiscDataReader(SqlConnection conn) : base(conn) { }

        public DataTable ReadMiscData(DateTime period)
        {
            using (var cmd = Connection.CreateCommand("dbo.MiscBillingCharge_Select"))
            using (var adap = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("Action", "GetAllByPeriod");
                cmd.Parameters.AddWithValue("Period", period);

                var dt = new DataTable("MiscExp");
                adap.Fill(dt);

                return dt;
            }
        }
    }
}
