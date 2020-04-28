using LNF.Billing;
using LNF.DataAccess;

namespace LNF.Impl.Repository.Billing
{
    /// <summary>
    /// Represents different types of usage charges
    /// </summary>
    public class BillingType : IBillingType, IDataItem
    {
        /// <summary>
        /// The unique id of a BillingType
        /// </summary>
        public virtual int BillingTypeID { get; set; }

        /// <summary>
        /// The name of a BillingType
        /// </summary>
        public virtual string BillingTypeName { get; set; }

        /// <summary>
        /// Indicates if a BillingType is currently active
        /// </summary>
        public virtual bool IsActive { get; set; }

        /// <summary>
        /// Determines whether the specified object is equal to the current object
        /// </summary>
        /// <param name="obj">The object to compare with the current object</param>
        /// <returns>True if the specified object is equal to the current object, otherwise false</returns>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is BillingType item)) return false;
            return item.BillingTypeID == BillingTypeID;
        }

        /// <summary>
        /// Serves as the default hash function
        /// </summary>
        /// <returns>A hash code for the current object</returns>
        public override int GetHashCode()
        {
            return BillingTypeID.GetHashCode();
        }
    }
}
