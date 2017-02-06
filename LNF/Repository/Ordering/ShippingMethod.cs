using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Repository.Ordering
{
    public class ShippingMethod : IDataItem
    {
        public virtual int ShippingMethodID { get; set; }
        public virtual string ShippingMethodName { get; set; }

        public static IQueryable<ShippingMethod> SelectAll()
        {
            return DA.Current.Query<ShippingMethod>();
        }
    }
}
