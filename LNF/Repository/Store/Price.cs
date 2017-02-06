using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Repository.Store
{
    public class Price : IDataItem
    {
        public virtual int PriceID { get; set; }
        public virtual VendorPackage VendorPackage { get; set; }
        public virtual decimal PriceBreakQuantity { get; set; }
        public virtual decimal PackageCost { get; set; }
        public virtual decimal PackageMarkup { get; set; }
        public virtual decimal PackagePrice { get; set; }
        public virtual DateTime DateActive { get; set; }
    }
}
