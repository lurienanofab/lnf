using LNF.CommonTools;
using LNF.Repository;
using LNF.Repository.Billing;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Billing
{
    public static class RoomBillingUtility
    {
        public static IRoomBilling CreateRoomBillingItem(bool isTemp)
        {
            IRoomBilling result;

            if (isTemp)
                result = new RoomBillingTemp();
            else
                result = new RoomBilling();

            return result;
        }

        public static IRoomBilling CreateRoomBillingFromDataRow(DataRow dr, bool isTemp)
        {
            // Using Convert.ToDecimal because values can be either decimal or double depending on if they are from RoomBilling or RoomBillingTemp.

            IRoomBilling item = CreateRoomBillingItem(isTemp);

            item.RoomBillingID = 0;
            item.Period = dr.Field<DateTime>("Period");
            item.ClientID = dr.Field<int>("ClientID");
            item.RoomID = dr.Field<int>("RoomID");
            item.AccountID = dr.Field<int>("AccountID");
            item.ChargeTypeID = dr.Field<int>("ChargeTypeID");
            item.BillingTypeID = dr.Field<int>("BillingTypeID");
            item.OrgID = dr.Field<int>("OrgID");
            item.ChargeDays = Convert.ToDecimal(dr["ChargeDays"]);
            item.PhysicalDays = Convert.ToDecimal(dr["PhysicalDays"]);
            item.AccountDays = Convert.ToDecimal(dr["AccountDays"]);
            item.Entries = Convert.ToDecimal(dr["Entries"]);
            item.Hours = Convert.ToDecimal(dr["Hours"]);
            item.IsDefault = GetValueOrDefault(dr, "IsDefault", false); //column may not be present
            item.RoomRate = dr.Field<decimal>("RoomRate");
            item.EntryRate = dr.Field<decimal>("EntryRate");
            item.MonthlyRoomCharge = dr.Field<decimal>("MonthlyRoomCharge");
            item.RoomCharge = dr.Field<decimal>("RoomCharge");
            item.EntryCharge = dr.Field<decimal>("EntryCharge");
            item.SubsidyDiscount = GetValueOrDefault(dr, "SubsidyDiscount", 0M); //column may not be present

            return item;
        }

        private static T GetValueOrDefault<T>(DataRow dr, string key, T defval)
        {
            if (dr.Table.Columns.Contains(key))
                return dr.Field<T>(key);
            else
                return defval;
        }

        public static IEnumerable<IRoomBilling> SelectRoomBilling(DateTime period)
        {
            IRoomBilling[] items;

            if (period >= DateTime.Now.FirstOfMonth())
                items = DA.Current.Query<RoomBillingTemp>().Where(x => x.Period == period).ToArray();
            else
                items = DA.Current.Query<RoomBilling>().Where(x => x.Period == period).ToArray();

            return items;
        }

        public static IEnumerable<IRoomBilling> SelectRoomBilling(DateTime period, int clientId)
        {
            IRoomBilling[] items;

            if (period >= DateTime.Now.FirstOfMonth())
                items = DA.Current.Query<RoomBillingTemp>().Where(x => x.Period == period && x.ClientID == clientId).ToArray();
            else
                items = DA.Current.Query<RoomBilling>().Where(x => x.Period == period && x.ClientID == clientId).ToArray();

            return items;
        }

        public static int UpdateBillingType(Client client, Account acct, BillingType billingType, DateTime period)
        {
            string queryName = "UpdateBillingTypeRoomBilling" + (RepositoryUtility.IsCurrentPeriod(period) ? "Temp" : string.Empty);
            return DA.Current.QueryBuilder().ApplyParameters(new
            {
                ClientID = client.ClientID,
                AccountID = acct.AccountID,
                BillingTypeID = billingType.BillingTypeID,
                Period = period
            }).NamedQuery(queryName).Update();
        }
    }
}
