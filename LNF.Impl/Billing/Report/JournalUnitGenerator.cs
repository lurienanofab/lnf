using LNF.Billing;
using LNF.Billing.Reports.ServiceUnitBilling;
using LNF.CommonTools;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Data;

namespace LNF.Impl.Billing.Report
{
    public abstract class JournalUnitGenerator<T> : ReportGenerator<T> where T : JournalUnitReport, new()
    {
        protected JournalUnitGenerator(ISession session, T report) : base(session, report) { }

        protected override void LoadReportItems(DataView dv)
        {
            var items = new List<JournalUnitReportItem>();

            foreach (DataRowView drv in dv)
            {
                var i = CreateJournalUnitReportItem(drv);
                items.Add(i);
            }

            Report.Items = items.ToArray();
        }

        //this should be called by the inheriting class in the GenerateDataTablesForSUB override
        protected override void ProcessTable(DataTable dtBilling)
        {
            DataTable dtReport = InitTable();

            double chargeAmount;
            string journalLineRef = string.Empty;
            double subsidyDiscount;
            double total = 0;

            //for loop each record in clientID and AccountID aggregate
            foreach (DataRow cadr in ClientAccountData.Rows)
            {
                if (cadr.RowState != DataRowState.Deleted)
                {
                    ValidPeriodCheck(cadr);

                    chargeAmount = Math.Round(Convert.ToDouble(dtBilling.Compute("SUM(LineCost)", DataRowFilter(cadr))), 2);
                    if (Math.Abs(chargeAmount) > 0.01)
                    {
                        subsidyDiscount = Utility.ConvertTo(dtBilling.Compute("SUM(SubsidyDiscount)", DataRowFilter(cadr)), 0D);
                        if (chargeAmount != 0 && subsidyDiscount != 0)
                        {
                            DataRow[] billingrows = dtBilling.Select(DataRowFilter(cadr));
                            DataRow drBilling = billingrows[0];
                            string debitAccount = Utility.ConvertTo(drBilling["Number"], string.Empty);
                            ReportAccount dai = new ReportAccount(debitAccount);

                            //get manager's name
                            journalLineRef = Utility.Clip(ManagerName(drBilling), 10);

                            switch (Report.JournalUnitType)
                            {
                                case JournalUnitTypes.A:
                                    ProcessJUA(dtReport, drBilling, dai, journalLineRef, subsidyDiscount, ref total);
                                    break;
                                case JournalUnitTypes.B:
                                    ProcessJUB(dtReport, drBilling, dai, journalLineRef, subsidyDiscount, ref total);
                                    break;
                                case JournalUnitTypes.C:
                                    ProcessJUC(dtReport, drBilling, dai, journalLineRef, subsidyDiscount, ref total);
                                    break;
                                default:
                                    throw new ArgumentException("Invalid JournalUnitType. Allowed values: A, B, C");
                            }
                        }
                    }
                }
            }

            _ReportTables.Add(dtReport);

            //Summary row
            ReportAccount cai = new ReportAccount(CreditAccount);

            DateTime period = Report.EndPeriod.AddMonths(-1);

            string account = (Report.JournalUnitType == JournalUnitTypes.C) ? "613280" : cai.Account;

            /*
            [2021-11-22 jg] Add the JournalUnitType next to BillingCategory in the ItemDescription

            Excerpt from email from Dave DeWeerd Oct 27, 2021:

            2) I’d add the case type so for instance rather than
                08/21 LNF Room Subsidy;SUB662
            enter
                08/21 LNF RoomA Subsidy;SUB662
            or 
                08/21 LNF Tool Subsidy;SUB663
            enter
                08/21 LNF ToolC Subsidy;SUB663
            (this line I believe comes through on subsidy funds, so when we review the non-943 activity super clear which case it is if we ever need to reference it)
            Other than those two items, seems a nice update, consistent with how we have been submitting the billing and probably takes out a few steps in the process, adds more consistency to the data
            */

            Report.CreditEntry = new CreditEntry
            {
                Account = account,
                FundCode = cai.FundCode,
                DeptID = cai.DeptID,
                ProgramCode = cai.ProgramCode,
                ClassName = cai.Class,
                ProjectGrant = cai.ProjectGrant,
                DepartmentalReferenceNumber = ReportSettings.FinancialManagerUserName,
                // CreditEntryItemDescription example: {0:MM/yy} {1} {2}{3} Subsidy;SUB{4}
                ItemDescription = string.Format(
                    Utility.GetRequiredAppSetting("CreditEntryItemDescription"),
                    /*0*/ period,
                    /*1*/ ReportSettings.CompanyName,
                    /*2*/ Utility.EnumToString(Report.BillingCategory),
                    /*3*/ Utility.EnumToString(Report.JournalUnitType),
                    /*4*/ ReportSettings.GetServiceUnitBillingNumber(period, Report.BillingCategory)
                ),
                MerchandiseAmount = Math.Round(-total, 2),
                CreditAccount = CreditAccount
            };

            DataRow totalrow = dtReport.NewRow();
            totalrow["ReportType"] = Utility.EnumToString(Report.ReportType);
            totalrow["ChargeType"] = Utility.EnumToString(Report.BillingCategory);
            totalrow["JournalUnitType"] = Report.JournalUnitType;
            totalrow["Period"] = period;
            totalrow["Account"] = account;
            totalrow["FundCode"] = cai.FundCode;
            totalrow["DeptID"] = cai.DeptID;
            totalrow["ProgramCode"] = cai.ProgramCode;
            totalrow["Class"] = cai.Class;
            totalrow["ProjectGrant"] = cai.ProjectGrant;
            totalrow["DepartmentalReferenceNumber"] = string.Empty;
            totalrow["ItemDescription"] = $"zz{ReportSettings.FinancialManagerUserName}";
            totalrow["MerchandiseAmount"] = Math.Round(-total, 2).ToString("0.00");
            dtReport.Rows.Add(totalrow);
        }

