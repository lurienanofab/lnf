using LNF.Data;
using LNF.Repository.Billing;
using System.Data;

namespace LNF.Billing
{
    public static class LineCostUtility
    {
        public static void CalculateRoomLineCost(DataTable dt)
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
                IRoomBilling item = RoomBillingUtility.CreateRoomBillingFromDataRow(dr, false);
                
                dr.SetField("Room", RoomUtility.GetRoomDisplayName(item.RoomID));

                if (item.RoomCharge > 0)
                    dr.SetField("DailyFee", item.RoomCharge);

                if (item.EntryCharge > 0)
                    dr.SetField("EntryFee", item.EntryCharge);

                dr.SetField("LineCost", item.GetLineCost());
            }
        }

        public static void CalculateToolLineCost(DataTable dt)
        {
            if (!dt.Columns.Contains("LineCost"))
                dt.Columns.Add("LineCost", typeof(decimal));

            if (!dt.Columns.Contains("Room"))
                dt.Columns.Add("Room", typeof(string));

            //Part I: Get the true cost based on billing types
            foreach (DataRow dr in dt.Rows)
            {
                IToolBilling item = ToolBillingUtility.CreateToolBillingFromDataRow(dr, false);
                dr.SetField("Room", RoomUtility.GetRoomDisplayName(item.RoomID));
                dr.SetField("LineCost", item.GetLineCost());
            }
        }
    }
}
