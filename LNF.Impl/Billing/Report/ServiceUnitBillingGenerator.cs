using LNF.Billing;
using LNF.Billing.Reports.ServiceUnitBilling;
using LNF.CommonTools;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Impl.Billing.Report
{
    public abstract class ServiceUnitBillingGenerator<T> : ReportGenerator<T> where T : ServiceUnitBillingReport, new()
    {
        protected ServiceUnitBillingGenerator(ISession session, T report) : base(session, report) { }

        protected override void LoadReportItems(DataView dv)
        {
            List<ServiceUnitBillingReportItem> items = new List<ServiceUnitBillingReportItem>();

            foreach (DataRowView drv in dv)
            {
                var i = CreateServiceUnitBillingReportItem(drv);
                items.Add(i);
            }

            AddCombinedItems(items);
            AddItems(items);
        }

        //this should be called by the inheriting class in the GenerateDataTablesForSUB override
        protected override void ProcessTable(DataTable dtBilling)
        {
            DataTable dtReport = InitTable();

            string deptRefNum = string.Empty;
            double chargeAmount = 0;
            double subsidyDiscount = 0;
            double total = 0;

            BillingUnit summary = new BillingUnit();

            //for loop each record in ClientID and AccountID aggregate
            foreach (DataRow cadr in ClientAccountData.Rows)
            {
                if (cadr.RowState != DataRowState.Deleted)
                {
                    ValidPeriodCheck(cadr);

                    chargeAmount = Math.Round(Utility.ConvertTo(dtBilling.Compute("SUM(LineCost)", DataRowFilter(cadr)), 0D), 2);
                    if (dtBilling.Columns.Contains("SubsidyDiscount"))
                        subsidyDiscount = Utility.ConvertTo(dtBilling.Compute("SUM(SubsidyDiscount)", DataRowFilter(cadr)), 0D);

                    if (chargeAmount != 0)
                    {
                        //2011-02-08 There will be some unavoidable rounding difference, so we basicaly ignore anything for 1 cent
                        if (Math.Abs(chargeAmount) > 0.01)
                        {
                            DataRow[] billingrows = dtBilling.Select(DataRowFilter(cadr));
                            if (billingrows.Length > 0)
                            {
                                DataRow dr = billingrows[0];
                                string debitAcctNumber = Utility.ConvertTo(dr["Number"], string.Empty);
                                ReportAccount debitAcct = new ReportAccount(debitAcctNumber);

                                //get manager's name
                                deptRefNum = ManagerName(cadr);

                                DateTime p = Utility.ConvertTo(dr["Period"], DateTime.MinValue);
                                DateTime invoiceDate = (p.Equals(DateTime.MinValue)) ? Report.EndPeriod.AddMonths(-1) : p;

                                DataRow newdr = dtReport.NewRow();
                                newdr["ReportType"] = ReportUtility.EnumToString(Report.ReportType);
                                newdr["ChargeType"] = ReportUtility.EnumToString(Report.BillingCategory);
                                newdr["Period"] = dr["Period"];
                                newdr["CardType"] = 1;
                                newdr["ShortCode"] = dr["ShortCode"];
                                newdr["Account"] = debitAcct.Account;
                                newdr["FundCode"] = debitAcct.FundCode;
                                newdr["DeptID"] = debitAcct.DeptID;
                                newdr["ProgramCode"] = debitAcct.ProgramCode;
                                newdr["Class"] = debitAcct.Class;
                                newdr["ProjectGrant"] = debitAcct.ProjectGrant;
                                newdr["VendorID"] = "0000456136"; //wtf?
                                newdr["InvoiceDate"] = invoiceDate.ToString("yyyy/MM/dd");
                                newdr["InvoiceID"] = GetInvoiceID();
                                newdr["Uniqname"] = dr["UserName"];
                                newdr["DepartmentalReferenceNumber"] = deptRefNum;
                                newdr["ItemDescription"] = GetItemDescription(dr);
                                newdr["QuantityVouchered"] = "1.0000";
                                newdr["CreditAccount"] = CreditAccount;
                                newdr["UsageCharge"] = Math.Round(chargeAmount, 2).ToString("0.00");
                                newdr["SubsidyDiscount"] = Math.Round(subsidyDiscount, 2).ToString("0.00");
                                newdr["BilledCharge"] = Math.Round(chargeAmount - subsidyDiscount, 2).ToString("0.00");
                                newdr["UnitOfMeasure"] = Math.Round(chargeAmount, 5).ToString("0.00000");
                                newdr["MerchandiseAmount"] = newdr["UsageCharge"];

                                //Used to calculate the total credit amount
                                total += chargeAmount;

                                //for testing purpose
                                newdr["AccountID"] = cadr["AccountID"];

                                dtReport.Rows.Add(newdr);
                            }
                        }
                    }
                }
            }

            _ReportTables.Add(dtReport);

            //Summary row
            ReportAccount credit_acct = new ReportAccount(CreditAccount);
            summary.CardType = 1;
            summary.ShortCode = CreditAccountShortCode;
            summary.Account = credit_acct.Account;
            summary.FundCode = credit_acct.FundCode;
            summary.DeptID = credit_acct.DeptID;
            summary.ProgramCode = credit_acct.ProgramCode;
            summary.ClassName = credit_acct.Class;
            summary.ProjectGrant = credit_acct.ProjectGrant;
            summary.InvoiceDate = Report.EndPeriod.AddMonths(-1).ToString("yyyy/MM/dd");
            summary.Uniqname = "doscar"; //wtf?
            summary.DepartmentalReferenceNumber = deptRefNum;
            summary.ItemDescription = "doscar"; //wtf?
            summary.MerchandiseAmount = -total;
            summary.CreditAccount = CreditAccount;
            summary.QuantityVouchered = "1.0000";
            AddSummary(summary);

            double SumUsageCharge = 0;
            double SumSubsidyDiscount = 0;
            double SumBilledCharge = 0;

            foreach (DataRow dr in dtReport.Rows)
            {
                SumUsageCharge += Utility.ConvertTo(dr["UsageCharge"], 0D);
                SumSubsidyDiscount += Utility.ConvertTo(dr["SubsidyDiscount"], 0D);
                SumBilledCharge += Utility.ConvertTo(dr["BilledCharge"], 0D);
                if (Report.BillingCategory == BillingCategory.Store)
                {
                    dr["UsageCharge"] = string.Empty;
                    dr["SubsidyDiscount"] = string.Empty;
                }
            }

            DataRow totalrow = dtReport.NewRow();
            totalrow["ReportType"] = ReportUtility.EnumToString(Report.ReportType);
            totalrow["ChargeType"] = ReportUtility.EnumToString(Report.BillingCategory);
            totalrow["Period"] = Report.EndPeriod.AddMonths(-1);
            totalrow["ShortCode"] = dtReport.Rows.Count - 1;
            totalrow["UsageCharge"] = (Report.BillingCategory == BillingCategory.Store) ? string.Empty : SumUsageCharge.ToString("0.00");
            totalrow["SubsidyDiscount"] = (Report.BillingCategory == BillingCategory.Store) ? string.Empty : SumSubsidyDiscount.ToString("0.00");
            totalrow["BilledCharge"] = SumBilledCharge.ToString("0.00");
            dtReport.Rows.Add(totalrow);
        }

        protected override DataTable InitTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ReportType", typeof(string));
            dt.Columns.Add("ChargeType", typeof(string));
            dt.Columns.Add("Period", typeof(DateTime));
            dt.Columns.Add("CardType", typeof(string));
            dt.Columns.Add("ShortCode", typeof(string));
            dt.Columns.Add("Account", typeof(string));
            dt.Columns.Add("FundCode", typeof(string));
            dt.Columns.Add("DeptID", typeof(string));
            dt.Columns.Add("ProgramCode", typeof(string));
            dt.Columns.Add("Class", typeof(string));
            dt.Columns.Add("ProjectGrant", typeof(string));
            dt.Columns.Add("VendorID", typeof(string));
            dt.Columns.Add("InvoiceDate", typeof(string));
            dt.Columns.Add("InvoiceID", typeof(string));
            dt.Columns.Add("Uniqname", typeof(string));
            dt.Columns.Add("LocationCode", typeof(string));
            dt.Columns.Add("DeliverTo", typeof(string));
            dt.Columns.Add("VendorOrderNum", typeof(string));
            dt.Columns.Add("DepartmentalReferenceNumber", typeof(string));
            dt.Columns.Add("Trip/EventNumber", typeof(string));
            dt.Columns.Add("ItemID", typeof(string));
            dt.Columns.Add("ItemDescription", typeof(string));
            dt.Columns.Add("VendorItemID", typeof(string));
            dt.Columns.Add("ManufacturerName", typeof(string));
            dt.Columns.Add("ModelNum", typeof(string));
            dt.Columns.Add("SerialNum", typeof(string));
            dt.Columns.Add("UMTagNum", typeof(string));
            dt.Columns.Add("QuantityVouchered", typeof(string));
            dt.Columns.Add("UnitOfMeasure", typeof(string));
            dt.Columns.Add("UnitPrice", typeof(string));
            dt.Columns.Add("MerchandiseAmount", typeof(string));
            dt.Columns.Add("VoucherComment", typeof(string));
            dt.Columns.Add("SubsidyDiscount", typeof(string));
            dt.Columns.Add("BilledCharge", typeof(string));
            dt.Columns.Add("UsageCharge", typeof(string));
            //Not belong in the excel format, but added for other purpose
            dt.Columns.Add("CreditAccount", typeof(string));
            dt.Columns.Add("AccountID", typeof(string));
            return dt;
        }

        private void AddItems(List<ServiceUnitBillingReportItem> items)
        {
            List<ServiceUnitBillingReportItem[]> reportItems;

            if (Report.Items != null)
                reportItems = Report.Items.ToList();
            else
                reportItems = new List<ServiceUnitBillingReportItem[]>();

            reportItems.Add(items.ToArray());

            Report.Items = reportItems.ToArray();
        }

        private void AddCombinedItems(List<ServiceUnitBillingReportItem> items)
        {
            List<ServiceUnitBillingReportItem> combinedItems;

            if (Report.CombinedItems != null)
                combinedItems = Report.CombinedItems.ToList();
            else
                combinedItems = new List<ServiceUnitBillingReportItem>();

            combinedItems.AddRange(items);

            Report.CombinedItems = combinedItems.ToArray();
        }

        private void AddSummary(BillingUnit billingUnit)
        {
            List<BillingUnit> summaries;

            if (Report.Summaries != null)
                summaries = Report.Summaries.ToList();
            else
                summaries = new List<BillingUnit>();

            summaries.Add(billingUnit);

            Report.Summaries = summaries.ToArray();
        }

        private string GetInvoiceID()
        {
            return string.Format("SUB {0} {1} LNF {2}"
                , GetServiceUnitBillingNumber()
                , Report.EndPeriod.AddMonths(-1).ToString("MM/yy")
                , ReportUtility.EnumToString(Report.BillingCategory).ToLower()
            );
        }

        private int GetServiceUnitBillingNumber()
        {
            DateTime Period = Report.EndPeriod.AddMonths(-1);
            DateTime July2010 = new DateTime(2010, 7, 1);
            int yearoff = Period.Year - July2010.Year;
            int monthoff = Period.Month - July2010.Month;

            int increment = (yearoff * 12 + monthoff) * 3;

            //263 is the starting number for room sub in July 2010
            if (Report.BillingCategory == BillingCategory.Tool)
                return 263 + increment + 1;
            else if (Report.BillingCategory == BillingCategory.Store)
                return 263 + increment + 2;
            else
                return 263 + increment;
        }

        private ServiceUnitBillingReportItem CreateServiceUnitBillingReportItem(DataRowView drv)
        {
            return new ServiceUnitBillingReportItem()
            {
                ReportType = Utility.ConvertTo(drv["ReportType"], string.Empty),
                ChargeType = Utility.ConvertTo(drv["ChargeType"], string.Empty),
                Period = Utility.ConvertTo(drv["Period"], DateTime.MinValue),
                CardType = Utility.ConvertTo(drv["CardType"], string.Empty),
                ShortCode = Utility.ConvertTo(drv["ShortCode"], string.Empty),
                Account = Utility.ConvertTo(drv["Account"], string.Empty),
                FundCode = Utility.ConvertTo(drv["FundCode"], string.Empty),
                DeptID = Utility.ConvertTo(drv["DeptID"], string.Empty),
                ProgramCode = Utility.ConvertTo(drv["ProgramCode"], string.Empty),
                Class = Utility.ConvertTo(drv["Class"], string.Empty),
                ProjectGrant = Utility.ConvertTo(drv["ProjectGrant"], string.Empty),
                VendorID = Utility.ConvertTo(drv["VendorID"], string.Empty),
                InvoiceDate = Utility.ConvertTo(drv["InvoiceDate"], string.Empty),
                InvoiceID = Utility.ConvertTo(drv["InvoiceID"], string.Empty),
                Uniqname = Utility.ConvertTo(drv["Uniqname"], string.Empty),
                LocationCode = Utility.ConvertTo(drv["LocationCode"], string.Empty),
                DeliverTo = Utility.ConvertTo(drv["DeliverTo"], string.Empty),
                VendorOrderNum = Utility.ConvertTo(drv["VendorOrderNum"], string.Empty),
                DepartmentalReferenceNumber = Utility.ConvertTo(drv["DepartmentalReferenceNumber"], string.Empty),
                TripOrEventNumber = Utility.ConvertTo(drv["Trip/EventNumber"], string.Empty),
                ItemID = Utility.ConvertTo(drv["ItemID"], string.Empty),
                ItemDescription = Utility.ConvertTo(drv["ItemDescription"], string.Empty),
                VendorItemID = Utility.ConvertTo(drv["VendorItemID"], string.Empty),
                ManufacturerName = Utility.ConvertTo(drv["ManufacturerName"], string.Empty),
                ModelNum = Utility.ConvertTo(drv["ModelNum"], string.Empty),
                SerialNum = Utility.ConvertTo(drv["SerialNum"], string.Empty),
                UMTagNum = Utility.ConvertTo(drv["UMTagNum"], string.Empty),
                QuantityVouchered = Utility.ConvertTo(drv["QuantityVouchered"], string.Empty),
                UnitOfMeasure = Utility.ConvertTo(drv["UnitOfMeasure"], string.Empty),
                UnitPrice = Utility.ConvertTo(drv["UnitPrice"], string.Empty),
                MerchandiseAmount = Utility.ConvertTo(drv["MerchandiseAmount"], string.Empty),
                VoucherComment = Utility.ConvertTo(drv["VoucherComment"], string.Empty),
                SubsidyDiscount = Utility.ConvertTo(drv["SubsidyDiscount"], string.Empty),
                BilledCharge = Utility.ConvertTo(drv["BilledCharge"], string.Empty),
                UsageCharge = Utility.ConvertTo(drv["UsageCharge"], string.Empty),
                CreditAccount = Utility.ConvertTo(drv["CreditAccount"], string.Empty),
                AccountID = Utility.ConvertTo(drv["AccountID"], string.Empty)
            };
        }
    }
}