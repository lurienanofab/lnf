﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Repository.Store
{
    public class ItemPriceHistory : IDataItem
    {
        public virtual int ItemID { get; set; }
        public virtual int PackageID { get; set; }
        public virtual int VendorPackageID { get; set; }
        public virtual int PriceID { get; set; }
        public virtual DateTime DateActive { get; set; }
        public virtual int BaseQMultiplier { get; set; }
        public virtual double PackageCost { get; set; }
        public virtual double PackageMarkup { get; set; }
        public virtual double PackagePrice { get; set; }
        public virtual double UnitCost { get; set; }
        public virtual double UnitPrice { get; set; }
    }
}
