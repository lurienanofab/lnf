using LNF.Billing;
using System;
using System.Data;
using System.Data.SqlClient;

namespace LNF.Impl.Billing
{
    //- Raw means straight from the DB.
    //- Cleaned means that extraneous data have been removed.
    //- Unlike room and tool, for which the cost is stored in the local DB, for the store
    //  the costs are stored remotely. Thus, when store activity is gathered, the cost will
    //  will be gathered as well.
    public class StoreDataReader : ReaderBase
    {
        public StoreDataReader(SqlConnection conn) : base(conn) { }

        public DataTable ReadStoreDataRaw(DateTime sd, DateTime ed, int clientId = 0)
        {
            using (var cmd = Connection.CreateCommand("dbo.sselMAS_Select"))
            using (var adap = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("Action", "StoreDataRaw");
                cmd.Parameters.AddWithValue("sDate", sd);
                cmd.Parameters.AddWithValue("eDate", ed);
                AddParameterIf(cmd, "ClientID", clientId > 0, clientId);

                var dt = new DataTable();
                dt.Columns.Add("Quantity", typeof(double)); // make Quantity column double for dryboxes

                adap.Fill(dt);

                return dt;
            }
        }

        //Making AllItems true for all items is logical.
        //However, in the SP, we need to pass in a 0 for all items.
        public DataTable ReadStoreDataClean(DateTime sd, DateTime ed, int clientId = 0, int itemId = 0, StoreDataCleanOption option = StoreDataCleanOption.AllItems)
        {
            using (var cmd = Connection.CreateCommand("dbo.StoreDataClean_Select"))
            using (var adap = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("Action", "ByClient");
                cmd.Parameters.AddWithValue("sDate", sd);
                cmd.Parameters.AddWithValue("eDate", ed);
                cmd.Parameters.AddWithValue("AllItems", (int)option);
                AddParameterIf(cmd, "ClientID", clientId > 0, clientId);
                AddParameterIf(cmd, "ItemID", itemId > 0, itemId);

                var dt = new DataTable();
                adap.Fill(dt);

                return dt;
            }
        }

        public DataTable ReadStoreData(DateTime period, int clientId = 0, int itemId = 0)
        {
            using (var cmd = Connection.CreateCommand("dbo.StoreData_Select"))
            using (var adap = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("Action", "AggByPeriod");
                cmd.Parameters.AddWithValue("Period", period);
                AddParameterIf(cmd, "ClientID", clientId > 0, clientId);
                AddParameterIf(cmd, "ItemID", itemId > 0, itemId);

                var dt = new DataTable();
                adap.Fill(dt);

                return dt;
            }
        }
    }
}
