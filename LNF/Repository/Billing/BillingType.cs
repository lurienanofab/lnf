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

            var item = obj as BillingType;

            if (item == null) return false;

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
        public static int Default
        {
            get { return Regular; }
        }

        /// <summary>
        /// The BillingTypeID for Internal GaAs
        /// </summary>
        public static int Int_Ga
        {
            get { return 1; }
        }

        /// <summary>
        /// The BillingTypeID for Internal Si
        /// </summary>
        public static int Int_Si
        {
            get { return 2; }
        }

        /// <summary>
        /// The BillingTypeID for Internal Hour
        /// </summary>
        public static int Int_Hour
        {
            get { return 3; }
        }

        /// <summary>
        /// The BillingTypeID for Internal Tools
        /// </summary>
        public static int Int_Tools
        {
            get { return 4; }
        }

        /// <summary>
        /// The BillingTypeID for External Academic GaAs
        /// </summary>
        public static int ExtAc_Ga
        {
            get { return 5; }
        }

        /// <summary>
        /// The BillingTypeID for External Academic Si
        /// </summary>
        public static int ExtAc_Si
        {
            get { return 6; }
        }

        /// <summary>
        /// The BillingTypeID for External Academic Tools
        /// </summary>
        public static int ExtAc_Tools
        {
            get { return 7; }
        }

        /// <summary>
        /// The BillingTypeID for External Academic Hour
        /// </summary>
        public static int ExtAc_Hour
        {
            get { return 8; }
        }

        /// <summary>
        /// The BillingTypeID for Non Academic
        /// </summary>
        public static int NonAc
        {
            get { return 9; }
        }

        /// <summary>
        /// The BillingTypeID for Non Academic Tools
        /// </summary>
        public static int NonAc_Tools
        {
            get { return 10; }
        }

        /// <summary>
        /// The BillingTypeID for Non Academic Hour
        /// </summary>
        public static int NonAc_Hour
        {
            get { return 11; }
        }

        /// <summary>
        /// The BillingTypeID for Regular
        /// </summary>
        public static int Regular
        {
            get { return 12; }
        }

        /// <summary>
        /// The BillingTypeID for Grower/Observer
        /// </summary>
        public static int Grower_Observer
        {
            get { return 13; }
        }

        /// <summary>
        /// The BillingTypeID for Remote
        /// </summary>
        public static int Remote
        {
            get { return 14; }
        }

        /// <summary>
        /// The BillingTypeID for RegularException
        /// </summary>
        public static int RegularException
        {
            get { return 15; }
        }

        /// <summary>
        /// The BillingTypeID for Other
        /// </summary>
        public static int Other
        {
            get { return 99; }
        }
    }
}
