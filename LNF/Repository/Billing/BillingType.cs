using LNF.Models.Billing;

namespace LNF.Repository.Billing
{
    /// <summary>
    /// Represents different types of usage charges
    /// </summary>
    public class BillingType : IDataItem
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

        /// <summary>
        /// The default BillingTypeID
        /// </summary>
        public static int Default => BillingTypeItem.Default;

        /// <summary>
        /// The BillingTypeID for Internal GaAs
        /// </summary>
        public static int Int_Ga => BillingTypeItem.Int_Ga;

        /// <summary>
        /// The BillingTypeID for Internal Si
        /// </summary>
        public static int Int_Si => BillingTypeItem.Int_Si;

        /// <summary>
        /// The BillingTypeID for Internal Hour
        /// </summary>
        public static int Int_Hour => BillingTypeItem.Int_Hour;

        /// <summary>
        /// The BillingTypeID for Internal Tools
        /// </summary>
        public static int Int_Tools => BillingTypeItem.Int_Tools;

        /// <summary>
        /// The BillingTypeID for External Academic GaAs
        /// </summary>
        public static int ExtAc_Ga => BillingTypeItem.ExtAc_Ga;

        /// <summary>
        /// The BillingTypeID for External Academic Si
        /// </summary>
        public static int ExtAc_Si => BillingTypeItem.ExtAc_Si;

        /// <summary>
        /// The BillingTypeID for External Academic Tools
        /// </summary>
        public static int ExtAc_Tools => BillingTypeItem.ExtAc_Tools;

        /// <summary>
        /// The BillingTypeID for External Academic Hour
        /// </summary>
        public static int ExtAc_Hour => BillingTypeItem.ExtAc_Hour;

        /// <summary>
        /// The BillingTypeID for Non Academic
        /// </summary>
        public static int NonAc => BillingTypeItem.NonAc;

        /// <summary>
        /// The BillingTypeID for Non Academic Tools
        /// </summary>
        public static int NonAc_Tools => BillingTypeItem.NonAc_Tools;

        /// <summary>
        /// The BillingTypeID for Non Academic Hour
        /// </summary>
        public static int NonAc_Hour => BillingTypeItem.NonAc_Hour;

        /// <summary>
        /// The BillingTypeID for Regular
        /// </summary>
        public static int Regular => BillingTypeItem.Regular;

        /// <summary>
        /// The BillingTypeID for Grower/Observer
        /// </summary>
        public static int Grower_Observer => BillingTypeItem.Grower_Observer;

        /// <summary>
        /// The BillingTypeID for Remote
        /// </summary>
        public static int Remote => BillingTypeItem.Remote;

        /// <summary>
        /// The BillingTypeID for RegularException
        /// </summary>
        public static int RegularException => BillingTypeItem.RegularException;

        /// <summary>
        /// The BillingTypeID for Other
        /// </summary>
        public static int Other => BillingTypeItem.Other;
    }
}
