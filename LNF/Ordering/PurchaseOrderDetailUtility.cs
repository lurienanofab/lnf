using LNF.Repository;
using LNF.Repository.Ordering;
using System;
using System.Collections.Generic;
using System.Data;

namespace LNF.Ordering
{
    public static class PurchaseOrderDetailUtility
    {
        public static PurchaseOrderDetail Add(PurchaseOrder po, PurchaseOrderItem item, PurchaseOrderCategory category, double qty, string unit, double unitPrice)
        {
            if (item.ItemID > 0)
            {
                bool isInventoryControlled = item.InventoryItemID != null;
                PurchaseOrderDetail pod = new PurchaseOrderDetail()
                {
                    Category = category,
                    IsInventoryControlled = isInventoryControlled,
                    Item = item,
                    PurchaseOrder = po,
                    Quantity = qty,
                    ToInventoryDate = null,
                    Unit = unit,
                    UnitPrice = unitPrice
                };
                DA.Current.Insert(pod);
                return pod;
            }

            throw new ArgumentException(string.Format("ItemID = {0} is not valid.", item.ItemID), "item");
        }

        public static PurchaseOrderDetail Update(int podid, PurchaseOrderCategory category, double quantity, string unit, double unitPrice)
        {
            PurchaseOrderDetail pod = DA.Current.Single<PurchaseOrderDetail>(podid);
            if (pod != null)
            {
                pod.Category = category;
                pod.Quantity = quantity;
                pod.Unit = unit;
                pod.UnitPrice = unitPrice;
            }
            return pod;
        }

        public static DataTable ToDataTable(this IEnumerable<PurchaseOrderDetail> query)
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

            foreach (PurchaseOrderDetail pod in query)
            {
                DataRow dr = dt.NewRow();
                dr["PODID"] = pod.PODID;
                dr["POID"] = pod.PurchaseOrder.POID;
                dr["ItemID"] = pod.Item.ItemID;
                dr["Quantity"] = pod.Quantity;
                dr["Unit"] = pod.Unit;
                dr["UnitPrice"] = pod.UnitPrice;
                dr["Description"] = pod.PurchaseOrder.Notes;
                dr["PartNum"] = pod.Item.PartNum;
                dr["CatID"] = pod.Category.CatID;
                dr["CatName"] = pod.Category.CatName;
                dr["ParentID"] = pod.Category.ParentID;
                dr["CatNo"] = pod.Category.CatNo;
                dr["CreatedDate"] = pod.PurchaseOrder.CreatedDate;
                dt.Rows.Add(dr);
            }

            return dt;
        }
    }
}
