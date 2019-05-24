using LNF.Billing;
using LNF.CommonTools;
using LNF.Models.Billing;
using System;
using System.Data;
using System.Linq;

namespace LNF.Impl.Billing.Report
{
    public static class ReportUtility
    {
        public static IBillingTypeManager BillingTypeManager => ServiceProvider.Current.Billing.BillingType;

        [Obsolete("Use LNF.CommonTools.LineCostUtility.CalculateToolLineCost instead.")]
        public static void ApplyToolFormula(DataTable dt, DateTime startPeriod, DateTime endPeriod)
        {
            if (!dt.Columns.Contains("LineCost"))
                dt.Columns.Add("LineCost", typeof(decimal));

            int billingTypeId;
            int roomId = 0;
            bool isStarted = true;

            foreach (DataRow dr in dt.Rows)
            {
                billingTypeId = dr.Field<int>("BillingTypeID");
                roomId = dr.Field<int>("RoomID");
                isStarted = dr.Field<bool>("IsStarted");

                if (BillingTypeManager.IsMonthlyUserBillingType(billingTypeId))
                {
                    //Monthly User, charge mask maker for everyone
                    if (roomId == 6)
                    {
                        if (dr.Field<int>("ResourceID") == 56000)
                        {
                            if (isStarted)
                                dr["LineCost"] = dr.Field<decimal>("UsageFeeCharged") + dr.Field<decimal>("OverTimePenaltyFee") + (dr.Field<decimal>("ResourceRate") == 0 ? 0 : dr.Field<decimal>("ReservationFee2"));
                            else
                                dr["LineCost"] = dr.Field<decimal>("UncancelledPenaltyFee") + dr.Field<decimal>("ReservationFee2");
                        }
                        else
                        {
                            dr["LineCost"] = 0;
                        }
                    }
                    else
                    {
                        //non clean room tools are always charged for usage fee
                        if (isStarted)
                            dr["LineCost"] = dr.Field<decimal>("UsageFeeCharged") + dr.Field<decimal>("OverTimePenaltyFee") + (dr.Field<decimal>("ResourceRate") == 0 ? 0 : dr.Field<decimal>("ReservationFee2"));
                        else
                            dr["LineCost"] = dr.Field<decimal>("UncancelledPenaltyFee") + dr.Field<decimal>("ReservationFee2");
                    }
                }

                //Per Use types
                if (startPeriod >= new DateTime(2010, 7, 1))
                {
                    //dr("LineCost") = dr("UsageFee") + dr("OverTimePenaltyFee") + dr("UncancelledPenaltyFee") + dr("ReservationFee2")

                    //2011-05 New tool billing started on 2011-04
                    if (startPeriod >= new DateTime(2011, 4, 1))
                    {
                        if (!dr.Field<bool>("IsCancelledBeforeAllowedTime"))
                            dr["LineCost"] = dr.Field<decimal>("UsageFeeCharged") + dr.Field<decimal>("OverTimePenaltyFee") + dr.Field<decimal>("BookingFee"); //IIf(dr("ResourceRate") = 0, 0, dr("ReservationFee"))
                        else
                            dr["LineCost"] = dr.Field<decimal>("BookingFee"); //Cancelled before two hours
                    }
                    else
                    {
                        if (isStarted)
                            dr["LineCost"] = dr.Field<decimal>("UsageFeeCharged") +dr.Field<decimal>("OverTimePenaltyFee") + (dr.Field<decimal>("ResourceRate") == 0 ? 0 : dr.Field<decimal>("ReservationFee2"));
                        else
                            dr["LineCost"] = dr.Field<decimal>("UncancelledPenaltyFee"); //+ dr("ReservationFee")
                    }

                    //If BillingTypeID = BillingType.Remote Then
                    //    '2010-12-06 Sandrine doesn't want Remote to be shown
                    //    'dr("LineCost") = 0
                    //End If
                }
                else
                {
                    if (isStarted)
                        dr["LineCost"] = dr.Field<decimal>("UsageFeeCharged") + dr.Field<decimal>("OverTimePenaltyFee") + (dr.Field<decimal>("ResourceRate") == 0 ? 0 : dr.Field<decimal>("ReservationFee2"));
                    else
                        dr["LineCost"] = dr.Field<decimal>("UncancelledPenaltyFee") + dr.Field<decimal>("ReservationFee2");
                }

                //If isStarted Then
                //    dr("LineCost") = dr("UsageFee") + dr("OverTimePenaltyFee") + dr("ReservationFee2")
                //Else
                //    dr("LineCost") = dr("UncancelledPenaltyFee") + dr("ReservationFee2")
                //End If

                //if resource rate is 0 , everything must be 0 (what about per use charge)
                if (dr.Field<decimal>("ResourceRate") == 0)
                    dr["LineCost"] = 0;
            }
        }

