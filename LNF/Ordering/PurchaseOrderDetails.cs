using System;
using System.Collections.Generic;
using System.Data;

namespace LNF.Ordering
{
    public static class PurchaseOrderDetails
    {
        public static IPurchaseOrderDetail Add(IPurchaseOrder po, IPurchaseOrderItem item, IPurchaseOrderCategory category, double qty, string unit, double unitPrice)
        {
            if (item.ItemID > 0)
            {
                bool isInventoryControlled = item.InventoryItemID != null;
                var pod = ServiceProvider.Current.Ordering.PurchaseOrder.AddDetail(po.POID, item.ItemID, category.CatID, qty, unit, unitPrice, isInventoryControlled);
                return pod;
            }

            throw new ArgumentException(string.Format("ItemID = {0} is not valid.", item.ItemID), "item");
        }

        public static IPurchaseOrderDetail Update(int podid, IPurchaseOrderCategory category, double qty, string unit, double unitPrice, bool isInventoryControlled)
        {
            return ServiceProvider.Current.Ordering.PurchaseOrder.UpdateDetail(podid, category.CatID, qty, unit, unitPrice, isInventoryControlled);
        }

        public static DataTable ToDataTable(this IEnumerable<IPurchaseOrderDetail> query)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("PODID", typeof(int));
            dt.Columns.Add("POID", typeof(int));
            dt.Columns.Add("ItemID", typeof(int));
            dt.Columns.Add("Quantity", typeof(double));
            dt.Columns.Add("Unit", typeof(string));
            dt.Columns.Add("UnitPrice", typeof(double));
            dt.Columns.Add("Description", typeof(string));
            dt.Columns.Add("PartNum", typeof(string));
            dt.Columns.Add("CatID", typeof(int));
            dt.Columns.Add("CatName", typeof(string));
            dt.Columns.Add("ParentID", typeof(int));
            dt.Columns.Add("CatNo", typeof(string));
            dt.Columns.Add("CreatedDate", typeof(DateTime));

            foreach (IPurchaseOrderDetail pod in query)
            {
                DataRow dr = dt.NewRow();
                dr["PODID"] = pod.PODID;
                dr["POID"] = pod.POID;
                dr["ItemID"] = pod.ItemID;
                dr["Quantity"] = pod.Quantity;
                dr["Unit"] = pod.Unit;
                dr["UnitPrice"] = pod.UnitPrice;
                dr["Description"] = pod.Notes;
                dr["PartNum"] = pod.PartNum;
                dr["CatID"] = pod.CatID;
                dr["CatName"] = pod.CatName;
                dr["ParentID"] = pod.ParentID;
                dr["CatNo"] = pod.CatNo;
                dr["CreatedDate"] = pod.CreatedDate;
                dt.Rows.Add(dr);
            }

            return dt;
        }
    }
}
