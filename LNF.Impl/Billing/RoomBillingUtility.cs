using LNF.Billing;
using LNF.Data;
using System.Data;

namespace LNF.Impl.Billing
{
    internal static class RoomBillingUtility
    {
        internal static void CalculateRoomLineCost(DataTable dt)
        {
            if (!dt.Columns.Contains("LineCost"))
                dt.Columns.Add("LineCost", typeof(decimal));

            if (!dt.Columns.Contains("DailyFee"))
                dt.Columns.Add("DailyFee", typeof(decimal));

            if (!dt.Columns.Contains("EntryFee"))
                dt.Columns.Add("EntryFee", typeof(decimal));

            if (!dt.Columns.Contains("Room"))
                dt.Columns.Add("Room", typeof(string));

            foreach (DataRow dr in dt.Rows)
            {
                IRoomBilling item = RoomBillingRepository.CreateRoomBillingFromDataRow(dr, false);

                dr.SetField("Room", Rooms.GetRoomDisplayName(item.RoomID));

                if (item.RoomCharge > 0)
                    dr.SetField("DailyFee", item.RoomCharge);

                if (item.EntryCharge > 0)
                    dr.SetField("EntryFee", item.EntryCharge);

                dr.SetField("LineCost", GetLineCost(item));
            }
        }

        internal static decimal GetLineCost(IRoomBilling item)
        {
            // [2015-11-13 jg] this is identical to the logic originally in:
            //      1) sselFinOps.AppCode.BLL.FormulaBL.ApplyRoomFormula (for External Invoice)
            //      2) sselIndReports.AppCode.Bll.RoomBillingBL.GetRoomBillingDataByClientID (for User Usage Summary)
            //      3) LNF.WebApi.Billing.Models.ReportUtility.ApplyRoomFormula (for SUB reports)

            decimal result = 0;

            int cleanRoomId = 6;
            int organicsBayId = 6;

            //1. Find out all Monthly type users and apply to Clean room
            if (BillingTypes.IsMonthlyUserBillingType(item.BillingTypeID))
            {
                if (item.RoomID == cleanRoomId) //Clean Room
                    result = item.MonthlyRoomCharge;
                else
                    result = item.TotalCharge;
            }
            //2. The growers are charged with room fee only when they reserve and activate a tool
            else if (BillingTypes.IsGrowerUserBillingType(item.BillingTypeID))
            {
                if (item.RoomID == organicsBayId) //Organics Bay
                    result = item.RoomCharge; //organics bay must be charged for growers as well
                else
                    result = item.AccountDays * item.RoomRate + item.EntryCharge;
            }
            else if (item.BillingTypeID == BillingTypes.Other)
            {
                result = 0;
            }
            else if (item.BillingTypeID == BillingTypes.Grower_Observer)
            {
                result = item.TotalCharge;
            }
            else
            {
                //Per Use types
                result = item.TotalCharge;
            }

            return result;
        }
    }
}
