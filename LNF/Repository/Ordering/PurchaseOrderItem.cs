namespace LNF.Repository.Ordering
{
    public class PurchaseOrderItem : IDataItem
    {
        public virtual int ItemID { get; set; }
        public virtual Vendor Vendor { get; set; }
        public virtual string Description { get; set; }
        public virtual string PartNum { get; set; }
        public virtual double UnitPrice { get; set; }
        public virtual bool Active { get; set; }
        public virtual int? InventoryItemID { get; set; }

        public static string CleanString(string value)
        {
            // Converts to lower case and strips spaces.
            if (string.IsNullOrEmpty(value))
                return string.Empty;
            else
                return value.ToLower().Replace(" ", string.Empty);
        }
    }
}
