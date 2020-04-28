using LNF.Billing.Process;
using LNF.CommonTools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace LNF.Impl.Billing
{
    //2010-03-23 Subsidy billing data process - run after the 3rd day of business every month
    public class BillingDataProcessStep4Subsidy : ReaderBase
    {
        private DataTable _special = null;
        private DataTable _newfac = null;

        public DateTime Period { get; private set; }
        public int ClientID { get; private set; }

        public BillingDataProcessStep4Subsidy(SqlConnection conn) : base(conn)
        {
            using (var cmd = new SqlCommand("SELECT * FROM Billing.dbo.SpecialSubsidy", conn) { CommandType = CommandType.Text })
            using (var adap = new SqlDataAdapter(cmd))
            {
                _special = new DataTable();
                adap.Fill(_special);
            }
        }

        private DataTable GetNewFaculty()
        {
            if (_newfac == null)
            {
                using (var cmd = new SqlCommand("dbo.ClientManager_Select", Connection) { CommandType = CommandType.StoredProcedure })
                using (var adap = new SqlDataAdapter(cmd))
                {
                    cmd.Parameters.AddWithValue("Action", "GetNewFaculty");
                    cmd.Parameters.AddWithValue("sDate", Period);
                    cmd.Parameters.AddWithValue("eDate", Period.AddMonths(1));
                    AddParameterIf(cmd, "ClientID", ClientID > 0, ClientID);

                    _newfac = new DataTable();
                    adap.Fill(_newfac);
                }
            }

            return _newfac;
        }

        private bool IsSpecial(int clientId, out DateTime ssd)
        {
            var rows = _special.Select($"ClientID = {clientId}");

            if (rows.Length > 0)
            {
                ssd = (DateTime)rows[0]["SubsidyStartDate"];
                return ssd.AddYears(1) > Period;
            }
            else
            {
                ssd = default(DateTime);
                return false;
            }
        }

        public PopulateSubsidyBillingResult PopulateSubsidyBilling(Step4Command cmd)
        {
            Period = cmd.Period;
            ClientID = cmd.ClientID;

            var result = new PopulateSubsidyBillingResult
            {
                Period = Period,
                ClientID = ClientID,
                Command = "subsidy"
            };

            DataSet ds = GetNecessaryTables();
            DataTable dtRoom = ds.Tables[0];
            DataTable dtTool = ds.Tables[1];
            DataTable dtTiers = ds.Tables[2];
            DataTable dtOut = ds.Tables[3];
            DataTable dtMiscCharges = ds.Tables[4];

            // The strategy here is we loop through dtRoom where all users in current month should have data in TieredSubsidyBilling Table
            // Then we loop through dtTool in case if someone had used the tool, but not the room (unlikely, but possible)
            // Then we calculate the user payment 

            // The code below will populate two columns "RoomSum" and "RoomMiscSum"
            DataRow newrow;
            foreach (DataRow dr in dtRoom.Rows)
            {
                newrow = dtOut.NewRow();

                newrow["Period"] = dr["Period"];
                newrow["ClientID"] = dr["ClientID"];
                newrow["OrgID"] = dr["OrgID"];

                newrow["RoomSum"] = dr["TotalCharge"];
                newrow["RoomMiscSum"] = GetMiscSumPerClient(dtMiscCharges, dr.Field<int>("ClientID"), "Room");

                // We still need to set the tool for people who don't use tool (but only room)
                newrow["ToolSum"] = 0;
                newrow["ToolMiscSum"] = 0;

                // The column does not allow NULL
                newrow["UserPaymentSum"] = 0;

                dtOut.Rows.Add(newrow);
            }

            foreach (DataRow dr in dtTool.Rows)
            {
                DataRow[] drs = dtOut.Select($"ClientID = {dr["ClientID"]} AND OrgID = {dr["OrgID"]}");

                if (drs.Length == 1)
                {
                    // we found data record for room, so we use the same data record
                    drs[0]["ToolSum"] = dr["TotalCharge"];
                    drs[0]["ToolMiscSum"] = GetMiscSumPerClient(dtMiscCharges, dr.Field<int>("ClientID"), "Tool");
                }
                else if (drs.Length == 0)
                {
                    // This user uses tool, but not room, so it's a whole new record
                    newrow = dtOut.NewRow();

                    newrow["Period"] = dr["Period"];
                    newrow["ClientID"] = dr["ClientID"];
                    newrow["OrgID"] = dr["OrgID"];

                    newrow["RoomSum"] = 0;
                    newrow["RoomMiscSum"] = 0;

                    newrow["ToolSum"] = dr["TotalCharge"];
                    newrow["ToolMiscSum"] = GetMiscSumPerClient(dtMiscCharges, dr.Field<int>("ClientID"), "Tool");

                    // The column does not allow NULL
                    newrow["UserPaymentSum"] = 0;

                    dtOut.Rows.Add(newrow);
                }
                else
                {
                    // error, it's impossible to have more than 1 data
                    SendEmail.SendDeveloperEmail("LNF.CommonTools.BillingDataProcessStep4Subsidy.PopulateSubsidyBilling", "Error in populating TieredSubsidyBilling", $"There is more than one row for client {drs[0]["ClientID"]}");
                }
            }

            foreach (DataRow dr in dtMiscCharges.Rows)
            {
                DataRow[] drs = dtOut.Select($"ClientID = {dr["ClientID"]}");

                if (drs.Length == 0)
                {
                    // This user has no tool nor room, so it's a whole new record
                    newrow = dtOut.NewRow();

                    newrow["Period"] = Period;
                    newrow["ClientID"] = dr["ClientID"];
                    newrow["OrgID"] = 17; // always internal

                    newrow["RoomSum"] = 0;
                    newrow["RoomMiscSum"] = GetMiscSumPerClient(dtMiscCharges, Convert.ToInt32(dr["ClientID"]), "Room");

                    newrow["ToolSum"] = 0;
                    newrow["ToolMiscSum"] = GetMiscSumPerClient(dtMiscCharges, Convert.ToInt32(dr["ClientID"]), "Tool");

                    // The column does not allow NULL
                    newrow["UserPaymentSum"] = 0;

                    dtOut.Rows.Add(newrow);
                }
            }

            result.RowsExtracted = dtOut.Rows.Count;

            // At this point, the TiredSubsidyBilling table should have all the records but some fields are missing.  
            // So we loop through the newly constructed table and fill out the missing fields
            PopulateFieldsFromHistory(dtOut);

            // Clean up the data before saving
            result.RowsDeleted = DeleteTieredSubsidyBilling();

            // Save everything back to the main table
            result.RowsLoaded = SaveNewTieredSubsidyBilling(dtOut);

            // Calculate the real subsidy amount and populate the details, UserPaymentSum is set in this method.
            CalculateSubsidyFee();

            // Push changes back to billing tables
            DistributeSubsidyMoneyEvenly();

            // [2015-10-20 jg] Added account subsidy feature per Sandrine. Some accounts get a fixed subsidy that overrides the tiered subsidy
            ApplyAccountSubsidy();

            return result;
        }

        private void ApplyAccountSubsidy()
        {
            DateTime sd = Period;
            DateTime ed = Period.AddMonths(1);

            using (var cmd = new SqlCommand("SELECT * FROM Billing.dbo.v_CurrentAccountSubsidy ORDER BY AccountID", Connection) { CommandType = CommandType.Text })
            using (var adap = new SqlDataAdapter(cmd))
            {
                var dt = new DataTable();
                adap.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                    ApplyAccountSubsidy(dr, Period);
            }
        }

        private void ApplyAccountSubsidy(DataRow dr, DateTime period)
        {
            // get all the billing records with this account and override the subsidy discount

            var accountId = dr.Field<int>("AccountID");
            var userPaymentPercentage = dr.Field<decimal>("UserPaymentPercentage");

            // xxxxx Tool xxxxx
            ApplyToolAccountSubsidy(accountId, userPaymentPercentage, period);

            // xxxxx Room xxxxx
            ApplyRoomAccountSubsidy(accountId, userPaymentPercentage, period);

            // xxxxx Misc xxxxx
            ApplyMiscAccountSubsidy(accountId, userPaymentPercentage, period);
        }

        private void ApplyToolAccountSubsidy(int accountId, decimal userPaymentPercentage, DateTime period)
        {
            var dtTool = new DataTable();

            using (var cmd = new SqlCommand("SELECT * FROM dbo.ToolBilling WHERE Period = @Period AND AccountID = @AccountID", Connection) { CommandType = CommandType.Text })
            using (var adap = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("Period", period);
                cmd.Parameters.AddWithValue("AccountID", accountId);
                adap.Fill(dtTool);
            }

            foreach (DataRow tb in dtTool.Rows)
            {
                tb["SubsidyDiscount"] = GetToolBillingTotalCharge(tb) * userPaymentPercentage;
            }

            using (var update = new SqlCommand("UPDATE dbo.ToolBilling SET SubsidyDiscount = @SubsidyDiscount WHERE ToolBillingID = @ToolBillingID", Connection) { CommandType = CommandType.Text })
            using (var adap = new SqlDataAdapter() { UpdateCommand = update })
            {
                update.Parameters.Add("ToolBillingID", SqlDbType.Int, 0, "ToolBillingID");
                update.Parameters.Add("SubsidyDiscount", SqlDbType.Decimal, 0, "SubsidyDiscount");
                adap.Update(dtTool);
            }
        }

        private void ApplyRoomAccountSubsidy(int accountId, decimal userPaymentPercentage, DateTime period)
        {
            var dtRoom = new DataTable();

            using (var cmd = new SqlCommand("SELECT * FROM dbo.RoomApportionmentInDaysMonthly WHERE Period = @Period AND AccountID = @AccountID", Connection) { CommandType = CommandType.Text })
            using (var adap = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("Period", period);
                cmd.Parameters.AddWithValue("AccountID", accountId);
                adap.Fill(dtRoom);
            }

            foreach (DataRow rb in dtRoom.Rows)
            {
                rb["SubsidyDiscount"] = GetRoomBillingTotalCharge(rb) * userPaymentPercentage;
            }

            using (var update = new SqlCommand("UPDATE dbo.RoomApportionmentInDaysMonthly SET SubsidyDiscount = @SubsidyDiscount WHERE AppID = @AppID", Connection) { CommandType = CommandType.Text })
            using (var adap = new SqlDataAdapter() { UpdateCommand = update })
            {
                update.Parameters.Add("AppID", SqlDbType.Int, 0, "AppID");
                update.Parameters.Add("SubsidyDiscount", SqlDbType.Decimal, 0, "SubsidyDiscount");
                adap.Update(dtRoom);
            }
        }

        private void ApplyMiscAccountSubsidy(int accountId, decimal userPaymentPercentage, DateTime period)
        {
            var dtMisc = new DataTable();

            using (var cmd = new SqlCommand("SELECT * FROM dbo.MiscBillingCharge WHERE Period = @Period AND AccountID = @AccountID AND SUBType IN ('Tool', 'Room')", Connection) { CommandType = CommandType.Text })
            using (var adap = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("Period", period);
                cmd.Parameters.AddWithValue("AccountID", accountId);
                adap.Fill(dtMisc);
            }

            foreach (DataRow rb in dtMisc.Rows)
            {
                rb["SubsidyDiscount"] = GetMiscBillingTotalCharge(rb) * userPaymentPercentage;
            }

            using (var update = new SqlCommand("UPDATE dbo.MiscBillingCharge SET SubsidyDiscount = @SubsidyDiscount WHERE ExpID = @ExpID", Connection) { CommandType = CommandType.Text })
            using (var adap = new SqlDataAdapter() { UpdateCommand = update })
            {
                update.Parameters.Add("ExpID", SqlDbType.Int, 0, "ExpID");
                update.Parameters.Add("SubsidyDiscount", SqlDbType.Decimal, 0, "SubsidyDiscount");
                adap.Update(dtMisc);
            }
        }

        public decimal GetSubsidyDiscountPercentage(DataTable dtTieredSubsidyBilling, int clientId)
        {
            decimal result = 0;

            //the the subsidy header data
            DataRow tsb = dtTieredSubsidyBilling.Select($"ClientID = {clientId} AND Period = '{Period}'").FirstOrDefault();

            if (tsb == null) return 0;

            var tierBillingId = tsb.Field<int>("TierBillingID");
            var userTotalSum = tsb.Field<decimal>("UserTotalSum");
            var userPaymentSum = tsb.Field<decimal>("UserPaymentSum");

            if (userTotalSum != 0)
            {
                decimal totalDiscount = userTotalSum - userPaymentSum;
                decimal totalOriginalPayment = userTotalSum;
                result = totalDiscount / totalOriginalPayment;
            }
            else
            {
                //get the subsidy details, there should be only one row
                //the row will contain the UserPaymentPercentage for whatever tier they are in based on their accumlated amount (and zero UserTotalSum)
                //any single charge can be multiplied by this % because if the sum of all charges equal zero then the sum of all discounts will also equal zero
                using (var cmd = new SqlCommand("SELECT ISNULL(UserPaymentPercentage, 0) FROM dbo.TieredSubsidyBillingDetail WHERE TierBillingID = @TierBillingID", Connection) { CommandType = CommandType.Text })
                {
                    cmd.Parameters.AddWithValue("TierBillingID", tierBillingId);
                    var userPaymentPercentage = Convert.ToDecimal(cmd.ExecuteScalar());
                    result = 1 - userPaymentPercentage;
                }
            }

            return result;
        }

        public void DistributeSubsidyMoneyEvenly()
        {
            //Get all the subsidy discount per person in UM
            //Get all RoomBilling and ToolBilling tables with UM
            DataSet ds = GetTablesForSubsidyDiscountDistribution();
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

            DataTable dtTieredSubsidyBilling;

            using (var cmd = new SqlCommand("SELECT * FROM dbo.TieredSubsidyBilling WHERE Period = @Period", Connection) { CommandType = CommandType.Text })
            using (var adap = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("Period", Period);
                dtTieredSubsidyBilling = new DataTable();
                adap.Fill(dtTieredSubsidyBilling);
            }

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
                    subsidyFactor = GetSubsidyDiscountPercentage(dtTieredSubsidyBilling, cid);
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
                            CustomData = $"ClientID = {cid}, Period = {Period}"
                        };

                        exp.LogException(Connection);
                    }
                }
            }

            SaveSubsidyDiscountRoom(dtRoomBilling);
            SaveSubsidyDiscountTool(dtToolBilling);
            SaveSubsidyDiscountRoomToolMisc(dtMiscBilling);

            CleanUpAfterSubsidy();
        }

        private void CleanUpAfterSubsidy()
        {
            using (var cmd = new SqlCommand("dbo.RoomApportionmentInDaysMonthly_Update", Connection) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("Action", "CleanUpAfterSubsidy");
                cmd.ExecuteNonQuery();
            }
        }

        private void SaveSubsidyDiscountRoomToolMisc(DataTable dtIn)
        {
            using (var update = new SqlCommand("dbo.MiscBillingCharge_Update_SubsidyDiscount", Connection) { CommandType = CommandType.StoredProcedure })
            using (var adap = new SqlDataAdapter { UpdateCommand = update })
            {
                update.Parameters.Add("ExpID", SqlDbType.Int, 0, "ExpID");
                update.Parameters.Add("SubsidyDiscount", SqlDbType.Money, 0, "SubsidyDiscount");

                var count = adap.Update(dtIn);

                // Only send email on failure.
                if (count < 0)
                    SendEmail.SendDeveloperEmail("LNF.CommonTools.BillingDataProcessStep4Subsidy.SaveSubsidyDiscountRoomToolMisc", $"Update MiscBilling SubsidyDiscount Failed - saving to database [{DateTime.Now:yyyy-MM-dd HH:mm:ss}]");
            }
        }

        private void SaveSubsidyDiscountRoom(DataTable dtIn)
        {
            using (var update = new SqlCommand("dbo.RoomApportionmentInDaysMonthly_Update_SubsidyDiscount", Connection) { CommandType = CommandType.StoredProcedure })
            using (var adap = new SqlDataAdapter { UpdateCommand = update })
            {
                update.Parameters.Add("AppID", SqlDbType.Int, 0, "AppID");
                update.Parameters.Add("SubsidyDiscount", SqlDbType.Money, 0, "SubsidyDiscount");

                var count = adap.Update(dtIn);

                // Only send email on failure.
                if (count < 0)
                    SendEmail.SendDeveloperEmail("LNF.CommonTools.BillingDataProcessStep4Subsidy.SaveSubsidyDiscountRoom", $"Update RoomBilling SubsidyDiscount Failed - saving to database [{DateTime.Now:yyyy-MM-dd HH:mm:ss}]");
            }
        }

        private void SaveSubsidyDiscountTool(DataTable dtIn)
        {
            using (var update = new SqlCommand("dbo.ToolBilling_Update_SubsidyDiscount", Connection) { CommandType = CommandType.StoredProcedure })
            using (var adap = new SqlDataAdapter { UpdateCommand = update })
            {
                update.Parameters.Add("ToolBillingID", SqlDbType.Int, 0, "ToolBillingID");
                update.Parameters.Add("SubsidyDiscount", SqlDbType.Money, 0, "SubsidyDiscount");

                var count = adap.Update(dtIn);

                // Only send email on failure.
                if (count < 0)
                    SendEmail.SendDeveloperEmail("LNF.CommonTools.BillingDataProcessStep4Subsidy.SaveSubsidyDiscountTool", $"Update ToolBilling SubsidyDiscount Failed - saving to database [{DateTime.Now:yyyy-MM-dd HH:mm:ss}]");
            }
        }

        private DataSet GetTablesForSubsidyDiscountDistribution()
        {
            using (var cmd = new SqlCommand("dbo.TieredSubsidyBilling_Select", Connection) { CommandType = CommandType.StoredProcedure })
            using (var adap = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("Action", "ForSubsidyDiscountDistribution");
                cmd.Parameters.AddWithValue("Period", Period);
                AddParameterIf(cmd, "ClientID", ClientID > 0, ClientID);

                var ds = new DataSet();
                adap.Fill(ds);

                return ds;
            }
        }

        private void CalculateSubsidyFee()
        {
            //Get Tieres table
            DataSet ds = GetTieredSubsidyBillingRelatedTables();
            DataTable dtIn = ds.Tables[0];
            DataTable dtDetails = ds.Tables[1]; //Empty table
            DataTable dtTiers = ds.Tables[2];

            DataTable dtOriginalSubsidyStartDate = GetFirstSubsidyDateTable();

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

                    drNew["Period"] = Period;
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
            DeleteTieredSubsidyBillingDetail();

            //Save everything back to the main table
            SaveNewTieredSubsidyBillingDetail(dtDetails);

            //Now fill out the UserPaymentSum Column of TieredSubsidyBilling
            UpdateUserPaymentSum();
        }

        private void UpdateUserPaymentSum()
        {
            DataSet ds = GetNecessaryTablesForUpdatingUserPayment();
            DataTable dtTierBilling = ds.Tables[0];
            DataTable dtTierBillingDetailGroupByTierBillingID = ds.Tables[1];

            foreach (DataRow dr in dtTierBilling.Rows)
            {
                DataRow[] rows = dtTierBillingDetailGroupByTierBillingID.Select($"TierBillingID = {dr["TierBillingID"]}");
                double userPaymentSum = rows[0].Field<double>("UserPaymentSum");
                dr.SetField("UserPaymentSum", userPaymentSum);
            }

            // Update the data using dateset's batch update feature.
            using (var update = new SqlCommand("dbo.TieredSubsidyBilling_Update", Connection) { CommandType = CommandType.StoredProcedure })
            using (var adap = new SqlDataAdapter { UpdateCommand = update })
            {
                update.Parameters.Add("TierBillingID", SqlDbType.Int, 0, "TierBillingID");
                update.Parameters.Add("UserPaymentSum", SqlDbType.Float, 0, "UserPaymentSum");
                var count = adap.Update(dtTierBilling);

                // Only send email on failure.
                if (count < 0)
                    SendEmail.SendDeveloperEmail("LNF.CommonTools.BillingDataProcessStep4Subsidy.UpdateUserPaymentSum", $"Update UserPaymentSum Failed - saving to database [{DateTime.Now:yyyy-MM-dd HH:mm:ss}]");
            }
        }

        private DataSet GetNecessaryTablesForUpdatingUserPayment()
        {
            using (var cmd = new SqlCommand("dbo.TieredSubsidyBilling_Select", Connection) { CommandType = CommandType.StoredProcedure })
            using (var adap = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("Action", "ForUpdatingUserPaymentSum");
                cmd.Parameters.AddWithValue("Period", Period);
                AddParameterIf(cmd, "ClientID", ClientID > 0, ClientID);

                var ds = new DataSet();
                adap.Fill(ds);

                return ds;
            }
        }

        private void DeleteTieredSubsidyBillingDetail()
        {
            using (var cmd = new SqlCommand("dbo.TieredSubsidyBillingDetail_Delete", Connection) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("Action", "DeleteCurrentRange");
                cmd.Parameters.AddWithValue("Period", Period);
                AddParameterIf(cmd, "ClientID", ClientID > 0, ClientID);
                cmd.ExecuteNonQuery();
            }
        }

        private void SaveNewTieredSubsidyBillingDetail(DataTable dtIn)
        {
            using (var insert = new SqlCommand("dbo.TieredSubsidyBillingDetail_Insert", Connection) { CommandType = CommandType.StoredProcedure })
            using (var adap = new SqlDataAdapter { InsertCommand = insert })
            {
                insert.Parameters.Add("Period", SqlDbType.DateTime, 0, "Period");
                insert.Parameters.Add("TierBillingID", SqlDbType.Int, 0, "TierBillingID");
                insert.Parameters.Add("FloorAmount", SqlDbType.Float, 0, "FloorAmount");
                insert.Parameters.Add("UserPaymentPercentage", SqlDbType.Float, 0, "UserPaymentPercentage");
                insert.Parameters.Add("OriginalApplyAmount", SqlDbType.Float, 0, "OriginalApplyAmount");

                var count = adap.Update(dtIn);

                // Only send email on failure.
                if (count < 0)
                    SendEmail.SendDeveloperEmail("LNF.CommonTools.BillingDataProcessStep4Subsidy.SaveNewTieredSubsidyBillingDetail", $"Error in Processing TieredSubsidyBillingDetail - saving to database [{DateTime.Now:yyyy-MM-dd HH:mm:ss}]");
            }
        }

        private DataSet GetTieredSubsidyBillingRelatedTables()
        {
            using (var cmd = new SqlCommand("dbo.TieredSubsidyBilling_Select", Connection) { CommandType = CommandType.StoredProcedure })
            using (var adap = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("Action", "SelectAllByPeriod");
                cmd.Parameters.AddWithValue("Period", Period);
                AddParameterIf(cmd, "ClientID", ClientID > 0, ClientID);

                var ds = new DataSet();
                adap.Fill(ds);

                return ds;
            }
        }

        private DataTable GetTieredSubsidyBillingDetailSchema()
        {
            using (var cmd = new SqlCommand("dbo.TieredSubsidyBillingDetail_Select", Connection) { CommandType = CommandType.StoredProcedure })
            using (var adap = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("Action", "SelectSchema");

                var dt = new DataTable();
                adap.Fill(dt);

                return dt;
            }
        }

        private DataTable GetSubsidyTiers(DateTime period)
        {
            using (var cmd = new SqlCommand("dbo.TieredSubsidyTiers_Select", Connection) { CommandType = CommandType.StoredProcedure })
            using (var adap = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("Period", period);

                var dt = new DataTable();
                adap.Fill(dt);

                return dt;
            }
        }

        private void TransformTiersIntoSortedDictionary(DataTable dtTiers, SortedList<double, double> TierRegular, SortedList<double, double> TierNewUser, SortedList<double, double> TierNewFacultyUser)
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

        private int DeleteTieredSubsidyBilling()
        {
            using (var cmd = new SqlCommand("dbo.TieredSubsidyBilling_Delete", Connection) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("Action", "DeleteCurrentRange");
                cmd.Parameters.AddWithValue("Period", Period);
                AddParameterIf(cmd, "ClientID", ClientID > 0, ClientID);
                var result = cmd.ExecuteNonQuery();
                return result;
            }
        }

        private int SaveNewTieredSubsidyBilling(DataTable dtIn)
        {
            using (var insert = new SqlCommand("dbo.TieredSubsidyBilling_Insert", Connection) { CommandType = CommandType.StoredProcedure })
            using (var adap = new SqlDataAdapter { InsertCommand = insert })
            {
                insert.Parameters.Add("Period", SqlDbType.DateTime, 0, "Period");
                insert.Parameters.Add("ClientID", SqlDbType.Int, 0, "ClientID");
                insert.Parameters.Add("OrgID", SqlDbType.Int, 0, "OrgID");
                insert.Parameters.Add("RoomSum", SqlDbType.Float, 0, "RoomSum");
                insert.Parameters.Add("RoomMiscSum", SqlDbType.Float, 0, "RoomMiscSum");
                insert.Parameters.Add("ToolSum", SqlDbType.Float, 0, "ToolSum");
                insert.Parameters.Add("ToolMiscSum", SqlDbType.Float, 0, "ToolMiscSum");
                insert.Parameters.Add("UserPaymentSum", SqlDbType.Float, 0, "UserPaymentSum");
                insert.Parameters.Add("StartingPeriod", SqlDbType.DateTime, 0, "StartingPeriod");
                insert.Parameters.Add("Accumulated", SqlDbType.Float, 0, "Accumulated");
                insert.Parameters.Add("IsNewStudent", SqlDbType.Bit, 0, "IsNewStudent");
                insert.Parameters.Add("IsNewFacultyUser", SqlDbType.Bit, 0, "IsNewFacultyUser");

                var count = adap.Update(dtIn);

                // Only send email on failure.
                if (count < 0)
                    SendEmail.SendDeveloperEmail("LNF.CommonTools.BillingDataProcessStep4Subsidy.SaveNewTieredSubsidyBilling", $"Error in Processing TieredSubsidyBilling - saving to database [{DateTime.Now:yyyy-MM-dd HH:mm:ss}]");

                return count;
            }
        }

        //This will populate necessary data fields for calculating the new subsidy amount
        //key fields: Accumulated original amount and Starting Period
        private void PopulateFieldsFromHistory(DataTable dtIn)
        {
            //Get everone's original starting date and current fiscal year
            DataTable dtLastBillingData = GetLastUsedDateFromTieredSubsidyBilling();
            DataTable dtOriginalSubsidyStartDate = GetFirstSubsidyDateTable();

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
                        dr["StartingPeriod"] = GetLatestFiscalYear(mySubsidyStartDate.Value);
                        dr["Accumulated"] = 0;

                        if (Period < mySubsidyStartDate.Value.AddYears(1) && Period >= mySubsidyStartDate)
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

                        ExceptionManager exp = new ExceptionManager
                        {
                            TimeStamp = DateTime.Now,
                            ExpName = "No SubsidyStartDate",
                            AppName = typeof(BillingDataProcessStep4Subsidy).Assembly.GetName().Name,
                            FunctionName = "LNF.CommonTools.BillingDataProcessStep4.PopulateFieldsFromHistory",
                            CustomData = $"ClientID = {cid}, Period = {Period}"
                        };

                        exp.LogException(Connection);
                    }
                }
                else
                {
                    //We got fiscalYearStarting Period, now we have to see if it's a new year
                    DateTime currentStartingPeriod = mySubsidyStartDate.Value;

                    while (currentStartingPeriod.AddYears(1) < Period)
                    {
                        currentStartingPeriod = currentStartingPeriod.AddYears(1);
                    }

                    //we need to see we have a new year coming
                    if (currentStartingPeriod.AddYears(1) == Period)
                    {
                        //new fiscal year
                        dr["StartingPeriod"] = GetLatestFiscalYear(mySubsidyStartDate.Value);
                        dr["Accumulated"] = 0;
                    }
                    else if (myLastLabUsagePeriod < currentStartingPeriod)
                    {
                        //new fiscal year, for people who skip the cut off month (most people are july 2009)
                        dr["StartingPeriod"] = GetLatestFiscalYear(mySubsidyStartDate.Value);
                        dr["Accumulated"] = 0;
                    }
                    else
                    {
                        //still the same fiscal year
                        dr["StartingPeriod"] = GetLatestFiscalYear(mySubsidyStartDate.Value);
                        double? accum = GetAccumulatedSum(cid, dtLastBillingData);
                        dr["Accumulated"] = (accum > 0) ? accum : 0;
                    }

                    //Determine whether it's New Student or not
                    if (Period < mySubsidyStartDate.Value.AddYears(1) && Period >= mySubsidyStartDate)
                        dr["IsNewStudent"] = true;
                    else
                        dr["IsNewStudent"] = false;
                }

                //2010-12 determine if this user belong to a new faculty
                dr["IsNewFacultyUser"] = GetIsNewFacultyUser(cid);
            }
        }

        private bool GetIsNewFacultyUser(int clientId)
        {
            if (IsSpecial(clientId, out DateTime ssd))
            {
                return true;
            }

            var rows = GetNewFaculty().Select($"ClientID = {clientId}");

            return rows.Length > 0;
        }

        private DateTime GetLatestFiscalYear(DateTime originalStartingPeriod)
        {
            DateTime curserTime = originalStartingPeriod;
            while (curserTime <= Period)
                curserTime = curserTime.AddYears(1);
            return curserTime.AddYears(-1);
        }

        private DateTime? GetFiscalYearStartingPeriod(int clientId, DataTable dt)
        {
            return GetDateFromTable(clientId, dt, "Period");
        }

        private DateTime? GetSubsidyStartDate(int clientId, DataTable dt)
        {
            return GetDateFromTable(clientId, dt, "SubsidyStartDate");
        }

        private double? GetAccumulatedSum(int clientId, DataTable dt)
        {
            IEnumerable<DataRow> query = from ddr in dt.AsEnumerable()
                                         where Convert.ToInt32(ddr["ClientID"]) == clientId
                                         select ddr;

            DataRow dr = query.FirstOrDefault();

            if (dr == null) return null;
            else return Convert.ToDouble(dr["AccumulatedSum"]);
        }

        private DateTime? GetDateFromTable(int clientId, DataTable dt, string type)
        {
            IEnumerable<DataRow> query = from ddr in dt.AsEnumerable()
                                         where Convert.ToInt32(ddr["ClientID"]) == clientId
                                         select ddr;

            DataRow dr = query.FirstOrDefault();

            if (dr == null) return null;
            else return Convert.ToDateTime(dr[type]);
        }

        private DataTable GetFirstSubsidyDateTable()
        {
            using (var cmd = new SqlCommand("dbo.ClientOrg_Select", Connection) { CommandType = CommandType.StoredProcedure })
            using (var adap = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("Action", "GetAllActiveStartDate");
                AddParameterIf(cmd, "ClientID", ClientID > 0, ClientID);

                var dt = new DataTable();
                adap.Fill(dt);

                return dt;
            }
        }

        private DataTable GetLastUsedDateFromTieredSubsidyBilling()
        {
            using (var cmd = new SqlCommand("dbo.TieredSubsidyBilling_Select", Connection) { CommandType = CommandType.StoredProcedure })
            using (var adap = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("Action", "GetLastUsedStartingDate");
                cmd.Parameters.AddWithValue("Period", Period);
                AddParameterIf(cmd, "ClientID", ClientID > 0, ClientID);

                var dt = new DataTable();
                adap.Fill(dt);

                return dt;
            }
        }

        //Get Misc Charge
        private double GetMiscSumPerClient(DataTable dt, int clientId, string subType)
        {
            DataRow[] drs = dt.Select($"SUBType = '{subType}' AND ClientID = {clientId}");

            if (drs.Length > 0)
                return drs.First().Field<double>("MiscCost");
            else
                return 0.0;
        }

        private DataSet GetNecessaryTables()
        {
            using (var cmd = new SqlCommand("dbo.TieredSubsidyBilling_Select", Connection) { CommandType = CommandType.StoredProcedure })
            using (var adap = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("Action", "PopulateTieredSubsidyBilling");
                cmd.Parameters.AddWithValue("Period", Period);
                AddParameterIf(cmd, "ClientID", ClientID > 0, ClientID);

                var ds = new DataSet();
                adap.Fill(ds);

                return ds;
            }
        }

        private decimal GetToolBillingTotalCharge(DataRow dr)
        {
            // We can include everything now because the value in each column is correct.
            // For example: after 2015-10-01 UncancelledPenaltyFee and ReservationFee2 will be zero
            // and before 2011-04-01 BookingFee will be zero. And UsageFeeCharged is whatever value
            // was calculated based on the rules in place at the time. By making the data correct
            // based on the current rules we don't have to check the period and apply different
            // logic in many different places. In other words this formula will work for any
            // period - much easier to manage.

            var usageFeeCharged = dr.Field<decimal>("UsageFeeCharged");
            var overTimePenaltyFee = dr.Field<decimal>("OverTimePenaltyFee");
            var bookingFee = dr.Field<decimal>("BookingFee");
            var uncancelledPenaltyFee = dr.Field<decimal>("UncancelledPenaltyFee");
            var reservationFee2 = dr.Field<decimal>("ReservationFee2");

            return usageFeeCharged + overTimePenaltyFee + bookingFee + uncancelledPenaltyFee + reservationFee2;
        }

        private decimal GetRoomBillingTotalCharge(DataRow dr)
        {
            // this matches the stored procedure TieredSubsidyBilling_Select @Action = 'ForSubsidyDiscountDistribution'

            var roomCharge = dr.Field<decimal>("RoomCharge");
            var entryCharge = dr.Field<decimal>("EntryCharge");

            return roomCharge + entryCharge;
        }

        private decimal GetMiscBillingTotalCharge(DataRow dr)
        {
            // this matches the stored procedure TieredSubsidyBilling_Select @Action = 'ForSubsidyDiscountDistribution'

            var quantity = dr.Field<double>("Quantity");
            var unitCost = dr.Field<decimal>("UnitCost");

            return Convert.ToDecimal(quantity) * unitCost;
        }
    }
}
