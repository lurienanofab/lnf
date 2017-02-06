using LNF.Repository.Data;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Repository.Ordering
{
    /// <summary>
    /// An account that can be used to create purchase orders
    /// </summary>
    public class PurchaseOrderAccount : IDataItem
    {
        /// <summary>
        /// The AccountID of a PurchaseOrderAccount
        /// </summary>
        public virtual int AccountID { get; set; }

        /// <summary>
        /// The ClientID of a PurchaseOrderAccount
        /// </summary>
        public virtual int ClientID { get; set; }

        /// <summary>
        /// Indicates if the PurchaseOrderAccount is active
        /// </summary>
        public virtual bool Active { get; set; }

        /// <summary>
        /// Override of Equals for composite key
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns>True if the objects are equal, otherwise false</returns>
        public override bool Equals(object obj)
        {
            PurchaseOrderAccount item = obj as PurchaseOrderAccount;
            if (item == null) return false;
            return item.AccountID == AccountID && item.ClientID == ClientID;
        }

        /// <summary>
        /// Override of GetHashCode for composite key
        /// </summary>
        /// <returns>An integer hash code value</returns>
        public override int GetHashCode()
        {
            return (AccountID.ToString() + "|" + ClientID.ToString()).GetHashCode();
        }
    }
}