        [Obsolete("Use LNF.CommonTools.LineCostUtility.CalculateRoomLineCost instead.")]
        public static void ApplyRoomFormula(DataTable dt)
        {
            if (!dt.Columns.Contains("LineCost"))
                dt.Columns.Add("LineCost", typeof(double));

            int billingTypeId;
            int roomId = 0;

            foreach (DataRow dr in dt.Rows)
            {
                billingTypeId = dr.Field<int>("BillingTypeID");
                roomId = dr.Field<int>("RoomID");

                //1. Find out all Monthly type users and apply to Clean room
                if (BillingTypeManager.IsMonthlyUserBillingType(billingTypeId))
                {
                    if (roomId == 6)
                        dr["LineCost"] = dr.Field<decimal>("MonthlyRoomCharge");
                    else
                        dr["LineCost"] = dr.Field<decimal>("RoomCharge") + dr.Field<decimal>("EntryCharge");
                }
                //2. The growers are charged with room fee only when they reserve and activate a tool
                else if (BillingTypeManager.IsGrowerUserBillingType(billingTypeId))
                {
                    if (roomId == 4)
                        dr["LineCost"] = dr.Field<decimal>("RoomCharge"); //Organics bay must be charged for growers as well
                    else
                        dr["LineCost"] = (dr.Field<decimal>("AccountDays") * dr.Field<decimal>("RoomRate")) + dr.Field<decimal>("EntryCharge");

                }
                else if (billingTypeId == BillingTypeManager.Other.BillingTypeID)
                {
                    dr["LineCost"] = 0;
                }
                else
                {
                    //Per Use types
                    dr["LineCost"] = dr.Field<decimal>("RoomCharge") + dr.Field<decimal>("EntryCharge");
                }
            }
        }

        public static void ApplyFilter(DataTable dtClientAccount, BillingCategory billingCategory)
        {
            string filter;

            switch (billingCategory)
            {
                case BillingCategory.Store:
                    //The only filter is 943777 account should be removed because it's also a credit account, but it's done in the main code already
                    filter = "ShortCode = 943777";
                    break;
                default:
                    filter = string.Empty;
                    break;
            }

            int accountId = 0;

            DataRow[] query = DataAccess.AccountSelect(new { Action = "GetAllNonBillingAccounts" }).Select(filter);

            foreach (DataRow dr in dtClientAccount.Rows)
            {
                accountId = dr.Field<int>("AccountID");
                DataRow acct = query.FirstOrDefault(x => x.Field<int>("AccountID") == accountId);
                if (acct != null) dr.Delete();
            }
        }

        public static void ApplyMiscCharge(DataTable dt, DataTable dtClientAccount, DateTime startPeriod, DateTime endPeriod, BillingCategory billingCategory, int clientId)
        {
            int primaryOrgId = 17;

            string billingCategoryName = Enum.GetName(typeof(BillingCategory), billingCategory);

            object queryParameters;
            
            if (clientId == 0)
                queryParameters = new { Action = "GetAllByPeriodAndSUBTypeAndOrgID", StartPeriod = startPeriod, EndPeriod = endPeriod, SUBType = billingCategoryName, OrgID = primaryOrgId };
            else
                queryParameters = new { Action = "GetAllByPeriodAndSUBTypeAndOrgID", StartPeriod = startPeriod, EndPeriod = endPeriod, SUBType = billingCategoryName, OrgID = primaryOrgId, ClientID = clientId };

            DataTable dtMisc = DataAccess.MiscBillingChargeSelect(queryParameters);

            DataRow ndr;
            DataRow[] rows;
            foreach (DataRow dr in dtMisc.Rows)
            {
                ndr = dt.NewRow();

                rows = dt.Select(string.Format("Period = '{0}' AND ClientID = {1} AND AccountID = {2}", dr["Period"], dr["ClientID"], dr["AccountID"]));

                if (rows.Length == 0)
                {
                    //This client and account has not been used in this month, so we have to add the entry to ClientAccountData
                    DataRow[] tempRows = dtClientAccount.Select(string.Format("Period = '{0}' AND ClientID = {1} AND AccountID = {2}", dr["Period"], dr["ClientID"], dr["AccountID"]));
                    if (tempRows.Length == 0)
                    {
                        DataRow ndrClientAccount = dtClientAccount.NewRow();
                        ndrClientAccount["Period"] = dr["Period"];
                        ndrClientAccount["ClientID"] = dr["ClientID"];
                        ndrClientAccount["AccountID"] = dr["AccountID"];
                        dtClientAccount.Rows.Add(ndrClientAccount);
                    }
                }

                ndr["Period"] = dr["Period"];
                ndr["ClientID"] = dr["ClientID"];
                ndr["AccountID"] = dr["AccountID"];
                ndr["DisplayName"] = dr["DisplayName"];
                ndr["UserName"] = dr["UserName"];

                if (dt.Columns.Contains("BillingTypeName"))
                    ndr["BillingTypeName"] = "Miscellaneous";

                ndr["Number"] = dr["Number"];
                ndr["ShortCode"] = dr["ShortCode"];
                ndr["LineCost"] = Utility.ConvertTo(dr["Quantity"], 0.0) * Utility.ConvertTo(dr["UnitCost"], 0.0);

                //it's possible store charge would come here
                try
                {
                    ndr["SubsidyDiscount"] = dr["SubsidyDiscount"];
                }
                catch { }

                dt.Rows.Add(ndr);
            }
        }

        public static string ClipText(string text, int length)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            return (text.Length > length) ? text.Substring(0, length) : text;
        }

        public static T StringToEnum<T>(string value)
        {
            if (typeof(T).IsEnum)
                return (T)Enum.Parse(typeof(T), value, true);
            else
                throw new ArgumentException("T must be an enum type");
        }

        public static string EnumToString<T>(T value)
        {
            if (typeof(T).IsEnum)
                return Enum.GetName(typeof(T), value);
            else
                throw new ArgumentException("T must be an enum type");
        }
    }
}