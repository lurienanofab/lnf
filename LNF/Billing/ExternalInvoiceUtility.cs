using LNF.CommonTools;
using LNF.Repository;
using LNF.Repository.Billing;
using System;
using System.Collections.Generic;
using System.Data;

namespace LNF.Billing
{
    public static class ExternalInvoiceUtility
    {
        public static DataTable GetExternalChargeTypes()
        {
            using (var dba = DA.Current.GetAdapter())
                return dba.ApplyParameters(new { Action = "External" }).FillDataTable("ChargeType_Select");
        }

        public static DataTable GetActiveExternalOrgs(DateTime sd, DateTime ed)
        {
            using (var dba = DA.Current.GetAdapter())
            {
                DataTable dt = dba.ApplyParameters(new { Action = "ActiveExternal", sDate = sd, eDate = ed }).FillDataTable("Org_Select");
                dt.PrimaryKey = new[] { dt.Columns["OrgAcctID"] }; //this is pointless because AccountID is already unique
                return dt;
            }
        }

        public static IDictionary<string, ExternalInvoiceUsage> GetAllUsage(DateTime sd, DateTime ed, int accountId, bool showRemote)
        {
            return new Dictionary<string, ExternalInvoiceUsage>
            {
                { "Tool", GetToolUsage(sd, ed, accountId, showRemote) },
                { "Room", GetRoomUsage(sd, ed, accountId, showRemote) },
                { "Store", GetStoreUsage(sd, ed, accountId) },
                { "Misc", GetMiscUsage(sd, ed, accountId) }
            };
        }

        public static ExternalInvoiceUsage GetToolUsage(DateTime sd, DateTime ed, int accountId, bool showRemote)
        {
            using (var dba = DA.Current.GetAdapter())
            {
                dba.AddParameter("@Action", "ForInvoice");
                dba.AddParameter("@StartPeriod", sd);
                dba.AddParameter("@EndPeriod", ed);
                dba.AddParameter("@IsInternal", false);

                if (accountId > 0)
                    dba.AddParameter("@AccountID", accountId);

                if (!showRemote)
                    dba.AddParameter("@BillingTypeID", BillingType.Remote);

                // It will return a dataset with two tables inside
                // table #1: the data table contains individual tool usage
                // table #2: the client table contains all users who has used the tools on this account
                var ds = dba.FillDataSet("ToolBilling_Select");

                ds.Tables[0].Columns.Add("LineCost", typeof(double));

                //Calculate the true cost based on billing types
                LineCostUtility.CalculateToolLineCost(ds.Tables[0]);

                var dt = ds.Tables[0];
                var dtClient = ds.Tables[1];

                var result = new ExternalInvoiceUsage();

                //Aggregate the report based on ClientID
                foreach (DataRow dr in dtClient.Rows)
                {
                    double totalFee = Convert.ToDouble(dt.Compute("SUM(LineCost)", string.Format("ClientID = {0} AND AccountID = {1}", dr["ClientID"], dr["AccountID"])));
                    DataRow[] rows = dt.Select(string.Format("ClientID = {0} AND AccountID = {1}", dr["ClientID"], dr["AccountID"]));
                    string desc = GetToolDescription(rows[0]);
                    result.Add(CreateInvoiceLineItem(rows[0], 1, totalFee, desc));
                }

                return result;
            }
        }

