using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Data;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Repository.Ordering;

namespace LNF.Ordering
{
    public static class PurchaserUtility
    {
        public static bool IsPurchaser(int clientId)
        {
            return DA.Current.Query<Purchaser>().Any(x => x.Client.ClientID == clientId && x.Active && !x.Deleted);
        }
    }
}
