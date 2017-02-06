using System;

namespace LNF.Repository.Ordering
{
    /// <summary>
    /// An item in the Store Manager Report output - combines IOF and Store data
    /// </summary>
    public class StoreManagerReportItem : IDataItem
    {
        /// <summary>
        /// The PO item id
        /// </summary>
        public virtual int ItemID { get; set; }

        /// <summary>
        /// The PO item description
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// The PO item vendor id
        /// </summary>
        public virtual int VendorID { get; set; }

        /// <summary>
        /// The PO item vendor name
        /// </summary>
        public virtual string VendorName { get; set; }

        /// <summary>
        /// The date the item was last ordered
        /// </summary>
        public virtual DateTime LastOrdered { get; set; }

        /// <summary>
        /// The PO item unit
        /// </summary>
        public virtual string Unit { get; set; }

        /// <summary>
        /// The PO item price
        /// </summary>
        public virtual double UnitPrice { get; set; }

        /// <summary>
        /// The store item id
        /// </summary>
        public virtual int StoreItemID { get; set; }

        /// <summary>
        /// The store item description
        /// </summary>
        public virtual string StoreDescription { get; set; }

        /// <summary>
        /// The store item package quantity
        /// </summary>
        public virtual int StorePackageQuantity { get; set; }

        /// <summary>
        /// The store item package price
        /// </summary>
        public virtual double StorePackagePrice { get; set; }

        /// <summary>
        /// The store item unit price
        /// </summary>
        public virtual double StoreUnitPrice { get; set; }
    }
}