        public static ExternalInvoiceUsage GetRoomUsage(DateTime sd, DateTime ed, int accountId, bool showRemote)
        {
            using (var dba = DA.Current.GetAdapter())
            {
                dba.AddParameter("@Action", "ForInvoice");
                dba.AddParameter("@StartPeriod", sd);
                dba.AddParameter("@EndPeriod", ed);
                dba.AddParameter("@IsInternal", false);

                if (accountId > 0)
                    dba.AddParameter("@AccountID", accountId);

                if (!showRemote)
                    dba.AddParameter("@BillingTypeID", BillingType.Remote);

                DataSet ds = dba.FillDataSet("RoomApportionmentInDaysMonthly_Select");

                ds.Tables[0].Columns.Add("LineCost", typeof(double));

                //Calculate the true cost based on billing types
                LineCostUtility.CalculateRoomLineCost(ds.Tables[0]);

                DataTable dt = ds.Tables[0];
                DataTable dtClient = ds.Tables[1];

                var result = new ExternalInvoiceUsage();

                //Aggregate the report based on ClientID
                foreach (DataRow dr in dtClient.Rows)
                {
                    double totalFee = Convert.ToDouble(dt.Compute("SUM(LineCost)", string.Format("ClientID = {0} AND AccountID = {1}", dr["ClientID"], dr["AccountID"])));
                    DataRow[] rows = dt.Select(string.Format("ClientID = {0} AND AccountID = {1}", dr["ClientID"], dr["AccountID"]), string.Empty);
                    string desc = GetRoomDescription(rows[0]);
                    result.Add(CreateInvoiceLineItem(rows[0], 1, totalFee, desc));
                }

                return result;
            }
        }

        public static ExternalInvoiceUsage GetStoreUsage(DateTime sd, DateTime ed, int accountId)
        {
            using (var dba = DA.Current.GetAdapter())
            {
                dba.AddParameter("@Action", "ForInvoice");
                dba.AddParameter("@StartPeriod", sd);
                dba.AddParameter("@EndPeriod", ed);
                dba.AddParameter("@IsInternal", false);

                if (accountId > 0)
                    dba.AddParameter("@AccountID", accountId);

                var ds = dba.FillDataSet("StoreBilling_Select");

                DataTable dt = ds.Tables[0];
                DataTable dtClient = ds.Tables[1];

                var result = new ExternalInvoiceUsage();

                //Aggregate the report based on ClientID
                foreach (DataRow dr in dtClient.Rows)
                {
                    double totalFee = Convert.ToDouble(dt.Compute("SUM(LineCost)", string.Format("ClientID = {0} AND AccountID = {1}", dr["ClientID"], dr["AccountID"])));
                    DataRow[] rows = dt.Select(string.Format("ClientID = {0} AND AccountID = {1}", dr["ClientID"], dr["AccountID"]));
                    string desc = GetStoreDescription(rows[0]);
                    result.Add(CreateInvoiceLineItem(rows[0], 1, totalFee, desc));
                }

                return result;
            }
        }

        public static ExternalInvoiceUsage GetMiscUsage(DateTime sd, DateTime ed, int accountId)
        {
            using (var dba = DA.Current.GetAdapter())
            {
                dba.AddParameter("@Action", "ForInvoice");
                dba.AddParameter("@StartPeriod", sd);
                dba.AddParameter("@EndPeriod", ed);
                dba.AddParameter("@IsInternal", false);

                if (accountId > 0)
                    dba.AddParameter("@AccountID", accountId);

                var dt = dba.FillDataTable("MiscBillingCharge_Select");

                dt.Columns.Add("LineCost", typeof(double), "Quantity * Cost");

                var result = new ExternalInvoiceUsage();

                //Aggregate the report based on ClientID
                foreach (DataRow dr in dt.Rows)
                {
                    double totalFee = Convert.ToDouble(dr.Field<decimal>("Cost"));
                    string desc = ExternalInvoiceUtility.GetMiscDescription(dr);
                    result.Add(CreateInvoiceLineItem(dr, dr.Field<double>("Quantity"), totalFee, desc));
                }

                return result;
            }
        }

        public static ExternalInvoiceLineItem CreateInvoiceLineItem(DataRow dr, double qty, double cost, string desc)
        {
            return new ExternalInvoiceLineItem()
            {
                ClientID = dr.Field<int>("ClientID"),
                OrgAcctID = dr.Field<int>("OrgAcctID"),
                OrgID = dr.Field<int>("OrgID"),
                AccountID = dr.Field<int>("AccountID"),
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
