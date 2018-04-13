using LNF.Repository;
using LNF.Repository.Billing;
using System;
using System.Collections;
using System.Data;

namespace LNF.CommonTools
{
    public class ApportionmentInDaysMonthlyProcessor
    {
        /// <summary>
        /// The Main Process
        /// </summary>
        /// <param name="Period"></param>
        /// <remarks></remarks>

        public void ProcessMonthlyData(DateTime Period)
        {
            //Step 1: Get users and rooms for this period and loop through each of them
            //Step 1.1: Get all the necessary data from database to save the round trip cost to DB connection
            //The code below will get a total of 7 tables
            DataTable dtResult = GetApportionmentTableSchema();
            dtResult.Columns.Add("Percentage", typeof(double)); //add this temporary column for convenience in calculating proper ChargeDays value, this will not be pushed to DB
            DataSet dsSource = GetRequiredDataFromDB(Period);
            DataTable dtUser = dsSource.Tables[0];
            DataTable dtRoom = dsSource.Tables[1];
            DataTable dtAccount = dsSource.Tables[2];
            DataTable dtRoomDay = dsSource.Tables[3];
            DataTable dtRoomMonth = dsSource.Tables[4];
            DataTable dtToolDay = dsSource.Tables[5];
            DataTable dtDefault = dsSource.Tables[6];
            DataTable dtAccountsUsedInTool = dsSource.Tables[7];
            DataTable dtRoomCost = dsSource.Tables[8];
            DataTable dtRoomDataClean = dsSource.Tables[9]; //used to find out the accurate number of entries for NAP rooms

            int clientId = 0;
            int roomId = 0;
            int accountId = 0;
            int billingTypeId = 0;
            int physicalDays = 0;
            int accountDays = 0;
            int chargeDays = 0;
            double entries = 0;
            double hours = 0;
            double maxDays = 0;
            double minDays = 0;
            double totalEntries = 0;
            double totalHours = 0;
            double totalMonthlyRoomCharge = 0;
            double defaultPercentage = 0;
            int numberOfAccountsPerClient = 0;
            DateTime enableDate, disableDate;

            //loop through each user and for each room
            foreach (DataRow urow in dtUser.Rows)
            {
                clientId = Convert.ToInt32(urow["ClientID"]);
                DataRow[] drowsAccountOriginal = dtAccount.Select(string.Format("ClientID = {0}", clientId));

                numberOfAccountsPerClient = drowsAccountOriginal.Length;

                foreach (DataRow rrow in dtRoom.Rows)
                {
                    roomId = Convert.ToInt32(rrow["RoomID"]);
                    DataRow[] rowsRoomMonth = dtRoomMonth.Select(string.Format("ClientID = {0} AND RoomID = {1}", clientId, roomId));
                    if (rowsRoomMonth.Length > 0)
                    {
                        physicalDays = Convert.ToInt32(rowsRoomMonth[0]["PhysicalDays"]);
                        totalHours = Convert.ToInt32(rowsRoomMonth[0]["TotalHoursPerMonth"]);

                        //hard code the roomID here.  No better alternative
                        if (roomId == 2 || roomId == 4)
                        {
                            DataRow[] rowsRoomClean = dtRoomDataClean.Select(string.Format("ClientID = {0} AND RoomID = {1}", clientId, roomId));
                            totalEntries = Convert.ToDouble(rowsRoomClean[0]["TotalEntriesForNAPRoom"]);
                        }
                        else
                        {
                            totalEntries = Convert.ToDouble(rowsRoomMonth[0]["TotalEntriesPerMonth"]);
                        }

                        //2009-07-21 remote processing accounts need to be added as well.  Remoting processing is room specific, so we have to use RoomID
                        //as a filter and allows remote accounts based on the room
                        DataRow[] rowsUsedAccountInTool = dtAccountsUsedInTool.Select(string.Format("ClientID = {0} AND RoomID = {1}", clientId, roomId));
                        ArrayList arr = new ArrayList();
                        foreach (DataRow dr in rowsUsedAccountInTool)
                        {
                            bool isFound = false;
                            foreach (DataRow ar in drowsAccountOriginal)
                            {
                                if (Convert.ToInt32(ar["AccountID"]) == Convert.ToInt32(dr["AccountID"]))
                                {
                                    isFound = true;
                                    break;
                                }
                            }

                            if (!isFound)
                            {
                                //not found, so this is remote processing account, we should add it to the drowsaccount
                                DataRow newrow = dtAccount.NewRow();
                                newrow["AccountID"] = dr["AccountID"];
                                newrow["ClientID"] = clientId;
                                newrow["EnableDate"] = Period;
                                newrow["DisableDate"] = Period.AddMonths(1).AddDays(-1);
                                newrow["ChargeTypeID"] = dr["ChargeTypeID"];

                                dtAccount.Rows.Add(newrow);

                                arr.Add(dr["AccountID"]);
                            }
                        }

                        //we might have new remote accounts added, so we have to regenerate all the rows with this client
                        DataRow[] drowsAccountWithRemote = dtAccount.Select(string.Format("ClientID = {0}", clientId));
                        numberOfAccountsPerClient = drowsAccountWithRemote.Length;

                        foreach (DataRow arow in drowsAccountWithRemote)
                        {
                            accountId = Convert.ToInt32(arow["AccountID"]);
                            enableDate = Convert.ToDateTime(arow["EnableDate"]);
                            billingTypeId = Convert.ToInt32(arow["BillingTypeID"]);

                            if (arow["DisableDate"] != DBNull.Value)
                                disableDate = Convert.ToDateTime(arow["DisableDate"]);
                            else
                                disableDate = DateTime.Now.AddMonths(120); //if it's null, it means this account is active, so we set it as a very long date in future since datetime cannot be null

                            //Check if this acount is enabled or disabled during the period - we need to change the physical days boundary according to accounts active days
                            int modifiedPhysicalDays = physicalDays; //holds reduced days if account is disabled during the period.  By default, we must set it as same as PhysicalDays

                            //the idea here is we have initial value of PhysicalDays, and for accounts that are enabled or(and) disabled in this period, we minus the days that are out of range
                            if (enableDate > Period)
                            {
                                DataRow[] temprows = dtRoomDay.Select(string.Format("ClientID = {0} AND RoomID = {1} AND EvtDate < '{2}'", clientId, roomId, enableDate));
                                modifiedPhysicalDays -= temprows.Length;
                            }
                            if (disableDate < Period.AddMonths(1).AddDays(-1))
                            {
                                DataRow[] temprows = dtRoomDay.Select(string.Format("ClientID = {0} AND RoomID = {1} AND EvtDate > '{2}'", clientId, roomId, disableDate));
                                modifiedPhysicalDays -= temprows.Length;
                            }

                            //At this point, we have right PhysicalDays value (account in full month of partial month),
                            //now we need to get the AccoutDays, which is not affected by the life time of accounts association
                            DataRow[] rowsToolDay = dtToolDay.Select(string.Format("ClientID = {0} AND RoomID = {1} AND AccountID = {2}", clientId, roomId, accountId));

                            if (rowsToolDay.Length == 1)
                            {
                                //user has use this account for x amount of days in tool reservations
                                accountDays = Convert.ToInt32(rowsToolDay[0]["AccountDays"]);
                            }
                            else
                            {
                                //user never make reservation on tools using this account
                                accountDays = 0;
                            }

                            //We also keep monthly room charge, this could save us a lot of time later on, and we also guarantee data persistance in future 
                            if (roomId == 6) //Only Clean Room allows monthly charge
                            {
                                if (billingTypeId == BillingType.Int_Ga || billingTypeId == BillingType.ExtAc_Ga)
                                    totalMonthlyRoomCharge = 875;
                                else if (billingTypeId == BillingType.Int_Si || billingTypeId == BillingType.ExtAc_Si)
                                    totalMonthlyRoomCharge = 1315;
                                else
                                    totalMonthlyRoomCharge = 0;
                            }
                            else
                            {
                                totalMonthlyRoomCharge = 0;
                            }

                            //Case 1: for people who has only one account
                            if (numberOfAccountsPerClient == 1)
                            {
                                chargeDays = physicalDays; //we shouldn't use modifiedPhysicalDays here because we have to charge everything on this account if only account is disabled
                                entries = totalEntries;
                                hours = totalHours;
                            }
                            else
                            {
                                //Multiple accounts during this period

                                //Get default apportion value for this client/room/account
                                DataRow[] rowDefaultApportion = dtDefault.Select(string.Format("ClientID = {0} AND RoomID = {1} AND AccountID = {2}", clientId, roomId, accountId));
                                if (rowDefaultApportion.Length == 1)
                                    defaultPercentage = Convert.ToDouble(rowDefaultApportion[0]["Percentage"]) / 100D;
                                else
                                {
                                    //no default value, so we have to treat it as zeor always
                                    //either this is a new account added to user in this period
                                    //or the user has no done any default apportionment before
                                    defaultPercentage = 0;
                                }

                                //because it's possible to have default AccountDays larger than Physical days, so we have to figure out who is the bigger value
                                //e.g. user can reserve and use several tools without stepping into the lab
                                if (modifiedPhysicalDays > accountDays)
                                {
                                    maxDays = modifiedPhysicalDays;
                                    minDays = accountDays;
                                }
                                else
                                {
                                    maxDays = accountDays;
                                    minDays = modifiedPhysicalDays;
                                }

                                if (defaultPercentage > 0)
                                {
                                    //multiple accounts users who don't have any accounts change would be satisfied by this part of code
                                    chargeDays = Convert.ToInt32(maxDays * defaultPercentage);
                                    entries = totalEntries * defaultPercentage;
                                    hours = totalHours * defaultPercentage;
                                }
                                else
                                {
                                    //it's new account, we assume zero first
                                    chargeDays = 0;
                                    entries = 0;
                                    hours = 0;
                                }

                                //no matter what, ChargeDays cannot be less than AccountDays
                                if (chargeDays < accountDays)
                                    chargeDays = accountDays;

                            }

                            DataRow newRow = dtResult.NewRow();
                            newRow["Period"] = Period;
                            newRow["ClientID"] = clientId;
                            newRow["RoomID"] = roomId;
                            newRow["AccountID"] = accountId;
                            newRow["ChargeTypeID"] = arow["ChargeTypeID"];
                            newRow["BillingTypeID"] = billingTypeId;
                            newRow["PhysicalDays"] = modifiedPhysicalDays;
                            newRow["ChargeDays"] = chargeDays;
                            newRow["AccountDays"] = accountDays;
                            newRow["Entries"] = entries;
                            newRow["Hours"] = hours;
                            newRow["isDefault"] = true;
                            newRow["MonthlyRoomCharge"] = totalMonthlyRoomCharge;

                            //We store the rate, it's for convenience reason, since we know rate hardly change
                            DataRow[] costrows = dtRoomCost.Select(string.Format("ChargeTypeID = {0} AND RecordID = {1}", arow["ChargeTypeID"], roomId));
                            if (costrows.Length > 0)
                            {
                                newRow["RoomRate"] = costrows[0]["MulVal"];
                                newRow["EntryRate"] = costrows[0]["AddVal"];
                            }
                            else
                            {
                                newRow["RoomRate"] = 0;
                                newRow["EntryRate"] = 0;
                            }

                            //temporary column
                            newRow["Percentage"] = defaultPercentage;
                            dtResult.Rows.Add(newRow);

                        } //end of account table loop

                        //delete all the remote accounts again because the next room needs clean data
                        foreach (int accID in arr)
                        {
                            DataRow[] drowsAccountRemote = dtAccount.Select(string.Format("ClientID = {0} AND AccountID = {1}", clientId, accID));
                            drowsAccountRemote[0].Delete();
                        }
                        arr.Clear();

                        //Apportionment value optimization for multiple accounts users
                        if (numberOfAccountsPerClient > 1)
                        {
                            //Make sure for each person at each room has met the minimum days requirement, as well as the sum of each entries is equal to TotalEntries

                            //Days re-calculation
                            DataRow[] rows = dtResult.Select(string.Format("ClientID = {0} AND RoomID = {1}", clientId, roomId));

                            int totalChargeDays = Convert.ToInt32(dtResult.Compute("SUM(ChargeDays)", string.Format("ClientID = {0} AND RoomID = {1}", clientId, roomId)));
                            int offDays = physicalDays - totalChargeDays;
                            if (offDays > 0) //we cannot allow ChargeDays be less than Physical Days
                            {
                                //since we know we need to add more days to ChargeDays at this moment, so we have to find out the proper distribution
                                //of days on current accounts.  We find out the real percentage among all accounts.  Then we divide individual account
                                //with this real Total Percentage
                                double realTotalPercentage = Convert.ToDouble(dtResult.Compute("SUM(Percentage)", string.Format("ClientID = {0} AND RoomID = {1}", clientId, roomId)));

                                if (realTotalPercentage <= 0)
                                {
                                    //divide equally, it means no default apportion on any of accounts
                                    foreach (DataRow drow in rows)
                                    {
                                        drow["ChargeDays"] = Convert.ToDouble(drow["ChargeDays"]) + RoundUp(Convert.ToDouble(offDays) / Convert.ToDouble(rows.Length));

                                        //we have to do a final check to make sure the charge days cannot be greater than the bigger of physicaldays or accountdays
                                        if (Convert.ToInt32(drow["ChargeDays"]) > Convert.ToInt32(drow["PhysicalDays"]) && Convert.ToInt32(drow["ChargeDays"]) > Convert.ToInt32(drow["AccountDays"]))
                                        {
                                            if (Convert.ToInt32(drow["PhysicalDays"]) > Convert.ToInt32(drow["AccountDays"]))
                                                drow["ChargeDays"] = drow["PhysicalDays"];
                                            else
                                                drow["ChargeDays"] = drow["AccountDays"];
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (DataRow drow in rows)
                                    {
                                        //We can only add days to non-zero accounts, so percentage zero accounts are skipped
                                        if (Convert.ToDouble(drow["Percentage"]) > 0)
                                            drow["ChargeDays"] = Convert.ToInt32(drow["ChargeDays"]) + RoundUp(Convert.ToDouble(offDays) * (Convert.ToDouble(drow["Percentage"]) / realTotalPercentage));
                                    }
                                }
                            }

                            //Entries and Hours re-calculation - the idea here is both are apportioned according to the ChargeDays, so we cannot do anything about Entries until ChargeDays are correct
                            totalChargeDays = Convert.ToInt32(dtResult.Compute("SUM(ChargeDays)", string.Format("ClientID = {0} AND RoomID = {1}", clientId, roomId)));
                            double final_percentage = 0D;
                            foreach (DataRow drow in rows)
                            {
                                if (totalChargeDays > 0)
                                {
                                    if (totalChargeDays > 0)
                                    {
                                        final_percentage = Convert.ToDouble(drow["ChargeDays"]) / Convert.ToDouble(totalChargeDays);
                                        drow["Entries"] = totalEntries * final_percentage;
                                        drow["Hours"] = totalHours * final_percentage;
                                    }
                                    else
                                    {
                                        drow["Entries"] = 0;
                                        drow["Hours"] = 0;
                                    }
                                }
                                else
                                {
                                    drow["Entries"] = totalEntries;
                                    drow["Hours"] = totalHours;
                                }

                            }

                            //MonthlyRoomCharge recalculation
                            //Remember that mutliple org has different charge, so we must calculate it separately
                            //Apply only to clean room
                            if (roomId == 6)
                            {
                                int[] array = { 5, 15, 25 };

                                //we lool three times based on chargetype, and for each charge type, we find out the monthly fee independently
                                foreach (int _chargeTypeID in array)
                                {
                                    DataRow[] temprows = dtResult.Select(string.Format("RoomID = {0} AND ClientID = {1} AND ChargeTypeID = {2}", 6, clientId, _chargeTypeID));

                                    if (temprows.Length > 0)
                                    {
                                        totalMonthlyRoomCharge = Convert.ToDouble(temprows[0]["MonthlyRoomCharge"]); //IMPORTANT - the code above will assign the correct fee amount for each monthly account, so it's all the same among the same org type
                                        totalChargeDays = Convert.ToInt32(dtResult.Compute("SUM(ChargeDays)", string.Format("RoomID = {0} AND ClientID = {1} AND ChargeTypeID = {2}", 6, clientId, _chargeTypeID)));

                                        foreach (DataRow dr in temprows)
                                        {
                                            if (totalChargeDays > 0)
                                            {
                                                final_percentage = Convert.ToDouble(dr["ChargeDays"]) / Convert.ToDouble(totalChargeDays);
                                                dr["MonthlyRoomCharge"] = totalMonthlyRoomCharge * final_percentage;
                                            }
                                            else
                                            {
                                                dr["MonthlyRoomCharge"] = 0;
                                            }
                                        }
                                    }
                                }
                            }

                        } //client has multiple accounts
                    } //if this room is being used by the user
                } //end of room table loop
            } //end of user table loop

            //Store data back to DB
            if (dtResult.Rows.Count > 0)
            {
                SaveNewApportionDatatoDB(dtResult);
            }
        }

        /// <summary>
        /// Get all the necessary tables to do the monhtly apportionment processing
        /// </summary>
        /// <param name="period"></param>
        /// <returns>7 tables so far</returns>
        /// <remarks></remarks>
        private DataSet GetRequiredDataFromDB(DateTime period)
        {
            using (var dba = DA.Current.GetAdapter())
                return dba.ApplyParameters(new { Period = period }).FillDataSet("RoomApportionmentInDaysMonthly_Populate");
        }

        /// <summary>
        /// Get the schema of Apportionment table, we don't want any data
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private DataTable GetApportionmentTableSchema()
        {
            using (var dba = DA.Current.GetAdapter())
            {
                return dba.ApplyParameters(new
                {
                    Action = "ForApportion",
                    Period = DateTime.Now,
                    ClientID = -1,
                    RoomID = -1
                }
                ).FillDataTable("RoomApportionmentInDaysMonthly_Select");
            }
        }

        private void SaveNewApportionDatatoDB(DataTable dtIn)
        {
            using (var dba = DA.Current.GetAdapter())
            {
                //Insert prepration - it's necessary because we may have to add new account that is a remote account
                dba.InsertCommand
                    .AddParameter("@Period", SqlDbType.DateTime)
                    .AddParameter("@ClientID", SqlDbType.Int)
                    .AddParameter("@RoomID", SqlDbType.Int)
                    .AddParameter("@AccountID", SqlDbType.Int)
                    .AddParameter("@ChargeTypeID", SqlDbType.Int)
                    .AddParameter("@BillingTypeID", SqlDbType.Int)
                    .AddParameter("@ChargeDays", SqlDbType.Int)
                    .AddParameter("@PhysicalDays", SqlDbType.Int)
                    .AddParameter("@AccountDays", SqlDbType.Int, 4)
                    .AddParameter("@Entries", SqlDbType.Float, 8)
                    .AddParameter("@Hours", SqlDbType.Float, 8)
                    .AddParameter("@isDefault", SqlDbType.Bit)
                    .AddParameter("@RoomRate", SqlDbType.Float)
                    .AddParameter("@EntryRate", SqlDbType.Float)
                    .AddParameter("@MonthlyRoomCharge", SqlDbType.Float);

                bool debug = false;

                if (dba.UpdateDataTable(dtIn, "RoomApportionmentInDaysMonthly_Insert") >= 0)
                {
                    if (debug && ServiceProvider.Current.Email != null)
                        ServiceProvider.Current.Email.SendMessage(0, "LNF.CommonTools.ApportionmentInDaysMonthlyProcessor.SaveNewApportionDatatoDB(DataTable dtIn)", string.Format("Processing Apportionment Successful - saving to database [{0}]", DateTime.Now), string.Empty, SendEmail.SystemEmail, SendEmail.DeveloperEmails);
                }
                else
                {
                    if (debug && ServiceProvider.Current.Email != null)
                        ServiceProvider.Current.Email.SendMessage(0, "LNF.CommonTools.ApportionmentInDaysMonthlyProcessor.SaveNewApportionDatatoDB(DataTable dtIn)", string.Format("Error in Processing Apportionment - saving to database [{0}]", DateTime.Now), string.Empty, SendEmail.SystemEmail, SendEmail.DeveloperEmails);
                }
            }
        }

        private int RoundUp(double valueIn)
        {
            int a = Convert.ToInt32(valueIn);
            if (valueIn > Convert.ToDouble(a))
                return a + 1;
            else
                return a;
        }
    }
}
