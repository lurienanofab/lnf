using LNF.CommonTools;
using LNF.Data;
using LNF.Scheduler;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Billing.Reports
{
    public static class ToolDetailUtility
    {
        public static ToolDetailResult GetToolDetailResult(DateTime period, int clientId, Reporting.Individual.ToolBilling toolBilling)
        {
            var result = new ToolDetailResult
            {
                Period = period,
                ClientID = clientId
            };

            var query = toolBilling.Provider.Billing.Tool.GetToolBilling(period, clientId);

            // need all resources, not just active, because we may be looking at historical data
            IEnumerable<IResource> resources = toolBilling.Provider.Scheduler.Resource.GetResources();
            IEnumerable<IAccount> accounts = toolBilling.Provider.Data.Account.GetAccounts();

            DataTable dtTool = toolBilling.GetAggreateByTool(query, resources, accounts);

            result.Items = dtTool.Select(string.Empty, "RoomName ASC, ResourceName ASC").Select(CreateToolDetailItem).ToList();

            SetLabels(dtTool, result.LabelTool, result.LabelRoomSum, result.LabelResFee);

            return result;
        }

        public static void SetLabels(DataTable dtTool, BillingReportLabel labelTool, BillingReportLabel labelRoomSum, BillingReportLabel labelResFee)
        {
            decimal subTotalActivated = 0;

            labelRoomSum.Text = string.Empty;
            labelResFee.Text = string.Empty;

            if (dtTool.Rows.Count > 0)
            {
                subTotalActivated = Convert.ToDecimal(dtTool.Compute("SUM(LineCost)", string.Empty));
                labelResFee.Text = string.Format("| Sub Total: {0:$#,##0.00}", subTotalActivated);
                labelResFee.Visible = true;
                UpdateRoomSums(dtTool, labelRoomSum);
            }

            if (subTotalActivated == 0)
                labelTool.Text = "No tool usage fees in this period";
            else
                labelTool.Text = string.Format("Total tool usage fees: {0:$#,##0.00}", subTotalActivated);

            labelTool.Visible = true;
        }

        private static void UpdateRoomSums(DataTable dt, BillingReportLabel lbl)
        {
            //2009-12-11 Used to calculate differen room charges, because users want to see those differentiated
            GetRoomSums(dt, out object lnfSum, out object cleanRoomSum, out object wetChemSum, out object testLabSum);
            UpdateSumLabel(lnfSum, lbl, "LNF");
            UpdateSumLabel(cleanRoomSum, lbl, "Clean Room");
            UpdateSumLabel(wetChemSum, lbl, "ROBIN");
            UpdateSumLabel(testLabSum, lbl, "DC Lab");
        }

        private static void GetRoomSums(DataTable dt, out object lnfSum, out object cleanRoomSum, out object wetChemSum, out object testLabSum)
        {
            lnfSum = dt.Compute("SUM(LineCost)", "RoomID = 154");
            cleanRoomSum = dt.Compute("SUM(LineCost)", "RoomID = 6");
            wetChemSum = dt.Compute("SUM(LineCost)", "RoomID = 25");
            testLabSum = dt.Compute("SUM(LineCost)", "RoomID = 2");
        }

        private static void UpdateSumLabel(object sum, BillingReportLabel lbl, string roomName)
        {
            if (Utility.TryConvertTo(sum, out double temp, 0.0))
            {
                if (!string.IsNullOrEmpty(lbl.Text)) lbl.Text += " | ";
                lbl.Text += string.Format("{0}: {1:$#,##0.00}", roomName, temp);
                lbl.Visible = true;
            }
        }

        private static ToolDetailItem CreateToolDetailItem(DataRow dr)
        {
            var result = new ToolDetailItem
            {
                ClientID = dr.Field<int>("ClientID"),
                ResourceID = dr.Field<int>("ResourceID"),
                ResourceName = dr.Field<string>("ResourceName"),
                RoomID = dr.Field<int>("RoomID"),
                RoomName = dr.Field<string>("RoomName"),
                ActivatedUsed = dr.Field<double>("ActivatedUsed"),
                ActivatedUnused = dr.Field<double>("ActivatedUnused"),
                TotalOverTime = dr.Field<decimal>("TotalOverTime"),
                OverTimePenaltyFee = dr.Field<decimal>("OverTimePenaltyFee"),
                UnstartedUnused = dr.Field<double>("UnstartedUnused"),
                BookingFee = dr.Field<decimal>("BookingFee"),
                TotalTransferredDuration = dr.Field<decimal>("TotalTransferredDuration"),
                TotalForgivenDuration = dr.Field<decimal>("TotalForgivenDuration"),
                ResourceRate = dr.Field<decimal>("ResourceRate"),
                AccountID = dr.Field<int>("AccountID"),
                AccountName = dr.Field<string>("AccountName"),
                ShortCode = dr.Field<string>("ShortCode"),
                LineCost = dr.Field<decimal>("LineCost")
            };

            return result;
        }
    }

    public class ToolDetailResult
    {
        public ToolDetailResult()
        {
            LabelRoomSum = new BillingReportLabel();
            LabelResFee = new BillingReportLabel();
            LabelTool = new BillingReportLabel();
        }

        public DateTime Period { get; set; }
        public int ClientID { get; set; }
        public BillingReportLabel LabelRoomSum { get; set; }
        public BillingReportLabel LabelResFee { get; set; }
        public BillingReportLabel LabelTool { get; set; }
        public IEnumerable<ToolDetailItem> Items { get; set; }
    }

    public class ToolDetailItem
    {
        public int ClientID { get; set; }
        public int ResourceID { get; set; }
        public string ResourceName { get; set; }
        public int RoomID { get; set; }
        public string RoomName { get; set; }
        public double ActivatedUsed { get; set; }
        public double ActivatedUnused { get; set; }
        public decimal TotalOverTime { get; set; }
        public decimal OverTimePenaltyFee { get; set; }
        public double UnstartedUnused { get; set; }
        public decimal BookingFee { get; set; }
        public decimal TotalTransferredDuration { get; set; }
        public decimal TotalForgivenDuration { get; set; }
        public decimal ResourceRate { get; set; }
        public int AccountID { get; set; }
        public string AccountName { get; set; }
        public string ShortCode { get; set; }
        public decimal LineCost { get; set; }
    }

    public class BillingReportLabel
    {
        public string Text { get; set; }
        public bool Visible { get; set; }
    }
}
