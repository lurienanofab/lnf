using LNF.Models.Billing;
using LNF.Repository;
using System;
using System.Collections.Generic;
using System.Data;

namespace LNF.Impl.Billing
{
    public class MiscBillingManager : ManagerBase, IMiscBillingManager
    {
        public MiscBillingManager(IProvider provider) : base(provider) { }

        public int CreateMiscBillingCharge(MiscBillingChargeCreateArgs args)
        {
            return Command().Param(args).ExecuteScalar<int>("dbo.MiscBillingCharge_Insert").Value;
        }

        public int UpdateMiscBilling(MiscBillingChargeUpdateArgs args)
        {
            return Command().Param(args).ExecuteNonQuery("dbo.MiscBillingCharge_Update").Value;
        }

        public int DeleteMiscBillingCharge(int expId)
        {
            return Command().Param("ExpID", expId).ExecuteNonQuery("dbo.MiscBillingCharge_Delete").Value;
        }

        public IMiscBillingCharge GetMiscBillingCharge(int expId)
        {
            var dt = Command()
                .Param("Action", "ByExpID")
                .Param("ExpID", expId)
                .FillDataTable("dbo.MiscBillingCharge_Select");

            if (dt.Rows.Count > 0)
                return MiscBillingChargeFromDataRow(dt.Rows[0]);
            else
               return null;
        }

        public IEnumerable<IMiscBillingCharge> GetMiscBillingCharges(DateTime period, int? clientId = null, bool? active = null)
        {
            var dt = Command()
                .Param("Action", "Search")
                .Param("Period", period)
                .Param("ClientID", clientId)
                .Param("Active", active)
                .FillDataTable("dbo.MiscBillingCharge_Select");

            var result = new List<IMiscBillingCharge>();
            
            foreach(DataRow dr in dt.Rows)
            {
                result.Add(MiscBillingChargeFromDataRow(dr));
            }

            return result;
        }

        private IMiscBillingCharge MiscBillingChargeFromDataRow(DataRow dr)
        {
            return new MiscBillingChargeItem
            {
                ExpID = dr.Field<int>("ExpID"),
                ClientID = dr.Field<int>("ClientID"),
                LName = dr.Field<string>("LName"),
                FName = dr.Field<string>("FName"),
                AccountID = dr.Field<int>("AccountID"),
                AccountName = dr.Field<string>("AccountName"),
                ShortCode = dr.Field<string>("ShortCode"),
                SUBType = dr.Field<string>("SUBType"),
                Period = dr.Field<DateTime>("Period"),
                ActDate = dr.Field<DateTime>("ActDate"),
                Description = dr.Field<string>("Description"),
                Quantity = dr.Field<double>("Quantity"),
                UnitCost = dr.Field<decimal>("UnitCost"),
                SubsidyDiscount = dr.Field<decimal>("SubsidyDiscount"),
                Active = dr.Field<bool>("Active")
            };
        }
    }
}
