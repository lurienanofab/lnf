using LNF.CommonTools;
using System;
using System.Data;

namespace LNF.Billing
{
    public static class ExternalInvoiceUtility
    {
        public static ExternalInvoiceLineItem CreateInvoiceLineItem(DataRow dr, double qty, double cost, string desc)
        {
            return new ExternalInvoiceLineItem()
            {
                AccountID = dr.Field<int>("AccountID"),
                OrgID = dr.Field<int>("OrgID"),
                ClientID = dr.Field<int>("ClientID"),
                ChargeTypeID = dr.Field<int>("ChargeTypeID"),
                LName = dr.Field<string>("LName"),
                FName = dr.Field<string>("FName"),
                OrgName = dr.Field<string>("OrgName"),
                AccountName = dr.Field<string>("Name"),
                PoEndDate = dr.Field<DateTime?>("PoEndDate"),
                PoRemainingFunds = dr.Field<decimal?>("PoRemainingFunds").GetValueOrDefault(),
                InvoiceNumber = dr.Field<string>("InvoiceNumber"),
                DeptRef = dr.Field<string>("DisplayDeptRefID"),
                Description = desc,
                Quantity = qty,
                Cost = cost
            };
        }

        public static string GetToolDescription(DataRow dr)
        {
            return string.Format("Tool - {0}", Utility.Left(dr.Field<string>("DisplayName"), 35));
        }

        public static string GetRoomDescription(DataRow dr)
        {
            return string.Format("Room - {0}", Utility.Left(dr.Field<string>("DisplayName"), 35));
        }

        public static string GetStoreDescription(DataRow dr)
        {
            return string.Format("Store - {0}", Utility.Left(dr.Field<string>("DisplayName"), 35));
        }

        public static string GetMiscDescription(DataRow dr)
        {
            return Utility.Left(dr.Field<string>("Description") + " " + dr.Field<string>("DisplayName"), 49);
        }
    }
}