        protected override DataTable InitTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ReportType", typeof(string));
            dt.Columns.Add("ChargeType", typeof(string));
            dt.Columns.Add("JournalUnitType", typeof(string));
            dt.Columns.Add("Period", typeof(DateTime));
            dt.Columns.Add("Account", typeof(string));
            dt.Columns.Add("FundCode", typeof(string));
            dt.Columns.Add("DeptID", typeof(string));
            dt.Columns.Add("ProgramCode", typeof(string));
            dt.Columns.Add("Class", typeof(string));
            dt.Columns.Add("ProjectGrant", typeof(string));
            dt.Columns.Add("DepartmentalReferenceNumber", typeof(string));
            dt.Columns.Add("ItemDescription", typeof(string));
            dt.Columns.Add("MerchandiseAmount", typeof(string));
            //Not belong in the excel format, but added for other purpose
            dt.Columns.Add("CreditAccount", typeof(string));
            dt.Columns.Add("AccountID", typeof(string));
            return dt;
        }

        private void ProcessJUA(DataTable dtReport, DataRow drBilling, ReportAccount debit_acct, string JournalLineRef, double SubsidyDiscount, ref double Total)
        {
            if (debit_acct.FundCode == "20000" || debit_acct.FundCode == "25000")
            {
                DataRow newdr = dtReport.NewRow();
                newdr["ReportType"] = Utility.EnumToString(Report.ReportType);
                newdr["ChargeType"] = Utility.EnumToString(Report.BillingCategory);
                newdr["JournalUnitType"] = Report.JournalUnitType;
                newdr["Period"] = drBilling["Period"];
                newdr["Account"] = debit_acct.Account;
                newdr["FundCode"] = debit_acct.FundCode;
                newdr["DeptID"] = debit_acct.DeptID;
                newdr["ProgramCode"] = debit_acct.ProgramCode;
                newdr["Class"] = debit_acct.Class;
                newdr["ProjectGrant"] = debit_acct.ProjectGrant;
                newdr["DepartmentalReferenceNumber"] = JournalLineRef;
                newdr["ItemDescription"] = GetItemDescription(drBilling, $"{ReportSettings.CompanyName}{ChargeTypeAbbreviation()}A");
                newdr["MerchandiseAmount"] = (Math.Round(SubsidyDiscount, 2) * -1).ToString("0.00");

                //Used to calculate the total credit amount
                Total += Utility.ConvertTo(newdr["MerchandiseAmount"], 0D);

                dtReport.Rows.Add(newdr);

                //2nd record of JU
                newdr = dtReport.NewRow();
                newdr["ReportType"] = Utility.EnumToString(Report.ReportType);
                newdr["ChargeType"] = Utility.EnumToString(Report.BillingCategory);
                newdr["JournalUnitType"] = Report.JournalUnitType;
                newdr["Period"] = Utility.ConvertTo(drBilling["Period"], DateTime.MinValue);
                newdr["Account"] = debit_acct.Account;
                newdr["FundCode"] = "10000";
                newdr["DeptID"] = debit_acct.DeptID;
                newdr["ProgramCode"] = debit_acct.ProgramCode;
                newdr["Class"] = debit_acct.Class;
                newdr["ProjectGrant"] = debit_acct.ProjectGrant;
                newdr["DepartmentalReferenceNumber"] = JournalLineRef;
                newdr["ItemDescription"] = GetItemDescription(drBilling, $"{ReportSettings.CompanyName}{ChargeTypeAbbreviation()}A");
                newdr["MerchandiseAmount"] = Math.Round(SubsidyDiscount, 2).ToString("0.00");

                dtReport.Rows.Add(newdr);

                //3rd record of JU A
                newdr = dtReport.NewRow();
                newdr["ReportType"] = Utility.EnumToString(Report.ReportType);
                newdr["ChargeType"] = Utility.EnumToString(Report.BillingCategory);
                newdr["JournalUnitType"] = Report.JournalUnitType;
                newdr["Period"] = Utility.ConvertTo(drBilling["Period"], DateTime.MinValue);
                newdr["Account"] = "450600";
                newdr["FundCode"] = "10000";
                newdr["DeptID"] = debit_acct.DeptID;
                newdr["ProgramCode"] = debit_acct.ProgramCode;
                newdr["Class"] = debit_acct.Class;
                newdr["ProjectGrant"] = debit_acct.ProjectGrant;
                newdr["DepartmentalReferenceNumber"] = JournalLineRef;
                newdr["ItemDescription"] = GetItemDescription(drBilling, $"{ReportSettings.CompanyName}{ChargeTypeAbbreviation()}A");
                newdr["MerchandiseAmount"] = (Math.Round(SubsidyDiscount, 2) * -1).ToString("0.00");

                dtReport.Rows.Add(newdr);
            }
        }

