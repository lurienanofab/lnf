using LNF.Cache;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Billing
{
    public static class BillingTypes
    {
        public static IEnumerable<IBillingType> All() => CacheManager.Current.GetValue("BillingTypes", p => p.Billing.BillingType.GetBillingTypes(), DateTimeOffset.Now.AddDays(7));

        public static IBillingType Default => Regular;
        public static IBillingType Int_Ga => Find(1);
        public static IBillingType Int_Si => Find(2);
        public static IBillingType Int_Hour => Find(3);
        public static IBillingType Int_Tools => Find(4);
        public static IBillingType ExtAc_Ga => Find(5);
        public static IBillingType ExtAc_Si => Find(6);
        public static IBillingType ExtAc_Tools => Find(7);
        public static IBillingType ExtAc_Hour => Find(8);
        public static IBillingType NonAc => Find(9);
        public static IBillingType NonAc_Tools => Find(10);
        public static IBillingType NonAc_Hour => Find(11);
        public static IBillingType Regular => Find(12);
        public static IBillingType Grower_Observer => Find(13);
        public static IBillingType Remote => Find(14);
        public static IBillingType RegularException => Find(15);
        public static IBillingType Other => Find(99);

        public static string GetBillingTypeName(IBillingType billingType)
        {
            if (billingType == ExtAc_Ga)
                return "External Academic GaAs";
            else if (billingType == ExtAc_Hour)
                return "External Academic Hour";
            else if (billingType == ExtAc_Si)
                return "External Academic Si";
            else if (billingType == ExtAc_Tools)
                return "External Academic Tools";
            else if (billingType == Int_Ga)
                return "Internal Academic GaAs";
            else if (billingType == Int_Hour)
                return "Internal Academic Hour";
            else if (billingType == Int_Si)
                return "Internal Academic Si";
            else if (billingType == Int_Tools)
                return "Internal Academic Tools";
            else if (billingType == NonAc)
                return "Non Academic";
            else if (billingType == NonAc_Hour)
                return "Non Academic Hour";
            else
                return "Other";
        }

        public static IBillingType GetBillingType(string text)
        {
            switch (text)
            {
                case "External Academic GaAs":
                    return ExtAc_Ga;
                case "External Academic Hour":
                    return ExtAc_Hour;
                case "External Academic Si":
                    return ExtAc_Si;
                case "External Academic Tools":
                    return ExtAc_Tools;
                case "Internal Academic GaAs":
                    return Int_Ga;
                case "Internal Academic Hour":
                    return Int_Hour;
                case "Internal Academic Si":
                    return Int_Si;
                case "Internal Academic Tools":
                    return Int_Tools;
                case "Non Academic":
                    return NonAc;
                case "Non Academic Hour":
                    return NonAc_Hour;
                default:
                    return Other;
            }
        }

        public static bool IsMonthlyUserBillingType(int billingTypeId)
        {
            // BillingType.Int_Ga || BillingType.Int_Si || BillingType.ExtAc_Ga || BillingType.ExtAc_Si
            bool result = new[] { Int_Ga.BillingTypeID, Int_Si.BillingTypeID, ExtAc_Ga.BillingTypeID, ExtAc_Si.BillingTypeID }.Contains(billingTypeId);
            return result;
        }

        public static bool IsGrowerUserBillingType(int billingTypeId)
        {
            // BillingType.Int_Tools || BillingType.ExtAc_Tools || BillingType.NonAc_Tools
            bool result = new[] { Int_Tools.BillingTypeID, ExtAc_Tools.BillingTypeID, NonAc_Tools.BillingTypeID }.Contains(billingTypeId);
            return result;
        }

        public static IBillingType Find(int billingTypeId)
        {
            return All().FirstOrDefault(x => x.BillingTypeID == billingTypeId);
        }

        public static IBillingType Find(string name)
        {
            return All().FirstOrDefault(x => x.BillingTypeName == name);
        }
    }
}
