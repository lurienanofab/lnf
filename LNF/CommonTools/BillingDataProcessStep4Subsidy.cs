using LNF.Billing;
using LNF.Logging;
using LNF.Repository;
using LNF.Repository.Billing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.CommonTools
{
    //2010-03-23 Subsidy billing data process - run after the 3rd day of business every month
    public class BillingDataProcessStep4Subsidy
    {
        public static void PopulateSubsidyBilling(DateTime period, int clientId = 0)
        {
            int rowsSelectedFromSource = 0;
            int rowsDeletedFromTieredSubsidyBilling = 0;
            int rowsInsertedIntoTieredSubsidyBilling = 0;

            using (var timer = LogTaskTimer.Start("BillingDataProcessStep4Subsidy.PopulateSubsidyBilling", "period = '{0:yyyy-MM-dd}', clientId = {1}, totalOutRows = {2}, rowsDeletedFromTieredSubsidyBilling = {3}", () => new object[] { period, clientId, rowsSelectedFromSource, rowsDeletedFromTieredSubsidyBilling, rowsInsertedIntoTieredSubsidyBilling }))
            {
                DataSet ds = GetNecessaryTables(period, clientId);
                DataTable dtRoom = ds.Tables[0];
                DataTable dtTool = ds.Tables[1];
                DataTable dtTiers = ds.Tables[2];
                DataTable dtOut = ds.Tables[3];
                DataTable dtMiscCharges = ds.Tables[4];

                //The strategy here is we loop through dtRoom where all users in current month should have data in TieredSubsidyBilling Table
                //Then we loop through dtTool in case if someone had used the tool, but not the room (unlikely, but possible)
                //Then we calculate the user payment 

                //The code below will populate two columns "RoomSum" and "RoomMiscSum"
                DataRow newrow;
                foreach (DataRow dr in dtRoom.Rows)
                {
                    newrow = dtOut.NewRow();

                    newrow["Period"] = dr["Period"];
                    newrow["ClientID"] = dr["ClientID"];
                    newrow["OrgID"] = dr["OrgID"];

                    newrow["RoomSum"] = dr["TotalCharge"];
                    newrow["RoomMiscSum"] = GetMiscSumPerClient(dtMiscCharges, dr.Field<int>("ClientID"), "Room");

                    //We still need to set the tool for people who don't use tool (but only room)
                    newrow["ToolSum"] = 0;
                    newrow["ToolMiscSum"] = 0;

                    //The column does not allow NULL
                    newrow["UserPaymentSum"] = 0;

                    dtOut.Rows.Add(newrow);
                }

                foreach (DataRow dr in dtTool.Rows)
                {
                    DataRow[] drs = dtOut.Select(string.Format("ClientID = {0} AND OrgID = {1}", dr["ClientID"], dr["OrgID"]));

                    if (drs.Length == 1)
                    {
                        //we found data record for room, so we use the same data record
                        drs[0]["ToolSum"] = dr["TotalCharge"];
                        drs[0]["ToolMiscSum"] = GetMiscSumPerClient(dtMiscCharges, dr.Field<int>("ClientID"), "Tool");
                    }
                    else if (drs.Length == 0)
                    {
                        //This user uses tool, but not room, so it's a whole new record
                        newrow = dtOut.NewRow();

                        newrow["Period"] = dr["Period"];
                        newrow["ClientID"] = dr["ClientID"];
                        newrow["OrgID"] = dr["OrgID"];

                        newrow["RoomSum"] = 0;
                        newrow["RoomMiscSum"] = 0;

                        newrow["ToolSum"] = dr["TotalCharge"];
                        newrow["ToolMiscSum"] = GetMiscSumPerClient(dtMiscCharges, dr.Field<int>("ClientID"), "Tool");

                        //The column does not allow NULL
                        newrow["UserPaymentSum"] = 0;

                        dtOut.Rows.Add(newrow);
                    }
                    else
                    {
                        //error, it's impossible to have more than 1 data
                        Providers.Email.SendMessage(0, "LNF.CommonTools.BillingDataProcessStep4Subsidy.PopulateSubsidyBilling(DateTime period, int clientId = 0)", "Error in populating TieredSubsidyBilling", string.Format("There is more than one row for client {0}", drs[0]["ClientID"]), SendEmail.SystemEmail, SendEmail.DeveloperEmails);
                    }
                }

                foreach (DataRow dr in dtMiscCharges.Rows)
                {
                    DataRow[] drs = dtOut.Select(string.Format("ClientID = {0}", dr["ClientID"]));

                    if (drs.Length == 0)
                    {
                        //This user has no tool nor room, so it's a whole new record
                        newrow = dtOut.NewRow();

                        newrow["Period"] = period;
                        newrow["ClientID"] = dr["ClientID"];
                        newrow["OrgID"] = 17; //always internal

                        newrow["RoomSum"] = 0;
                        newrow["RoomMiscSum"] = GetMiscSumPerClient(dtMiscCharges, Convert.ToInt32(dr["ClientID"]), "Room");

                        newrow["ToolSum"] = 0;
                        newrow["ToolMiscSum"] = GetMiscSumPerClient(dtMiscCharges, Convert.ToInt32(dr["ClientID"]), "Tool");

                        //The column does not allow NULL
                        newrow["UserPaymentSum"] = 0;

                        dtOut.Rows.Add(newrow);
                    }
                }

                rowsSelectedFromSource = dtOut.Rows.Count;

                //At this point, the TiredSubsidyBilling table should have all the records but some fields are missing.  
                //So we loop through the newly constructed table and fill out the missing fields
                PopulateFieldsFromHistory(period, dtOut, clientId);

                //Clean up the data before saving
                rowsDeletedFromTieredSubsidyBilling = DeleteTieredSubsidyBilling(period, clientId);

                //Save everything back to the main table
                rowsInsertedIntoTieredSubsidyBilling = SaveNewTieredSubsidyBilling(dtOut);

                //Calculate the real subsidy amount and populate the details, UserPaymentSum is set in this method.
                CalculateSubsidyFee(period, clientId);

                //Push changes back to billing tables
                DistributeSubsidyMoneyEvenly(period, clientId);

                //[2015-10-20 jg] Added account subsidy feature per Sandrine. Some accounts get a fixed subsidy that overrides the tiered subsidy
                ApplyAccountSubsidy(period);
            }
        }

        private static void ApplyAccountSubsidy(DateTime period)
        {
            var accountSubsidy = AccountSubsidyUtility.GetActive(period, period.AddMonths(1));
            foreach (var item in accountSubsidy)
                item.ApplyToBilling(period);
        }

        public static decimal GetSubsidyDiscountPercentage(DateTime period, int clientId)
        {
            decimal result = 0;

            //the the subsidy header data
            TieredSubsidyBilling tsb = DA.Current.Query<TieredSubsidyBilling>().FirstOrDefault(x => x.Client.ClientID == clientId && x.Period == period);
            if (tsb == null) return 0;

            if (tsb.UserTotalSum != 0)
            {
                decimal totalDiscount = tsb.UserTotalSum - tsb.UserPaymentSum;
                decimal totalOriginalPayment = tsb.UserTotalSum;
                result = totalDiscount / totalOriginalPayment;
            }
            else
            {
                //get the subsidy details, there should be only one row
                //the row will contain the UserPaymentPercentage for whatever tier they are in based on their accumlated amount (and zero UserTotalSum)
                //any single charge can be multiplied by this % because if the sum of all charges equal zero then the sum of all discounts will also equal zero
                TieredSubsidyBillingDetail detail = tsb.Details.FirstOrDefault();
                if (detail == null) return 0;
                result = 1 - detail.UserPaymentPercentage;
            }

            return result;
        }

        public static void DistributeSubsidyMoneyEvenly(DateTime period, int clientId)
        {
            //Get all the subsidy discount per person in UM
            //Get all RoomBilling and ToolBilling tables with UM
            DataSet ds = GetTablesForSubsidyDiscountDistribution(period, clientId);
            DataTable dtSubsidy = ds.Tables[0];
            DataTable dtRoomBilling = ds.Tables[1];
            DataTable dtToolBilling = ds.Tables[2];
            DataTable dtUser = ds.Tables[3];
            DataTable dtMiscBilling = ds.Tables[4];

            //Get the sum of total cost
            int cid = 0;
            DataRow[] subsidyRows;
            DataRow[] roomBillingRows;
            DataRow[] toolBillingRows;
            DataRow[] miscBillingRows;
            decimal totalDiscount = 0;
            decimal totalOriginalPayment = 0;
            decimal subsidyFactor = 0;
            //double discountFactor = 0;

            foreach (DataRow dr in dtUser.Rows)
            {
                cid = Convert.ToInt32(dr["ClientID"]);

                subsidyRows = dtSubsidy.Select(string.Format("ClientID = {0}", cid));

                if (subsidyRows.Length == 1)
                {
                    totalDiscount = Convert.ToDecimal(subsidyRows[0]["UserTotalSum"]) - Convert.ToDecimal(subsidyRows[0]["UserPaymentSum"]);
                    roomBillingRows = dtRoomBilling.Select(string.Format("ClientID = {0}", cid));
                    toolBillingRows = dtToolBilling.Select(string.Format("ClientID = {0}", cid));
                    miscBillingRows = dtMiscBilling.Select(string.Format("ClientID = {0}", cid));

                    totalOriginalPayment = Convert.ToDecimal(subsidyRows[0]["UserTotalSum"]);

                    //subsidyFactor = TotalDiscount / TotalOriginalPayment;
                    subsidyFactor = GetSubsidyDiscountPercentage(period, cid);
                    //discountFactor = 1 - subsidyFactor;

                    if (totalOriginalPayment != 0)
                    {
                        decimal subsidyDiscount = 0;

                        foreach (DataRow drRoom in roomBillingRows)
                        {
                            subsidyDiscount = drRoom.Field<decimal>("TotalCharge") * Convert.ToDecimal(subsidyFactor);
                            drRoom["SubsidyDiscount"] = subsidyDiscount;
                        }

                        foreach (DataRow drTool in toolBillingRows)
                        {
                            subsidyDiscount = drTool.Field<decimal>("TotalCharge") * Convert.ToDecimal(subsidyFactor);
                            drTool.SetField("SubsidyDiscount", subsidyDiscount);
                        }

                        foreach (DataRow drMisc in miscBillingRows)
                        {
                            subsidyDiscount = drMisc.Field<decimal>("TotalCharge") * Convert.ToDecimal(subsidyFactor);
                            drMisc.SetField("SubsidyDiscount", subsidyDiscount);
                        }
                    }
                    else
                    {
                        //it's possible to have zero payment
                        foreach (DataRow drRoom in roomBillingRows)
                        {
                            drRoom.SetField("SubsidyDiscount", 0M);
                        }

                        foreach (DataRow drTool in toolBillingRows)
                        {
                            drTool.SetField("SubsidyDiscount", 0M);
                        }

                        foreach (DataRow drMisc in miscBillingRows)
                        {
                            drMisc.SetField("SubsidyDiscount", 0M);
                        }

                        ExceptionManager exp = new ExceptionManager
                        {
                            TimeStamp = DateTime.Now,
                            ExpName = "User has zero or negative TotalOriginalPayment",
                            AppName = typeof(BillingDataProcessStep4Subsidy).Assembly.GetName().Name,
                            FunctionName = "CommonTools-DistributeSubsidyMoneyEvenly",
                            CustomData = string.Format("ClientID = {0} Period = {1}", cid, period)
                        };
                        exp.LogException();
                    }
                }
            }

            SaveSubsidyDiscountRoom(dtRoomBilling);
            SaveSubsidyDiscountTool(dtToolBilling);
            SaveSubsidyDiscountRoomToolMisc(dtMiscBilling);

            CleanUpAfterSubsidy();
        }

        private static void CleanUpAfterSubsidy()
        {
            using (var dba = DA.Current.GetAdapter())
            {
                dba.SelectCommand.ApplyParameters(new { Action = "CleanUpAfterSubsidy" }).ExecuteNonQuery("RoomApportionmentInDaysMonthly_Update");
            }
        }

        private static void SaveSubsidyDiscountRoomToolMisc(DataTable dtIn)
        {
            using (var dba = DA.Current.GetAdapter())
            {
                // Update the data using dateset's batch update feature.
                dba.UpdateCommand
                    .AddParameter("@ExpID", SqlDbType.Int)
                    .AddParameter("@SubsidyDiscount", SqlDbType.Money);

                // Only send email on failure.
                if (dba.UpdateDataTable(dtIn, updateSql: "MiscBillingCharge_Update_SubsidyDiscount") < 0)
                    Providers.Email.SendMessage(0, "LNF.CommonTools.BillingDataProcessStep4Subsidy.SaveSubsidyDiscountRoomToolMisc(DataTable dtIn)", string.Format("Update MiscBilling SubsidyDiscount Failed - saving to database [{0}]", DateTime.Now), string.Empty, SendEmail.SystemEmail, SendEmail.DeveloperEmails);
            }
        }

        private static void SaveSubsidyDiscountRoom(DataTable dtIn)
        {
            using (var dba = DA.Current.GetAdapter())
            {
                // Update the data using dateset's batch update feature.
                dba.UpdateCommand
                    .AddParameter("@AppID", SqlDbType.Int)
                    .AddParameter("@SubsidyDiscount", SqlDbType.Money);

                // Only send email on failure.
                if (dba.UpdateDataTable(dtIn, null, "RoomApportionmentInDaysMonthly_Update_SubsidyDiscount", null) < 0)
                    Providers.Email.SendMessage(0, "LNF.CommonTools.BillingDataProcessStep4Subsidy.SaveSubsidyDiscountRoom(DataTable dtIn)", string.Format("Update RoomBilling SubsidyDiscount Failed - saving to database [{0}]", DateTime.Now), string.Empty, SendEmail.SystemEmail, SendEmail.DeveloperEmails);
            }
        }

        private static void SaveSubsidyDiscountTool(DataTable dtIn)
        {
            using (var dba = DA.Current.GetAdapter())
            {
                // Update the data using dateset's batch update feature.
                dba.UpdateCommand
                    .AddParameter("@ToolBillingID", SqlDbType.Int)
                    .AddParameter("@SubsidyDiscount", SqlDbType.Money);

                // Only send email on failure.
                if (dba.UpdateDataTable(dtIn, null, "ToolBilling_Update_SubsidyDiscount", null) < 0)
                    Providers.Email.SendMessage(0, "LNF.CommonTools.BillingDataProcessStep4Subsidy.SaveSubsidyDiscountTool(DataTable dtIn)", string.Format("Update ToolBilling SubsidyDiscount Failed - saving to database [{0}]", DateTime.Now), string.Empty, SendEmail.SystemEmail, SendEmail.DeveloperEmails);
            }
        }

        private static DataSet GetTablesForSubsidyDiscountDistribution(DateTime period, int clientId = 0)
        {
            using (var dba = DA.Current.GetAdapter())
            {
                dba.SelectCommand
                    .AddParameter("@Action", "ForSubsidyDiscountDistribution")
                    .AddParameter("@Period", period)
                    .AddParameterIf("@ClientID", clientId > 0, clientId);
                return dba.FillDataSet("TieredSubsidyBilling_Select");
            }
        }

        private static void CalculateSubsidyFee(DateTime period, int clientId = 0)
        {
            //Get Tieres table
            DataSet ds = GetTieredSubsidyBillingRelatedTables(period, clientId);
            DataTable dtIn = ds.Tables[0];
            DataTable dtDetails = ds.Tables[1]; //Empty table
            DataTable dtTiers = ds.Tables[2];

            DataTable dtOriginalSubsidyStartDate = GetFirstSubsidyDateTable(clientId);

            SortedList<double, double> tierRegular = new SortedList<double, double>();
            SortedList<double, double> tierNewUser = new SortedList<double, double>();
            SortedList<double, double> tierNewFacultyUser = new SortedList<double, double>();
            SortedList<double, double> currentTier = new SortedList<double, double>();
            TransformTiersIntoSortedDictionary(dtTiers, tierRegular, tierNewUser, tierNewFacultyUser);

            DataRow drNew;
            double startSum;
            double endSum;
            double userTotalSum;
            bool isNegative = false;

            foreach (DataRow dr in dtIn.Rows)
            {
                isNegative = false;
                userTotalSum = Convert.ToDouble(dr["UserTotalSum"]);
                if (Convert.ToDouble(dr["UserTotalSum"]) < 0)
                {
                    isNegative = true;
                    userTotalSum *= -1;
                }

                startSum = Convert.ToDouble(dr["Accumulated"]);
                endSum = startSum + userTotalSum;

                // [2011-02-17] This is code for new faculty billing, must comment out the above expression.
                // [2016-03-08 jg] Changing priority order to check IsNewStudent first because this is the only type of subsidy
                //       at this time. The order can now be changed by rearranging the elements in the tierPriority dictionary.
                Dictionary<string, SortedList<double, double>> tierPriority = new Dictionary<string, SortedList<double, double>>
                {
                    { "IsNewStudent", tierNewUser },
                    { "IsNewFacultyUser", tierNewFacultyUser },
                    { "Default", tierRegular }
                };

                foreach (var kvp in tierPriority)
                {
                    if (kvp.Key == "Default" || Convert.ToBoolean(dr[kvp.Key]))
                    {
                        currentTier = kvp.Value;
                        break;
                    }
                }

                if (currentTier == null)
                    throw new InvalidOperationException("Cannot determine the current subsidy tier.");

                //Tier 0 means negative amount - it's possible that we should pay the user for specific month
                //Tier 1 is the first tier, Tier 2 is second tier  
                int floorTier = 0;
                int topTier = 0;

                foreach (KeyValuePair<double, double> kvp in currentTier)
                {
                    if (kvp.Key > startSum)
                    {
                        //it's possible that StartSum is negative
                        break;
                    }
                    else
                        floorTier += 1;
                }

                foreach (KeyValuePair<double, double> kvp in currentTier)
                {
                    if (kvp.Key < endSum)
                        topTier += 1;
                    else
                        break;
                }

                //after this point, we know which tiers we are in now
                double currentUpperValue = 0;
                int currentTierIndex; //since list is zero indexed based, I need this to keep track of indexing indicator
                for (int i = floorTier; i <= topTier; i++)
                {
                    currentTierIndex = i - 1;
                    drNew = dtDetails.NewRow();

                    //Make sure we don't increment when we are at the top level
                    if (currentTierIndex + 1 < currentTier.Count)
                        currentUpperValue = currentTier.Keys[currentTierIndex + 1];
                    else
                    {
                        //come here when user is in last tier, there is no more upper value
                        currentUpperValue = 999999999;
                    }

                    int tierBillingID = dr.Field<int>("TierBillingID");
                    double floorAmount = currentTier.Keys[currentTierIndex];
                    double userPaymentPercentage = currentTier.Values[currentTierIndex];

                    drNew["Period"] = period;
                    drNew["TierBillingID"] = tierBillingID;
                    drNew["FloorAmount"] = floorAmount;
                    drNew["UserPaymentPercentage"] = userPaymentPercentage;

                    double originalApplyAmount = 0; //this is the portion of the total usage charge applied to a given subsidy tier
                    if (floorTier == topTier)
                    {
                        //1st Case: When we are just in one tier, there is no cross tier calculation needed
                        originalApplyAmount = (isNegative) ? (endSum - startSum) * -1 : endSum - startSum;
                    }
                    else
                    {
                        //2nd Case: We have crossed multiple tiers
                        if (i == topTier)
                        {
                            //We are in the last tier of multi tier calculation
                            originalApplyAmount = (isNegative) ? (endSum - currentTier.Keys[currentTierIndex]) * -1 : endSum - currentTier.Keys[currentTierIndex];
                        }
                        else
                        {
                            //We are in any tier except last
                            if (i == floorTier)
                            {
                                //We are in first tier
                                originalApplyAmount = (isNegative) ? (currentUpperValue - startSum) * -1 : currentUpperValue - startSum;
                            }
                            else
                            {
                                //we are in the middle tier
                                originalApplyAmount = (isNegative) ? (currentUpperValue - currentTier.Keys[currentTierIndex]) * -1 : currentUpperValue - currentTier.Keys[currentTierIndex];
                            }
                        }
                    }
                    drNew["OriginalApplyAmount"] = originalApplyAmount;
                    dtDetails.Rows.Add(drNew);
                }
            }

            //Clean up the data before saving
            DeleteTieredSubsidyBillingDetail(period, clientId);

            //Save everything back to the main table
            SaveNewTieredSubsidyBillingDetail(dtDetails);

            //Now fill out the UserPaymentSum Column of TieredSubsidyBilling
            UpdateUserPaymentSum(period, clientId);
        }

        private static void UpdateUserPaymentSum(DateTime period, int clientId = 0)
        {
            DataSet ds = GetNecessaryTablesForUpdatingUserPayment(period, clientId);
            DataTable dtTierBilling = ds.Tables[0];
            DataTable dtTierBillingDetailGroupByTierBillingID = ds.Tables[1];

            foreach (DataRow dr in dtTierBilling.Rows)
            {
                DataRow[] rows = dtTierBillingDetailGroupByTierBillingID.Select(string.Format("TierBillingID = {0}", dr["TierBillingID"]));
                double userPaymentSum = rows[0].Field<double>("UserPaymentSum");
                dr.SetField("UserPaymentSum", userPaymentSum);
            }

            using (var dba = DA.Current.GetAdapter())
            {
                // Update the data using dateset's batch update feature.
                dba.UpdateCommand
                    .AddParameter("@TierBillingID", SqlDbType.Int)
                    .AddParameter("@UserPaymentSum", SqlDbType.Float);

                // Only send email on failure.
                if (dba.UpdateDataTable(dtTierBilling, updateSql: "TieredSubsidyBilling_Update") < 0)
                    Providers.Email.SendMessage(0, "LNF.CommonTools.BillingDataProcessStep4Subsidy.UpdateUserPaymentSum(DateTime period, int clientId = 0)", string.Format("Update UserPaymentSum Failed - saving to database [{0}]", DateTime.Now), string.Empty, SendEmail.SystemEmail, SendEmail.DeveloperEmails);
            }
        }

        private static DataSet GetNecessaryTablesForUpdatingUserPayment(DateTime period, int clientId = 0)
        {
            using (var dba = DA.Current.GetAdapter())
            {
                dba.SelectCommand
                    .AddParameter("@Action", "ForUpdatingUserPaymentSum")
                    .AddParameter("@Period", period)
                    .AddParameterIf("@ClientID", clientId > 0, clientId);
                return dba.FillDataSet("TieredSubsidyBilling_Select");
            }
        }

        private static void DeleteTieredSubsidyBillingDetail(DateTime period, int clientId = 0)
        {
            using (var dba = DA.Current.GetAdapter())
            {
                dba.SelectCommand
                    .AddParameter("@Action", "DeleteCurrentRange")
                    .AddParameter("@Period", period)
                    .AddParameterIf("@ClientID", clientId > 0, clientId)
                    .ExecuteNonQuery("TieredSubsidyBillingDetail_Delete");
            }
        }

        private static void SaveNewTieredSubsidyBillingDetail(DataTable dtIn)
        {
            using (var dba = DA.Current.GetAdapter())
            {
                dba.InsertCommand
                    .AddParameter("@Period", SqlDbType.DateTime)
                    .AddParameter("@TierBillingID", SqlDbType.Int)
                    .AddParameter("@FloorAmount", SqlDbType.Float)
                    .AddParameter("@UserPaymentPercentage", SqlDbType.Float)
                    .AddParameter("@OriginalApplyAmount", SqlDbType.Float);

                // Only send email on failure.
                if (dba.UpdateDataTable(dtIn, "TieredSubsidyBillingDetail_Insert") < 0)
                    Providers.Email.SendMessage(0, "LNF.CommonTools.BillingDataProcessStep4Subsidy.SaveNewTieredSubsidyBillingDetail(DataTable dtIn)", string.Format("Error in Processing TieredSubsidyBillingDetail - saving to database [{0}]", DateTime.Now), string.Empty, SendEmail.SystemEmail, SendEmail.DeveloperEmails);
            }
        }

        private static DataSet GetTieredSubsidyBillingRelatedTables(DateTime period, int clientId = 0)
        {
            using (var dba = DA.Current.GetAdapter())
            {
                dba.SelectCommand
                    .AddParameter("@Action", "SelectAllByPeriod")
                    .AddParameter("@Period", period)
                    .AddParameterIf("@ClientID", clientId > 0, clientId);
                return dba.FillDataSet("TieredSubsidyBilling_Select");
            }
        }

        private static DataTable GetTieredSubsidyBillingDetailSchema()
        {
            using (var dba = DA.Current.GetAdapter())
                return dba.ApplyParameters(new { Action = "SelectSchema" }).FillDataTable("TieredSubsidyBillingDetail_Select");
        }

        private static DataTable GetSubsidyTiers(DateTime period)
        {
            using (var dba = DA.Current.GetAdapter())
                return dba.ApplyParameters(new { Period = period }).FillDataTable("TieredSubsidyTiers_Select");
        }

        private static void TransformTiersIntoSortedDictionary(DataTable dtTiers, SortedList<double, double> TierRegular, SortedList<double, double> TierNewUser, SortedList<double, double> TierNewFacultyUser)
        {
            IEnumerable<DataRow> query;

            query = from tempobj in dtTiers.AsEnumerable()
                    where Convert.ToInt32(tempobj["GroupID"]) == 0
                    select tempobj;

            foreach (DataRow dr in query)
            {
                TierRegular.Add(Convert.ToDouble(dr["FloorAmount"]), Convert.ToDouble(dr["UserPaymentPercentage"]));
            }

            query = from tempobj in dtTiers.AsEnumerable()
                    where Convert.ToInt32(tempobj["GroupID"]) == 1
                    select tempobj;

            foreach (DataRow dr in query)
            {
                TierNewUser.Add(Convert.ToDouble(dr["FloorAmount"]), Convert.ToDouble(dr["UserPaymentPercentage"]));
            }

            query = from tempobj in dtTiers.AsEnumerable()
                    where Convert.ToInt32(tempobj["GroupID"]) == 2
                    select tempobj;

            foreach (DataRow dr in query)
            {
                TierNewFacultyUser.Add(Convert.ToDouble(dr["FloorAmount"]), Convert.ToDouble(dr["UserPaymentPercentage"]));
            }
        }

        private static int DeleteTieredSubsidyBilling(DateTime period, int clientId = 0)
        {
            using (var dba = DA.Current.GetAdapter())
            {
                return dba.SelectCommand
                    .AddParameter("@Action", "DeleteCurrentRange")
                    .AddParameter("@Period", period)
                    .AddParameterIf("@ClientID", clientId > 0, clientId)
                    .ExecuteNonQuery("TieredSubsidyBilling_Delete");
            }
        }

        private static int SaveNewTieredSubsidyBilling(DataTable dtIn)
        {
            using (var dba = DA.Current.GetAdapter())
            {

                dba.InsertCommand
                    .AddParameter("@Period", SqlDbType.DateTime)
                    .AddParameter("@ClientID", SqlDbType.Int)
                    .AddParameter("@OrgID", SqlDbType.Int)
                    .AddParameter("@RoomSum", SqlDbType.Float)
                    .AddParameter("@RoomMiscSum", SqlDbType.Float)
                    .AddParameter("@ToolSum", SqlDbType.Float)
                    .AddParameter("@ToolMiscSum", SqlDbType.Float)
                    .AddParameter("@UserPaymentSum", SqlDbType.Float)
                    .AddParameter("@StartingPeriod", SqlDbType.DateTime)
                    .AddParameter("@Accumulated", SqlDbType.Float)
                    .AddParameter("@IsNewStudent", SqlDbType.Bit)
                    .AddParameter("@IsNewFacultyUser", SqlDbType.Bit);

                int count = dba.UpdateDataTable(dtIn, "TieredSubsidyBilling_Insert");

                // Only send email on failure.
                if (count < 0)
                    Providers.Email.SendMessage(0, "LNF.CommonTools.BillingDataProcessStep4Subsidy.SaveNewTieredSubsidyBilling(DataTable dtIn)", string.Format("Error in Processing TieredSubsidyBilling - saving to database [{0}]", DateTime.Now), string.Empty, SendEmail.SystemEmail, SendEmail.DeveloperEmails);

                return count;
            }
        }

        //This will populate necessary data fields for calculating the new subsidy amount
        //key fields: Accumulated original amount and Starting Period
        private static void PopulateFieldsFromHistory(DateTime period, DataTable dtIn, int clientId)
        {
            //Get everone's original starting date and current fiscal year
            DataTable dtLastBillingData = GetLastUsedDateFromTieredSubsidyBilling(period, clientId);
            DataTable dtOriginalSubsidyStartDate = GetFirstSubsidyDateTable(clientId);

            foreach (DataRow dr in dtIn.Rows)
            {
                int cid = dr.Field<int>("ClientID");

                DateTime? myLastLabUsagePeriod = GetFiscalYearStartingPeriod(cid, dtLastBillingData);
                DateTime? mySubsidyStartDate = GetSubsidyStartDate(cid, dtOriginalSubsidyStartDate);

                if (myLastLabUsagePeriod == null)
                {
                    //No Starting Period, this means user uses the lab for the first time after 2010-07-01 when subsidy started
                    if (mySubsidyStartDate != null)
                    {
                        dr["StartingPeriod"] = GetLatestFiscalYear(period, mySubsidyStartDate.Value);
                        dr["Accumulated"] = 0;

                        if (period < mySubsidyStartDate.Value.AddYears(1) && period >= mySubsidyStartDate)
                            dr["IsNewStudent"] = true;
                        else
                            dr["IsNewStudent"] = false;
                    }
                    else
                    {
                        //Exception - no SubsidyStartDate record
                        dr["StartingPeriod"] = DBNull.Value;
                        dr["Accumulated"] = 0;
                        dr["IsNewStudent"] = false;

                        ExceptionManager exp = new ExceptionManager { TimeStamp = DateTime.Now, ExpName = "No SubsidyStartDate", AppName = typeof(BillingDataProcessStep4Subsidy).Assembly.GetName().Name, FunctionName = "CommonTools-PopulateFieldsFromHistory", CustomData = string.Format("ClientID = {0}, Period = {1}", cid, period) };
                        exp.LogException();
                    }
                }
                else
                {
                    //We got fiscalYearStarting Period, now we have to see if it's a new year
                    DateTime currentStartingPeriod = mySubsidyStartDate.Value;

                    while (currentStartingPeriod.AddYears(1) < period)
                    {
                        currentStartingPeriod = currentStartingPeriod.AddYears(1);
                    }

                    //we need to see we have a new year coming
                    if (currentStartingPeriod.AddYears(1) == period)
                    {
                        //new fiscal year
                        dr["StartingPeriod"] = GetLatestFiscalYear(period, mySubsidyStartDate.Value);
                        dr["Accumulated"] = 0;
                    }
                    else if (myLastLabUsagePeriod < currentStartingPeriod)
                    {
                        //new fiscal year, for people who skip the cut off month (most people are july 2009)
                        dr["StartingPeriod"] = GetLatestFiscalYear(period, mySubsidyStartDate.Value);
                        dr["Accumulated"] = 0;
                    }
                    else
                    {
                        //still the same fiscal year
                        dr["StartingPeriod"] = GetLatestFiscalYear(period, mySubsidyStartDate.Value);
                        double? accum = GetAccumulatedSum(cid, dtLastBillingData);
                        dr["Accumulated"] = (accum > 0) ? accum : 0;
                    }

                    //Determine whether it's New Student or not
                    if (period < mySubsidyStartDate.Value.AddYears(1) && period >= mySubsidyStartDate)
                        dr["IsNewStudent"] = true;
                    else
                        dr["IsNewStudent"] = false;
                }

                //2010-12 determine if this user belong to a new faculty
                dr["IsNewFacultyUser"] = GetIsNewFacultyUser(period, cid);
            }
        }

        private static bool GetIsNewFacultyUser(DateTime period, int clientId)
        {
            using (var dba = DA.Current.GetAdapter())
            {
                var args = new
                {
                    Action = "GetManagerOrgIDNewFaculty",
                    sDate = period,
                    eDate = period.AddMonths(1),
                    ClientID = clientId
                };

                using (IDataReader reader = dba.ApplyParameters(args).ExecuteReader("ClientManager_Select"))
                {
                    bool result = reader.Read();
                    reader.Close();
                    return result;
                }
            }
        }

        private static DateTime GetLatestFiscalYear(DateTime Period, DateTime OriginalStartingPeriod)
        {
            DateTime curserTime = OriginalStartingPeriod;
            while (curserTime <= Period)
                curserTime = curserTime.AddYears(1);
            return curserTime.AddYears(-1);
        }

        private static DateTime? GetFiscalYearStartingPeriod(int clientId, DataTable dt)
        {
            return GetDateFromTable(clientId, dt, "Period");
        }

        private static DateTime? GetSubsidyStartDate(int clientId, DataTable dt)
        {
            return GetDateFromTable(clientId, dt, "SubsidyStartDate");
        }

        private static double? GetAccumulatedSum(int clientId, DataTable dt)
        {
            IEnumerable<DataRow> query = from ddr in dt.AsEnumerable()
                                         where Convert.ToInt32(ddr["ClientID"]) == clientId
                                         select ddr;

            DataRow dr = query.FirstOrDefault();

            if (dr == null) return null;
            else return Convert.ToDouble(dr["AccumulatedSum"]);
        }

        private static DateTime? GetDateFromTable(int clientId, DataTable dt, string type)
        {
            IEnumerable<DataRow> query = from ddr in dt.AsEnumerable()
                                         where Convert.ToInt32(ddr["ClientID"]) == clientId
                                         select ddr;

            DataRow dr = query.FirstOrDefault();

            if (dr == null) return null;
            else return Convert.ToDateTime(dr[type]);
        }

        private static bool IsNewFiscalYear(DateTime Period, DateTime LastUsedDate)
        {
            return false;
        }

        private static DataTable GetFirstSubsidyDateTable(int clientId = 0)
        {
            using (var dba = DA.Current.GetAdapter())
            {
                dba.SelectCommand
                    .AddParameter("@Action", "GetAllActiveStartDate")
                    .AddParameterIf("@ClientID", clientId > 0, clientId);

                return dba.FillDataTable("ClientOrg_Select");
            }
        }

        private static DataTable GetLastUsedDateFromTieredSubsidyBilling(DateTime period, int clientId = 0)
        {
            using (var dba = DA.Current.GetAdapter())
            {
                dba.SelectCommand
                    .AddParameter("@Action", "GetLastUsedStartingDate")
                    .AddParameter("@Period", period)
                    .AddParameterIf("@ClientID", clientId > 0, clientId);

                return dba.FillDataTable("TieredSubsidyBilling_Select");
            }
        }

        //Get Misc Charge
        private static double GetMiscSumPerClient(DataTable dt, int clientId, string subType)
        {
            DataRow[] drs = dt.Select(string.Format("SUBType = '{0}' AND ClientID = {1}", subType, clientId));
            if (drs.Length > 0)
                return drs.First().Field<double>("MiscCost");
            else
                return 0.0;
        }

        private static DataSet GetNecessaryTables(DateTime period, int clientId = 0)
        {
            using (var dba = DA.Current.GetAdapter())
            {
                dba.SelectCommand
                    .AddParameter("@Action", "PopulateTieredSubsidyBilling")
                    .AddParameter("@Period", period)
                    .AddParameterIf("@ClientID", clientId > 0, clientId);

                return dba.FillDataSet("TieredSubsidyBilling_Select");
            }
        }

    }
}
