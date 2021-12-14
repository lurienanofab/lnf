using System;

namespace LNF.Billing.Reports.ServiceUnitBilling
{
    public static class ReportSettings
    {
        public static readonly DateTime July2009 = new DateTime(2009, 7, 1);
        public static readonly DateTime July2010 = new DateTime(2010, 7, 1);

        public static string CompanyName => GlobalSettings.Current.CompanyName;
        public static string FinancialManagerUserName => GlobalSettings.Current.FinancialManagerUserName;

        public static int GetJournalUnitNumber(DateTime period, BillingCategory billingCategory, JournalUnitTypes juType)
        {
            int yearOff = period.Year - July2010.Year;
            int monthOff = period.Month - July2010.Month;

            int increment = (yearOff * 12 + monthOff) * 6;

            //263 is the starting number for room sub in July 2010
            if (billingCategory == BillingCategory.Room && juType == JournalUnitTypes.A)
                return 100 + increment;
            else if (billingCategory == BillingCategory.Room && juType == JournalUnitTypes.B)
                return 100 + increment + 1;
            else if (billingCategory == BillingCategory.Room && juType == JournalUnitTypes.C)
                return 100 + increment + 2;
            else if (billingCategory == BillingCategory.Tool && juType == JournalUnitTypes.A)
                return 100 + increment + 3;
            else if (billingCategory == BillingCategory.Tool && juType == JournalUnitTypes.B)
                return 100 + increment + 4;
            else if (billingCategory == BillingCategory.Tool && juType == JournalUnitTypes.C)
                return 100 + increment + 5;
            else
                throw new ArgumentException("Invalid arguments passed to LNF.Billing.Reports.ServiceUnitBilling.ReportSettings.GetJournalUnitNumber");
        }

        public static int GetServiceUnitBillingNumber(DateTime period, BillingCategory billingCategory)
        {
            int yearoff = period.Year - July2010.Year;
            int monthoff = period.Month - July2010.Month;

            int increment = (yearoff * 12 + monthoff) * 3;

            //263 is the starting number for room sub in July 2010
            if (billingCategory == BillingCategory.Tool)
                return 263 + increment + 1;
            else if (billingCategory == BillingCategory.Store)
                return 263 + increment + 2;
            else
                return 263 + increment;
        }
    }
}