        private void ProcessJUB(DataTable dtReport, DataRow drBilling, ReportAccount debit_acct, string JournalLineRef, double SubsidyDiscount, ref double Total)
        {
            if (debit_acct.FundCode == "10000" && debit_acct.ProgramCode == "CSTSH")
            {
                DataRow newdr = dtReport.NewRow();
                newdr["ReportType"] = Utility.EnumToString(Report.ReportType);
                newdr["ChargeType"] = Utility.EnumToString(Report.BillingCategory);
                newdr["JournalUnitType"] = Report.JournalUnitType;
                newdr["Period"] = drBilling["Period"];
                newdr["Account"] = "450600";
                newdr["FundCode"] = debit_acct.FundCode;
                newdr["DeptID"] = debit_acct.DeptID;
                newdr["ProgramCode"] = debit_acct.ProgramCode;
                newdr["Class"] = debit_acct.Class;
                newdr["ProjectGrant"] = debit_acct.ProjectGrant;
                newdr["DepartmentalReferenceNumber"] = JournalLineRef;
                newdr["ItemDescription"] = GetItemDescription(drBilling, $"{ReportSettings.CompanyName}{ChargeTypeAbbreviation()}B");
                newdr["MerchandiseAmount"] = (Math.Round(SubsidyDiscount, 2) * -1).ToString("0.00");

                //Used to calculate the total credit amount
                Total += Utility.ConvertTo(newdr["MerchandiseAmount"], 0D);

                dtReport.Rows.Add(newdr);
            }
        }

