using LNF.DataAccess;
using LNF.Store;
using System;

namespace LNF.Impl.Repository.Store
{
    public class Item : IItem, IDataItem
    {
        public virtual int ItemID { get; set; }
        public virtual string Description { get; set; }
        public virtual string Notes { get; set; }
        public virtual int CatID { get; set; }
        public virtual string ManufacturerPN { get; set; }
        public virtual bool Active { get; set; }
        public virtual bool StoreDisplay { get; set; }
        public virtual bool CrossCharge { get; set; }
        public virtual int StockQuantity { get; set; }
        public virtual int? MinStockQuantity { get; set; }
        public virtual int StockOnOrder { get; set; }
        public virtual DateTime? OrderDate { get; set; }
        public virtual DateTime? EstimatedArrivalDate { get; set; }
        public virtual string SearchKeyWords { get; set; }
        public virtual bool IsChemicalInventory { get; set; }
        public virtual int ChemicalInventoryCatID { get; set; }
        public virtual bool IsPopular { get; set; }
        public virtual int? MaxStockQuantity { get; set; }
    }
}
