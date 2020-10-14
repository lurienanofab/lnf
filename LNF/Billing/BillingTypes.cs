using LNF.Cache;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Billing
{
    public static class BillingTypes
    {
        public static IEnumerable<IBillingType> All() => CacheManager.Current.GetValue("BillingTypes", p => p.Billing.BillingType.GetBillingTypes(), DateTimeOffset.Now.AddDays(7));

        public static readonly int Int_Ga = 1;
        public static readonly int Int_Si = 2;
        public static readonly int Int_Hour = 3;
        public static readonly int Int_Tools = 4;
        public static readonly int ExtAc_Ga = 5;
        public static readonly int ExtAc_Si = 6;
        public static readonly int ExtAc_Tools = 7;
        public static readonly int ExtAc_Hour = 8;
        public static readonly int NonAc = 9;
        public static readonly int NonAc_Tools = 10;
        public static readonly int NonAc_Hour = 11;
        public static readonly int Regular = 12;
        public static readonly int Grower_Observer = 13;
        public static readonly int Remote = 14;
        public static readonly int RegularException = 15;
        public static readonly int Other = 99;

        public static class Instance
        {
            public static IBillingType Default => Regular;
            public static IBillingType Int_Ga => Find(BillingTypes.Int_Ga);
            public static IBillingType Int_Si => Find(BillingTypes.Int_Si);
            public static IBillingType Int_Hour => Find(BillingTypes.Int_Hour);
            public static IBillingType Int_Tools => Find(BillingTypes.Int_Tools);
            public static IBillingType ExtAc_Ga => Find(BillingTypes.ExtAc_Ga);
            public static IBillingType ExtAc_Si => Find(BillingTypes.ExtAc_Si);
            public static IBillingType ExtAc_Tools => Find(BillingTypes.ExtAc_Tools);
            public static IBillingType ExtAc_Hour => Find(BillingTypes.ExtAc_Hour);
            public static IBillingType NonAc => Find(BillingTypes.NonAc);
            public static IBillingType NonAc_Tools => Find(BillingTypes.NonAc_Tools);
            public static IBillingType NonAc_Hour => Find(BillingTypes.NonAc_Hour);
            public static IBillingType Regular => Find(BillingTypes.Regular);
            public static IBillingType Grower_Observer => Find(BillingTypes.Grower_Observer);
            public static IBillingType Remote => Find(BillingTypes.Remote);
            public static IBillingType RegularException => Find(BillingTypes.RegularException);
            public static IBillingType Other => Find(BillingTypes.Other);
        }

        public static string GetBillingTypeName(IBillingType billingType)
        {
            if (billingType == Instance.ExtAc_Ga)
                return "External Academic GaAs";
            else if (billingType == Instance.ExtAc_Hour)
                return "External Academic Hour";
            else if (billingType == Instance.ExtAc_Si)
                return "External Academic Si";
            else if (billingType == Instance.ExtAc_Tools)
                return "External Academic Tools";
            else if (billingType == Instance.Int_Ga)
                return "Internal Academic GaAs";
            else if (billingType == Instance.Int_Hour)
                return "Internal Academic Hour";
            else if (billingType == Instance.Int_Si)
                return "Internal Academic Si";
            else if (billingType == Instance.Int_Tools)
                return "Internal Academic Tools";
            else if (billingType == Instance.NonAc)
                return "Non Academic";
            else if (billingType == Instance.NonAc_Hour)
                return "Non Academic Hour";
            else
                return "Other";
        }

        public static IBillingType GetBillingType(string text)
        {
            switch (text)
            {
                case "External Academic GaAs":
                    return Instance.ExtAc_Ga;
                case "External Academic Hour":
                    return Instance.ExtAc_Hour;
                case "External Academic Si":
                    return Instance.ExtAc_Si;
                case "External Academic Tools":
                    return Instance.ExtAc_Tools;
                case "Internal Academic GaAs":
                    return Instance.Int_Ga;
                case "Internal Academic Hour":
                    return Instance.Int_Hour;
                case "Internal Academic Si":
                    return Instance.Int_Si;
                case "Internal Academic Tools":
                    return Instance.Int_Tools;
                case "Non Academic":
                    return Instance.NonAc;
                case "Non Academic Hour":
                    return Instance.NonAc_Hour;
                default:
                    return Instance.Other;
            }
        }

        public static bool IsMonthlyUserBillingType(int billingTypeId)
        {
            // BillingType.Int_Ga || BillingType.Int_Si || BillingType.ExtAc_Ga || BillingType.ExtAc_Si
            bool result = new[] { Int_Ga, Int_Si, ExtAc_Ga, ExtAc_Si }.Contains(billingTypeId);
            return result;
        }

        public static bool IsGrowerUserBillingType(int billingTypeId)
        {
            // BillingType.Int_Tools || BillingType.ExtAc_Tools || BillingType.NonAc_Tools
            bool result = new[] { Int_Tools, ExtAc_Tools, NonAc_Tools }.Contains(billingTypeId);
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
