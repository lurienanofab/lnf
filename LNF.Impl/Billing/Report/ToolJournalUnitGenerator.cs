﻿using LNF.Models.Billing.Reports.ServiceUnitBilling;
using System.Data;

namespace LNF.Impl.Billing.Report
{
    public class ToolJournalUnitGenerator : JournalUnitGenerator<ToolJU>
    {
        private ToolJournalUnitGenerator(ToolJU report) : base(report) { }

        public static ToolJournalUnitGenerator Create(ToolJU report)
        {
            return new ToolJournalUnitGenerator(report);
        }

        protected override void GenerateDataTables()
        {
            object queryParameters;

            if (Report.ClientID == 0)
                queryParameters = new { Action = "ForSUBReport", Report.StartPeriod, Report.EndPeriod };
            else
                queryParameters = new { Action = "ForSUBReport", Report.StartPeriod, Report.EndPeriod, Report.ClientID };

            var ds = DataAccess.ToolBillingSelect(queryParameters);

            DataTable dtBillingData = ds.Tables[0];
            ClientAccountData = ds.Tables[1];
            ApplyFormula(dtBillingData);
            ApplyFilter();
            ApplyMiscCharge(dtBillingData, Report.ClientID);
            ProcessTable(dtBillingData);
        }
    }
}