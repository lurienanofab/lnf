namespace LNF.Models.Billing
{
    public class BillingTypeItem : IBillingType
    {
        public int BillingTypeID { get; set; }
        public string BillingTypeName { get; set; }
        public bool IsActive { get; set; }

        /// <summary>
        /// The default BillingTypeID
        /// </summary>
        public static int Default => Regular;

        /// <summary>
        /// The BillingTypeID for Internal GaAs
        /// </summary>
        public static int Int_Ga => 1;

        /// <summary>
        /// The BillingTypeID for Internal Si
        /// </summary>
        public static int Int_Si => 2;

        /// <summary>
        /// The BillingTypeID for Internal Hour
        /// </summary>
        public static int Int_Hour => 3;

        /// <summary>
        /// The BillingTypeID for Internal Tools
        /// </summary>
        public static int Int_Tools => 4;

        /// <summary>
        /// The BillingTypeID for External Academic GaAs
        /// </summary>
        public static int ExtAc_Ga => 5;

        /// <summary>
        /// The BillingTypeID for External Academic Si
        /// </summary>
        public static int ExtAc_Si => 6;

        /// <summary>
        /// The BillingTypeID for External Academic Tools
        /// </summary>
        public static int ExtAc_Tools => 7;

        /// <summary>
        /// The BillingTypeID for External Academic Hour
        /// </summary>
        public static int ExtAc_Hour => 8;

        /// <summary>
        /// The BillingTypeID for Non Academic
        /// </summary>
        public static int NonAc => 9;

        /// <summary>
        /// The BillingTypeID for Non Academic Tools
        /// </summary>
        public static int NonAc_Tools => 10;

        /// <summary>
        /// The BillingTypeID for Non Academic Hour
        /// </summary>
        public static int NonAc_Hour => 11;

        /// <summary>
        /// The BillingTypeID for Regular
        /// </summary>
        public static int Regular => 12;

        /// <summary>
        /// The BillingTypeID for Grower/Observer
        /// </summary>
        public static int Grower_Observer => 13;

        /// <summary>
        /// The BillingTypeID for Remote
        /// </summary>
        public static int Remote => 14;

        /// <summary>
        /// The BillingTypeID for RegularException
        /// </summary>
        public static int RegularException => 15;

        /// <summary>
        /// The BillingTypeID for Other
        /// </summary>
        public static int Other => 99;
    }
}
