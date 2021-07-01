using LNF.Billing;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Billing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace LNF.Impl.Billing
{
    public class MiscBillingRepository : SqlClientRepositoryBase, IMiscBillingRepository
    {
        public int CreateMiscBillingCharge(MiscBillingChargeCreateArgs args)
        {
            using (var cmd = NewCommand("sselData.dbo.MiscBillingCharge_Insert"))
            {
                cmd.Parameters.AddWithValue("AccountID", args.AccountID, SqlDbType.Int);
                cmd.Parameters.AddWithValue("ActDate", args.ActDate, SqlDbType.DateTime);
                cmd.Parameters.AddWithValue("ClientID", args.ClientID, SqlDbType.Int);
                cmd.Parameters.AddWithValue("Description", args.Description, SqlDbType.NVarChar, 100);
                cmd.Parameters.AddWithValue("Period", args.Period, SqlDbType.DateTime);
                cmd.Parameters.AddWithValue("Quantity", args.Quantity, SqlDbType.Float);
                cmd.Parameters.AddWithValue("SUBType", args.SUBType, SqlDbType.NVarChar, 10);
                cmd.Parameters.AddWithValue("UnitCost", args.UnitCost, SqlDbType.Decimal);

                cmd.Connection.Open();
                var obj = cmd.ExecuteScalar();
                cmd.Connection.Close();
                var result = Convert.ToInt32(obj ?? 0);

                return result;
            }
        }

        public int UpdateMiscBilling(MiscBillingChargeUpdateArgs args)
        {
            using (var cmd = NewCommand("sselData.dbo.MiscBillingCharge_Update"))
            {
                cmd.Parameters.AddWithValue("ExpID", args.ExpID, SqlDbType.Int);
                cmd.Parameters.AddWithValue("Description", args.Description, SqlDbType.NVarChar, 100);
                cmd.Parameters.AddWithValue("Period", args.Period, SqlDbType.DateTime);
                cmd.Parameters.AddWithValue("Quantity", args.Quantity, SqlDbType.Float);
                cmd.Parameters.AddWithValue("UnitCost", args.UnitCost, SqlDbType.Decimal);

                cmd.Connection.Open();
                var result = cmd.ExecuteNonQuery();
                cmd.Connection.Close();

                return result;
            }
        }

        public int DeleteMiscBillingCharge(int expId)
        {
            using (var cmd = NewCommand("sselData.dbo.MiscBillingCharge_Delete"))
            {
                cmd.Parameters.AddWithValue("ExpID", expId, SqlDbType.Int);

                cmd.Connection.Open();
                var result = cmd.ExecuteNonQuery();
                cmd.Connection.Close();

                return result;
            }
        }

        public IMiscBillingCharge GetMiscBillingCharge(int expId)
        {
            using (var cmd = NewCommand("sselData.dbo.MiscBillingCharge_Select"))
            using (var adap = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("Action", "ByExpID", SqlDbType.NVarChar, 50);
                cmd.Parameters.AddWithValue("ExpID", expId, SqlDbType.Int);

                var dt = new DataTable();
                adap.Fill(dt);

                if (dt.Rows.Count == 0)
                    return null;

                var dr = dt.Rows[0];

                var result = new MiscBillingCharge
                {
                    ExpID = dr.Field<int>("ExpID"),
                    ClientID = dr.Field<int>("ClientID"),
                    AccountID = dr.Field<int>("AccountID"),
                    SUBType = dr.Field<string>("SUBType"),
                    Period = dr.Field<DateTime>("Period"),
                    ActDate = dr.Field<DateTime>("ActDate"),
                    Description = dr.Field<string>("Description"),
                    Quantity = dr.Field<double>("Quantity"),
                    UnitCost = dr.Field<decimal>("UnitCost"),
                    SubsidyDiscount = dr.Field<decimal>("SubsidyDiscount"),
                    Active = dr.Field<bool>("Active")
                };

                return result;
            }
        }

        public IEnumerable<IMiscBillingChargeItem> GetMiscBillingCharges(DateTime period, string[] types, int clientId = 0, int accountId = 0, bool? active = null)
        {
            var ltypes = types.Select(x => x.ToLower()).ToArray();

            //EXEC sselData.dbo.MiscBillingCharge_Select @Action = 'Search', @Period = :Period, @ClientID = :ClientID, @AccountID = :AccountID, @Active = :Active
            using (var cmd = NewCommand("sselData.dbo.MiscBillingCharge_Select"))
            using (var adap = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("Action", "Search", SqlDbType.NVarChar, 50);
                cmd.Parameters.AddWithValue("Period", period, SqlDbType.DateTime);

                if (clientId > 0)
                    cmd.Parameters.AddWithValue("ClientID", clientId, SqlDbType.Int);

                if (accountId > 0)
                    cmd.Parameters.AddWithValue("AccountID", accountId, SqlDbType.Int);

                cmd.Parameters.AddWithValue("Active", active, SqlDbType.Bit);

                var dt = new DataTable();
                adap.Fill(dt);

                if (dt.Rows.Count == 0)
                    return new List<IMiscBillingChargeItem>();

                var result = new List<IMiscBillingChargeItem>();

                foreach (DataRow dr in dt.Rows)
                {
                    var subType = dr.Field<string>("SUBType");

                    if (ltypes.Contains(subType.ToLower()))
                    {
                        result.Add(new MiscBillingChargeItem
                        {
                            ExpID = dr.Field<int>("ExpID"),
                            ClientID = dr.Field<int>("ClientID"),
                            LName = dr.Field<string>("LName"),
                            FName = dr.Field<string>("FName"),
                            AccountID = dr.Field<int>("AccountID"),
                            AccountName = dr.Field<string>("AccountName"),
                            ShortCode = dr.Field<string>("ShortCode"),
                            SUBType = subType,
                            Period = dr.Field<DateTime>("Period"),
                            ActDate = dr.Field<DateTime>("ActDate"),
                            Description = dr.Field<string>("Description"),
                            Quantity = dr.Field<double>("Quantity"),
                            UnitCost = dr.Field<decimal>("UnitCost"),
                            SubsidyDiscount = dr.Field<decimal>("SubsidyDiscount"),
                            Active = dr.Field<bool>("Active")
                        });
                    }
                }

                return result;
            }
        }
    }
}
