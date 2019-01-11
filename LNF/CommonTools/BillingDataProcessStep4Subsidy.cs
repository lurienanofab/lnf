using LNF.Billing;
using LNF.Models.Billing.Process;
using LNF.Repository;
using LNF.Repository.Billing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.CommonTools
{
    //2010-03-23 Subsidy billing data process - run after the 3rd day of business every month
    public static class BillingDataProcessStep4Subsidy
    {
        public static PopulateSubsidyBillingProcessResult PopulateSubsidyBilling(DateTime period, int clientId = 0)
        {
            var result = new PopulateSubsidyBillingProcessResult
            {
                Period = period,
                ClientID = clientId,
                Command = "subsidy"
            };

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
                DataRow[] drs = dtOut.Select($"ClientID = {dr["ClientID"]} AND OrgID = {dr["OrgID"]}");

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
                    ServiceProvider.Current.Email.SendMessage(0, "LNF.CommonTools.BillingDataProcessStep4Subsidy.PopulateSubsidyBilling", "Error in populating TieredSubsidyBilling", $"There is more than one row for client {drs[0]["ClientID"]}", SendEmail.SystemEmail, SendEmail.DeveloperEmails);
                }
            }

            foreach (DataRow dr in dtMiscCharges.Rows)
            {
                DataRow[] drs = dtOut.Select($"ClientID = {dr["ClientID"]}");

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

            result.RowsExtracted = dtOut.Rows.Count;

            //At this point, the TiredSubsidyBilling table should have all the records but some fields are missing.  
            //So we loop through the newly constructed table and fill out the missing fields
            PopulateFieldsFromHistory(period, dtOut, clientId);

            //Clean up the data before saving
            result.RowsDeleted = DeleteTieredSubsidyBilling(period, clientId);

            //Save everything back to the main table
            result.RowsLoaded = SaveNewTieredSubsidyBilling(dtOut);

            //Calculate the real subsidy amount and populate the details, UserPaymentSum is set in this method.
            CalculateSubsidyFee(period, clientId);

            //Push changes back to billing tables
            DistributeSubsidyMoneyEvenly(period, clientId);

            //[2015-10-20 jg] Added account subsidy feature per Sandrine. Some accounts get a fixed subsidy that overrides the tiered subsidy
            ApplyAccountSubsidy(period);

            return result;
        }

        private static void ApplyAccountSubsidy(DateTime period)
        {
            DateTime sd = period;
            DateTime ed = period.AddMonths(1);

            //base query
            var baseQuery = DA.Current.Query<AccountSubsidy>()
                .Where(x => x.EnableDate < ed && (x.DisableDate == null || x.DisableDate > sd))
                .ToList();

            // step1: it is possible to have duplicates because of disabling and re-enabling in the same
            //        date range, in this case get the last one by joining to self grouped by max AccountSubsidyID
            var step1 = baseQuery.Join(
                baseQuery.GroupBy(x => x.AccountID).Select(g => new { Account = g.Key, AccountSubsidyID = g.Max(n => n.AccountSubsidyID) }),
                o => o.AccountSubsidyID,
                i => i.AccountSubsidyID,
                (o, i) => o);

            var accountSubsidy = step1.OrderBy(x => x.AccountID).ToList();

            foreach (var item in accountSubsidy)
                item.ApplyToBilling(period);
        }

        public static decimal GetSubsidyDiscountPercentage(IEnumerable<TieredSubsidyBilling> tieredSubsidyBilling, DateTime period, int clientId)
        {
            decimal result = 0;

            //the the subsidy header data
            TieredSubsidyBilling tsb = tieredSubsidyBilling.FirstOrDefault(x => x.Client.ClientID == clientId && x.Period == period);

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

            var tieredSubsidyBilling = DA.Current.Query<TieredSubsidyBilling>().Where(x => x.Period == period).ToList();

            foreach (DataRow dr in dtUser.Rows)
            {
                cid = Convert.ToInt32(dr["ClientID"]);

                string filter = $"ClientID = {cid}";

                subsidyRows = dtSubsidy.Select(filter);

                if (subsidyRows.Length == 1)
                {
                    totalDiscount = Convert.ToDecimal(subsidyRows[0]["UserTotalSum"]) - Convert.ToDecimal(subsidyRows[0]["UserPaymentSum"]);
                    roomBillingRows = dtRoomBilling.Select(filter);
                    toolBillingRows = dtToolBilling.Select(filter);
                    miscBillingRows = dtMiscBilling.Select(filter);

                    totalOriginalPayment = Convert.ToDecimal(subsidyRows[0]["UserTotalSum"]);

                    //subsidyFactor = TotalDiscount / TotalOriginalPayment;
                    subsidyFactor = GetSubsidyDiscountPercentage(tieredSubsidyBilling, period, cid);
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
                            CustomData = $"ClientID = {cid}, Period = {period}"
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
            DA.Command()
                .Param("Action", "CleanUpAfterSubsidy")
                .ExecuteNonQuery("dbo.RoomApportionmentInDaysMonthly_Update");
        }

        private static void SaveSubsidyDiscountRoomToolMisc(DataTable dtIn)
        {
            int count = DA.Command().Update(dtIn, x =>
            {
                x.Update.SetCommandText("dbo.MiscBillingCharge_Update_SubsidyDiscount");
                x.Update.AddParameter("ExpID", SqlDbType.Int);
                x.Update.AddParameter("SubsidyDiscount", SqlDbType.Money);
            });

            // Only send email on failure.
            if (count < 0)
                ServiceProvider.Current.Email.SendMessage(0, "LNF.CommonTools.BillingDataProcessStep4Subsidy.SaveSubsidyDiscountRoomToolMisc", $"Update MiscBilling SubsidyDiscount Failed - saving to database [{DateTime.Now:yyyy-MM-dd HH:mm:ss}]", string.Empty, SendEmail.SystemEmail, SendEmail.DeveloperEmails);
        }

        private static void SaveSubsidyDiscountRoom(DataTable dtIn)
        {
            int count = DA.Command().Update(dtIn, x =>
            {
                x.Update.SetCommandText("dbo.RoomApportionmentInDaysMonthly_Update_SubsidyDiscount");
                x.Update.AddParameter("AppID", SqlDbType.Int);
                x.Update.AddParameter("SubsidyDiscount", SqlDbType.Money);
            });

            // Only send email on failure.
            if (count < 0)
                ServiceProvider.Current.Email.SendMessage(0, "LNF.CommonTools.BillingDataProcessStep4Subsidy.SaveSubsidyDiscountRoom", $"Update RoomBilling SubsidyDiscount Failed - saving to database [{DateTime.Now:yyyy-MM-dd HH:mm:ss}]", string.Empty, SendEmail.SystemEmail, SendEmail.DeveloperEmails);
        }

        private static void SaveSubsidyDiscountTool(DataTable dtIn)
        {
            int count = DA.Command().Update(dtIn, x =>
            {
                x.Update.SetCommandText("dbo.ToolBilling_Update_SubsidyDiscount");
                x.Update.AddParameter("ToolBillingID", SqlDbType.Int);
                x.Update.AddParameter("SubsidyDiscount", SqlDbType.Money);
            });

            // Only send email on failure.
            if (count < 0)
                ServiceProvider.Current.Email.SendMessage(0, "LNF.CommonTools.BillingDataProcessStep4Subsidy.SaveSubsidyDiscountTool", $"Update ToolBilling SubsidyDiscount Failed - saving to database [{DateTime.Now:yyyy-MM-dd HH:mm:ss}]", string.Empty, SendEmail.SystemEmail, SendEmail.DeveloperEmails);
        }

        private static DataSet GetTablesForSubsidyDiscountDistribution(DateTime period, int clientId = 0)
        {
            return DA.Command()
                .Param("Action", "ForSubsidyDiscountDistribution")
                .Param("Period", period)
                .Param("ClientID", clientId > 0, clientId)
                .FillDataSet("dbo.TieredSubsidyBilling_Select");
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
                DataRow[] rows = dtTierBillingDetailGroupByTierBillingID.Select($"TierBillingID = {dr["TierBillingID"]}");
                double userPaymentSum = rows[0].Field<double>("UserPaymentSum");
                dr.SetField("UserPaymentSum", userPaymentSum);
            }

            // Update the data using dateset's batch update feature.
            int count = DA.Command().Update(dtTierBilling, x =>
            {
                x.Update.SetCommandText("dbo.TieredSubsidyBilling_Update");
                x.Update.AddParameter("TierBillingID", SqlDbType.Int);
                x.Update.AddParameter("UserPaymentSum", SqlDbType.Float);
            });

            // Only send email on failure.
            if (count < 0)
                ServiceProvider.Current.Email.SendMessage(0, "LNF.CommonTools.BillingDataProcessStep4Subsidy.UpdateUserPaymentSum", $"Update UserPaymentSum Failed - saving to database [{DateTime.Now:yyyy-MM-dd HH:mm:ss}]", string.Empty, SendEmail.SystemEmail, SendEmail.DeveloperEmails);
        }

        private static DataSet GetNecessaryTablesForUpdatingUserPayment(DateTime period, int clientId = 0)
        {
            return DA.Command()
                 .Param("Action", "ForUpdatingUserPaymentSum")
                 .Param("Period", period)
                 .Param("ClientID", clientId > 0, clientId)
                 .FillDataSet("dbo.TieredSubsidyBilling_Select");
        }

        private static void DeleteTieredSubsidyBillingDetail(DateTime period, int clientId = 0)
        {
            DA.Command()
                .Param("Action", "DeleteCurrentRange")
                .Param("Period", period)
                .Param("ClientID", clientId > 0, clientId)
                .ExecuteNonQuery("dbo.TieredSubsidyBillingDetail_Delete");
        }

        private static void SaveNewTieredSubsidyBillingDetail(DataTable dtIn)
        {
            int count = DA.Command().Update(dtIn, x =>
            {
                x.Insert.SetCommandText("dbo.TieredSubsidyBillingDetail_Insert");
                x.Insert.AddParameter("Period", SqlDbType.DateTime);
                x.Insert.AddParameter("TierBillingID", SqlDbType.Int);
                x.Insert.AddParameter("FloorAmount", SqlDbType.Float);
                x.Insert.AddParameter("UserPaymentPercentage", SqlDbType.Float);
                x.Insert.AddParameter("OriginalApplyAmount", SqlDbType.Float);
            });

            // Only send email on failure.
            if (count < 0)
                ServiceProvider.Current.Email.SendMessage(0, "LNF.CommonTools.BillingDataProcessStep4Subsidy.SaveNewTieredSubsidyBillingDetail", $"Error in Processing TieredSubsidyBillingDetail - saving to database [{DateTime.Now:yyyy-MM-dd HH:mm:ss}]", string.Empty, SendEmail.SystemEmail, SendEmail.DeveloperEmails);
        }

        private static DataSet GetTieredSubsidyBillingRelatedTables(DateTime period, int clientId = 0)
        {
            return DA.Command()
                .Param("Action", "SelectAllByPeriod")
                .Param("Period", period)
                .Param("ClientID", clientId > 0, clientId)
                .FillDataSet("dbo.TieredSubsidyBilling_Select");
        }

        private static DataTable GetTieredSubsidyBillingDetailSchema()
        {
            return DA.Command()
                .Param("Action", "SelectSchema")
                .FillDataTable("dbo.TieredSubsidyBillingDetail_Select");
        }

        private static DataTable GetSubsidyTiers(DateTime period)
        {
            return DA.Command()
                .Param("Period", period)
                .FillDataTable("dbo.TieredSubsidyTiers_Select");
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
            return DA.Command()
                .Param("Action", "DeleteCurrentRange")
                .Param("Period", period)
                .Param("ClientID", clientId > 0, clientId)
                .ExecuteNonQuery("dbo.TieredSubsidyBilling_Delete").Value;
        }

        private static int SaveNewTieredSubsidyBilling(DataTable dtIn)
        {
            int count = DA.Command().Update(dtIn, x =>
            {
                x.Insert.SetCommandText("dbo.TieredSubsidyBilling_Insert");
                x.Insert.AddParameter("Period", SqlDbType.DateTime);
                x.Insert.AddParameter("ClientID", SqlDbType.Int);
                x.Insert.AddParameter("OrgID", SqlDbType.Int);
                x.Insert.AddParameter("RoomSum", SqlDbType.Float);
                x.Insert.AddParameter("RoomMiscSum", SqlDbType.Float);
                x.Insert.AddParameter("ToolSum", SqlDbType.Float);
                x.Insert.AddParameter("ToolMiscSum", SqlDbType.Float);
                x.Insert.AddParameter("UserPaymentSum", SqlDbType.Float);
                x.Insert.AddParameter("StartingPeriod", SqlDbType.DateTime);
                x.Insert.AddParameter("Accumulated", SqlDbType.Float);
                x.Insert.AddParameter("IsNewStudent", SqlDbType.Bit);
                x.Insert.AddParameter("IsNewFacultyUser", SqlDbType.Bit);
            });

            // Only send email on failure.
            if (count < 0)
                ServiceProvider.Current.Email.SendMessage(0, "LNF.CommonTools.BillingDataProcessStep4Subsidy.SaveNewTieredSubsidyBilling", $"Error in Processing TieredSubsidyBilling - saving to database [{DateTime.Now:yyyy-MM-dd HH:mm:ss}]", string.Empty, SendEmail.SystemEmail, SendEmail.DeveloperEmails);

            return count;
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

                        ExceptionManager exp = new ExceptionManager()
                        {
                            TimeStamp = DateTime.Now,
                            ExpName = "No SubsidyStartDate",
                            AppName = typeof(BillingDataProcessStep4Subsidy).Assembly.GetName().Name,
                            FunctionName = "LNF.CommonTools.BillingDataProcessStep4.PopulateFieldsFromHistory",
                            CustomData = $"ClientID = {cid}, Period = {period}"
                        };

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
            var args = new
            {
                Action = "GetManagerOrgIDNewFaculty",
                sDate = period,
                eDate = period.AddMonths(1),
                ClientID = clientId
            };

            using (IDataReader reader = DA.Command().Param(args).ExecuteReader("dbo.ClientManager_Select"))
            {
                bool result = reader.Read();
                reader.Close();
                return result;
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

        private static DataTable GetFirstSubsidyDateTable(int clientId = 0)
        {
            return DA.Command()
                .Param("Action", "GetAllActiveStartDate")
                .Param("ClientID", clientId > 0, clientId)
                .FillDataTable("dbo.ClientOrg_Select");
        }

        private static DataTable GetLastUsedDateFromTieredSubsidyBilling(DateTime period, int clientId = 0)
        {
            return DA.Command()
                .Param("Action", "GetLastUsedStartingDate")
                .Param("Period", period)
                .Param("ClientID", clientId > 0, clientId)
                .FillDataTable("TieredSubsidyBilling_Select");
        }

        //Get Misc Charge
        private static double GetMiscSumPerClient(DataTable dt, int clientId, string subType)
        {
            DataRow[] drs = dt.Select($"SUBType = '{subType}' AND ClientID = {clientId}");

            if (drs.Length > 0)
                return drs.First().Field<double>("MiscCost");
            else
                return 0.0;
        }

        private static DataSet GetNecessaryTables(DateTime period, int clientId = 0)
        {
            return DA.Command()
                .Param("Action", "PopulateTieredSubsidyBilling")
                .Param("Period", period)
                .Param("ClientID", clientId > 0, clientId)
                .FillDataSet("dbo.TieredSubsidyBilling_Select");
        }

    }
}