        private void ProcessJUC(DataTable dtReport, DataRow drBilling, ReportAccount debitAcct, string journalLineRef, double subsidyDiscount, ref double total)
        {
            if (debitAcct.FundCode != "20000" && debitAcct.FundCode != "25000" && !(debitAcct.FundCode == "10000" && debitAcct.ProgramCode == "CSTSH"))
            {
                DataRow newdr = dtReport.NewRow();
                newdr["ReportType"] = Utility.EnumToString(Report.ReportType);
                newdr["ChargeType"] = Utility.EnumToString(Report.BillingCategory);
                newdr["JournalUnitType"] = Report.JournalUnitType;
                newdr["Period"] = drBilling["Period"];
                newdr["Account"] = debitAcct.Account;
                newdr["FundCode"] = debitAcct.FundCode;
                newdr["DeptID"] = debitAcct.DeptID;
                newdr["ProgramCode"] = debitAcct.ProgramCode;
                newdr["Class"] = debitAcct.Class;
                newdr["ProjectGrant"] = debitAcct.ProjectGrant;
                newdr["DepartmentalReferenceNumber"] = journalLineRef;
                newdr["ItemDescription"] = GetItemDescription(drBilling, $"{ReportSettings.CompanyName}{ChargeTypeAbbreviation()}C");
                newdr["MerchandiseAmount"] = (Math.Round(subsidyDiscount, 2) * -1).ToString("0.00");

                //Used to calculate the total credit amount
                total += Utility.ConvertTo(newdr["MerchandiseAmount"], 0D);

                dtReport.Rows.Add(newdr);
            }
        }

        private static readonly Dictionary<BillingCategory, string> ChargeTypeAbbreviationLookup = new Dictionary<BillingCategory, string>()
        {
            { BillingCategory.Room | BillingCategory.Tool | BillingCategory.Store, "al" },
            { BillingCategory.Room, "rm" },
            { BillingCategory.Tool, "tl" },
            { BillingCategory.Store, "st" }
        };

        private string ChargeTypeAbbreviation()
        {
            return ChargeTypeAbbreviationLookup[Report.BillingCategory];
        }

        private JournalUnitReportItem CreateJournalUnitReportItem(DataRowView drv)
        {
            return new JournalUnitReportItem()
            {
                ReportType = Utility.ConvertTo(drv["ReportType"], string.Empty),
                ChargeType = Utility.ConvertTo(drv["ChargeType"], string.Empty),
                JournalUnitType = Utility.ConvertTo(drv["JournalUnitType"], string.Empty),
                Period = Utility.ConvertTo(drv["Period"], DateTime.MinValue),
                Account = Utility.ConvertTo(drv["Account"], string.Empty),
                FundCode = Utility.ConvertTo(drv["FundCode"], string.Empty),
                DeptID = Utility.ConvertTo(drv["DeptID"], string.Empty),
                ProgramCode = Utility.ConvertTo(drv["ProgramCode"], string.Empty),
                Class = Utility.ConvertTo(drv["Class"], string.Empty),
                ProjectGrant = Utility.ConvertTo(drv["ProjectGrant"], string.Empty),
                DepartmentalReferenceNumber = Utility.ConvertTo(drv["DepartmentalReferenceNumber"], string.Empty),
                ItemDescription = Utility.ConvertTo(drv["ItemDescription"], string.Empty),
                MerchandiseAmount = Utility.ConvertTo(drv["MerchandiseAmount"], string.Empty),
                CreditAccount = Utility.ConvertTo(drv["CreditAccount"], string.Empty),
                AccountID = Utility.ConvertTo(drv["AccountID"], string.Empty)
            };
        }
    }
}