using LNF.Cache;
using LNF.CommonTools;
using LNF.Data;
using LNF.Repository;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Billing
{
    public static class ExternalInvoiceUtility
    {
        public static string GetDeptRef(Org org, DateTime startDate, DateTime endDate)
        {
            Account acct = OrgRechargeUtility.GetRechargeAccount(org, startDate, endDate);
            AccountChartFields fields = new AccountChartFields(acct);
            return fields.Project;
        }

        public static DataSet GetInvoiceReportFromSession(DateTime startPeriod, DateTime endPeriod)
        {
            DataSet ds = CacheManager.Current.InvoiceReport();

            if (ds == null)
                ds = GetInvoiceReport(startPeriod, endPeriod);

            return ds;
        }

        public static DataSet GetInvoiceReport(DateTime startPeriod, DateTime endPeriod)
        {
            using (var dba = DA.Current.GetAdapter())
            {
                DataSet dsInvoiceReport = new DataSet("InvoiceReport");

                // globalCost info
                dba.FillDataSet(dsInvoiceReport, "GlobalCost_Select", "Global");

                if (dsInvoiceReport.Tables.Contains("Org")) dsInvoiceReport.Tables.Remove(dsInvoiceReport.Tables["Org"]);
                if (dsInvoiceReport.Tables.Contains("Account")) dsInvoiceReport.Tables.Remove(dsInvoiceReport.Tables["Account"]);
                if (dsInvoiceReport.Tables.Contains("RoomUsage")) dsInvoiceReport.Tables.Remove(dsInvoiceReport.Tables["RoomUsage"]);
                if (dsInvoiceReport.Tables.Contains("StoreUsage")) dsInvoiceReport.Tables.Remove(dsInvoiceReport.Tables["StoreUsage"]);
                if (dsInvoiceReport.Tables.Contains("ToolUsage")) dsInvoiceReport.Tables.Remove(dsInvoiceReport.Tables["ToolUsage"]);
                if (dsInvoiceReport.Tables.Contains("MiscExp")) dsInvoiceReport.Tables.Remove(dsInvoiceReport.Tables["MiscExp"]);

                // get all active external orgs along with their accounts
                dba.ApplyParameters(new { Action = "ActiveExternal", sDate = startPeriod, eDate = endPeriod }).FillDataSet(dsInvoiceReport, "Org_Select", "Org");
                dsInvoiceReport.Tables["Org"].PrimaryKey = new[] { dsInvoiceReport.Tables["Org"].Columns["OrgAcctID"] };

                // get the account info - used for grid display
                dba.SelectCommand.ClearParameters();
                dba.ApplyParameters(new { Action = "AllActiveExternal", sDate = startPeriod, eDate = endPeriod }).FillDataSet(dsInvoiceReport, "Account_Select", "Account");
                dsInvoiceReport.Tables["Account"].PrimaryKey = new[] { dsInvoiceReport.Tables["Account"].Columns["AccountID"] };

                CacheManager.Current.InvoiceReport(dsInvoiceReport);
                CacheManager.Current.StartPeriod(startPeriod);
                CacheManager.Current.EndPeriod(endPeriod);

                return dsInvoiceReport;
            }
        }

        public static DataView GetOrgs(DataSet dsInvoiceReport, DateTime startPeriod)
        {
            // read date - no need to store as calcCost pulls the info from the DB
            DataTable dtRoomUsage = ReadData.Room.ReadRoomData(startPeriod, 0, 0);
            DataTable dtStoreUsage = ReadData.Store(startPeriod, startPeriod.AddMonths(1), 0, 0).ReadStoreData();
            DataTable dtToolUsage = ReadData.Tool.ReadToolData(startPeriod, 0, 0);
            DataTable dtMiscUsage = ReadData.Misc.ReadMiscData(startPeriod);

            var ds = new DataSet();
            ds.Tables.Add(dtRoomUsage);
            ds.Tables.Add(dtStoreUsage);
            ds.Tables.Add(dtToolUsage);
            ds.Tables.Add(dtMiscUsage);

            // now check each org for activity
            // each row has a unique AccountID
            DataRow[] fdr;
            foreach (DataRow dr in dsInvoiceReport.Tables["Org"].Rows)
            {
                dr.SetField("HasActivity", 0);
                foreach (DataTable dt in ds.Tables)
                {
                    if (dr.Field<int>("HasActivity") == 0) // only check if still needed
                    {
                        fdr = dt.Select(string.Format("AccountID = {0}", dr["AccountID"]));
                        if (fdr.Length > 0)
                        {
                            dr.SetField("HasActivity", 1);
                            break;
                        }
                    }
                }
            }

            if (dsInvoiceReport.Tables[dtRoomUsage.TableName] != null) dsInvoiceReport.Tables.Remove(dtRoomUsage.TableName);
            if (dsInvoiceReport.Tables[dtStoreUsage.TableName] != null) dsInvoiceReport.Tables.Remove(dtStoreUsage.TableName);
            if (dsInvoiceReport.Tables[dtToolUsage.TableName] != null) dsInvoiceReport.Tables.Remove(dtToolUsage.TableName);
            if (dsInvoiceReport.Tables[dtMiscUsage.TableName] != null) dsInvoiceReport.Tables.Remove(dtMiscUsage.TableName);

            dsInvoiceReport.Merge(ds);

            DataView dvOrgs = dsInvoiceReport.Tables["Org"].DefaultView;
            dvOrgs.RowFilter = "HasActivity = 1";
            dvOrgs.Sort = "OrgName, Name";

            return dvOrgs;
        }

        public static IList<InvoiceLineItem> GetUsage(ExternalInvoiceManager mgr, int accountId)
        {
            var result = new List<InvoiceLineItem>();

            //Room Usage
            var room = mgr.GetRoomData(accountId);
            result.AddRange(room);

            //ToolUsage
            var tool = mgr.GetToolData(accountId);
            result.AddRange(tool);

            //Store Usage
            var store = mgr.GetStoreData(accountId);
            result.AddRange(store);

            //Misc 
            var misc = mgr.GetMiscData(accountId);
            result.AddRange(misc);

            return result;
        }

        public static DataView GetUsageSummary(DataView dvOrgs, bool showRemote, DateTime startPeriod, DateTime endPeriod)
        {
            if (dvOrgs == null) return null;

            DataView dvChargeType = GetExternalChargeTypes();

            if (dvChargeType == null) return null;

            ExternalInvoiceManager mgr = new ExternalInvoiceManager(startPeriod, endPeriod, showRemote);

            IList<InvoiceLineItem> usage;

            foreach (DataRowView drv in dvOrgs)
            {
                int accountId = Convert.ToInt32(drv["AccountID"]);

                usage = GetUsage(mgr, accountId);

                DataRow[] rows = dvChargeType.Table.Select(string.Format("ChargeTypeID = {0}", drv["ChargeTypeID"]));

                if (rows.Length > 0)
                {
                    DataRow dr = rows.First();

                    double total = usage.Sum(x => x.LineTotal);

                    if (dr["Total"] == DBNull.Value)
                        dr["Total"] = 0.0;

                    total += Convert.ToDouble(dr["Total"]);

                    dr["Total"] = total;
                }
            }

            return dvChargeType;
        }

        public static DataView GetExternalChargeTypes()
        {
            using (var dba = DA.Current.GetAdapter())
            {
                DataTable dtChargeType = dba.ApplyParameters(new { Action = "All" }).FillDataTable("ChargeType_Select");
                if (dtChargeType == null) return null;
                dtChargeType.Columns.Add("Total", typeof(double));
                int internalChargeTypeId = 5;
                return new DataView(dtChargeType, string.Format("ChargeTypeID <> {0}", internalChargeTypeId), "ChargeTypeID ASC", DataViewRowState.CurrentRows);
            }
        }
    }
}
