using LNF.Billing;
using LNF.Logging;
using LNF.Repository;
using LNF.Repository.Billing;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;

namespace LNF.CommonTools
{
    public struct RoomBillingAmounts
    {
        public readonly decimal PhysicalDays;
        public readonly decimal Entries;
        public readonly decimal Hours;

        public RoomBillingAmounts(decimal physicalDays, decimal entries, decimal hours)
        {
            PhysicalDays = physicalDays;
            Entries = entries;
            Hours = hours;
        }
    }

    //This is the main class to process the billing information since 2009-07-01
    //This class will popuate the RoomBilling, ToolBilling, StoreBilling and all associated temporary tables
    public static class BillingDataProcessStep1
    {
        #region Room Billing
        private const string FOR_PARENT_ROOMS = "ForParentRooms";

        ///<summary>
        ///The main process that loads data into the RoomBilling table.
        ///Note: This table is called RoomApportionmentInDaysMonthly.
        ///</summary>
        public static void PopulateRoomBilling(DateTime period, int clientId, bool isTemp)
        {
            bool useParentRooms = bool.Parse(ConfigurationManager.AppSettings["UseParentRooms"]);

            int count = 0;

            //Before saving to DB, we have to delete the old data in the same period
            //This must be done first because PopulateRoomBilling is now a two step process
            count = DeleteRoomBillingData(period, clientId, isTemp);

            DataSet ds;
            DataTable dt;

            ds = GetDataForRoomBilling(period);
            dt = LoadRoomBilling(ds, period, clientId, isTemp);
            count = SaveRoomBillingData(dt, isTemp);

            if (useParentRooms)
            {
                ds = GetDataForRoomBilling(period, FOR_PARENT_ROOMS);
                dt = LoadRoomBilling(ds, period, clientId, isTemp);
                count = SaveRoomBillingData(dt, isTemp);
            }
        }

        public static DataTable LoadRoomBilling(DataSet dsSource, DateTime period, int clientId, bool isTemp)
        {
            int count = 0;

            using (var timer = LogTaskTimer.Start("BillingDataProcessStep1.LoadRoomBilling", "period = '{0:yyyy-MM-dd}', clientId = {1}, isTemp = {2}, count = {3}", () => new object[] { period, clientId, isTemp, count }))
            {
                //Step 1: Get users and rooms for this period and loop through each of them
                //Step 1.1: Get all the necessary data from database to save the round trip cost to DB connection
                //The code below will get a total of 11 tables
                DataTable dtUser = dsSource.Tables[0]; //all users who used the lab
                DataTable dtRoom = dsSource.Tables[1]; //all rooms we can bill in this period
                DataTable dtAccount = dsSource.Tables[2]; //all active accounts for each user in table #0
                DataTable dtRoomDay = dsSource.Tables[3]; //RoomDataDayView data - aggreate based on daily data of room usage
                DataTable dtRoomMonth = dsSource.Tables[4]; //RoomDataMonthView data - aggregate based on monthly data of room usage
                DataTable dtToolDayByAcct = dsSource.Tables[5]; //ToolDataDayView - get to know accounts used for tool
                DataTable dtDefault = dsSource.Tables[6]; //Default apportionment values
                DataTable dtAccountsUsedInTool = dsSource.Tables[7]; //Accounts used in tool usage (Including remote processing accounts)
                DataTable dtRoomCost = dsSource.Tables[8]; //Room Cost table for this period
                DataTable dtRoomDataClean = dsSource.Tables[9]; //RoomDataClean group by clientID and roomID. This shows correct entries for NAP room
                DataTable dtResult = dsSource.Tables[10]; //RoomApportionmentInDaysMonthly empty datatable
                DataTable dtUserApportionData = dsSource.Tables[11]; //RoomBillingUserApportionData for this period
                DataTable dtToolUsageData = dsSource.Tables[12]; //ToolData used for grower/observer 
                DataTable dtClientRemote = dsSource.Tables[13]; //ClientRemote table to check if user is remote processing

                dtResult.Columns.Add("Percentage", typeof(double)); //add this temporary column for convenience in calculating proper ChargeDays value, this will not be pushed to DB
                dtResult.Columns["Entries"].DefaultValue = 0;
                dtResult.Columns["Entries"].AllowDBNull = false;

                dtAccount.Columns.Add("IsGrowerObserver", typeof(bool), string.Format("BillingTypeID = {0}", (int)BillingType.Grower_Observer));

                int cid = 0;
                int roomId = 0;
                int accountId = 0;
                int billingTypeId = 0;
                decimal physicalDays = 0;
                decimal accountDays = 0;
                decimal chargeDays = 0;
                decimal entries = 0;
                decimal hours = 0;
                decimal totalEntries = 0;
                decimal totalHours = 0;
                double totalMonthlyRoomCharge = 0;
                double defaultPercentage = 0;
                int numberOfAccountsPerClient = 0;

                //loop through each user and for each room
                DataRow[] userRows;
                if (clientId == 0)
                    userRows = dtUser.Select("1 = 1");
                else
                    userRows = dtUser.Select(string.Format("ClientID = {0}", clientId));

                foreach (DataRow urow in userRows)
                {
                    cid = urow.Field<int>("ClientID");
                    DataRow[] drowsAccountOriginal = dtAccount.Select(string.Format("ClientID = {0}", cid));

                    numberOfAccountsPerClient = drowsAccountOriginal.Length;

                    foreach (DataRow rrow in dtRoom.Rows)
                    {
                        roomId = rrow.Field<int>("RoomID");
                        DataRow[] rowsRoomMonth = dtRoomMonth.Select(string.Format("ClientID = {0} AND RoomID = {1}", cid, roomId));

                        if (rowsRoomMonth.Length > 0)
                        {
                            physicalDays = rowsRoomMonth[0].Field<int>("PhysicalDays");
                            totalHours = Convert.ToDecimal(rowsRoomMonth[0].Field<double>("TotalHoursPerMonth"));

                            //hard code the roomID here.  No better alternative [really??]
                            int[] napRooms = { 2, 4 };
                            if (napRooms.Contains(roomId))
                            {
                                DataRow[] rowsRoomClean = dtRoomDataClean.Select(string.Format("ClientID = {0} AND RoomID = {1}", cid, roomId));
                                totalEntries = rowsRoomClean[0].Field<int>("TotalEntriesForNAPRoom");
                            }
                            else
                                totalEntries = Convert.ToDecimal(rowsRoomMonth[0].Field<double>("TotalEntriesPerMonth"));

                            //2009-07-21 remote processing accounts need to be added as well. Remote processing is room specific,
                            //  so we have to use RoomID as a filter and allows remote accounts based on the room
                            DataRow[] rowsUsedAccountInTool = dtAccountsUsedInTool.Select(string.Format("ClientID = {0} AND RoomID = {1}", cid, roomId));
                            List<int> remoteAccounts = new List<int>();
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
                                    //not found, so this is probably a remote processing account, we should add it to dtAccount
                                    DataRow ndr = dtAccount.NewRow();
                                    ndr["AccountID"] = dr["AccountID"];
                                    ndr["ClientID"] = cid;
                                    ndr["OrgID"] = dr["OrgID"];
                                    ndr["ChargeTypeID"] = dr["ChargeTypeID"];

                                    DataRow[] drsClientRemote = dtClientRemote.Select(string.Format("ClientID = {0} AND AccountID = {1}", cid, dr["AccountID"]));

                                    if (drsClientRemote.Length > 0)
                                        ndr["BillingTypeID"] = BillingType.Remote;
                                    else
                                        ndr["BillingTypeID"] = BillingType.RegularException;

                                    dtAccount.Rows.Add(ndr);

                                    remoteAccounts.Add(dr.Field<int>("AccountID"));
                                }
                            }

                            //we might have new remote accounts added, so we have to regenerate all the rows with this client
                            // [2017-11-01 jg] need to have any Grower/Observer orgs at the end because physical days will be set to zero, so do them last
                            DataRow[] drowsAccountWithRemote = dtAccount.Select(string.Format("ClientID = {0}", cid), "IsGrowerObserver ASC, OrgID ASC, AccountID ASC");
                            numberOfAccountsPerClient = drowsAccountWithRemote.Length;

                            //This is used to distinguish different orgs so we can do apportionment later on
                            List<int> orgsPerClient = new List<int>();

                            // [2015-12-01 jg] We are currently not handling the case of mulitple orgs when at least one is grower/observer billing
                            // type. Sandrine said that this is not required and would take too much effort to implement. We sould consider it a rule
                            // that a grower/observer will only have one org. If there is more than one org then physicalDays gets set to zero for the
                            // grower/observer org which means the next org will also be zero regardless of which billing type or if there is tool
                            // usage. So the current logic below only works for grower/observers if they have only one org, therefore an exception
                            // is thrown if this is not the case.
                            bool isGrowerObserver = drowsAccountWithRemote.Any(x => x.Field<int>("BillingTypeID") == BillingType.Grower_Observer);
                            int numberOfOrgs = drowsAccountWithRemote.Select(x => x.Field<int>("OrgID")).Distinct().Count();

                            //if (isGrowerObserver && numberOfOrgs > 1)
                            //    throw new InvalidOperationException(string.Format("A grower/observer has multiple orgs but this is not allowed. [Period = {0:yyyy-MM-dd}, ClientID = {1}, RoomID = {2}]", period, cid, roomId));

                            foreach (DataRow arow in drowsAccountWithRemote)
                            {
                                int orgId = arow.Field<int>("OrgID");

                                if (!orgsPerClient.Contains(orgId))
                                    orgsPerClient.Add(orgId);

                                accountId = arow.Field<int>("AccountID");
                                billingTypeId = arow.Field<int>("BillingTypeID");

                                // At this point, we have right physicalDays value (account in full month of partial month),
                                // now we need to get the accoutDays, which is not affected by the life time of accounts association
                                DataRow[] rowsToolDay = dtToolDayByAcct.Select(string.Format("ClientID = {0} AND RoomID = {1} AND AccountID = {2}", cid, roomId, accountId));

                                if (rowsToolDay.Length == 1)
                                {
                                    //user has use this account for x amount of days in tool reservations
                                    accountDays = rowsToolDay.First().Field<int>("AccountDays");
                                }
                                else
                                {
                                    //user never make reservation on tools using this account
                                    accountDays = 0;
                                }

                                // Case 1: for people who has only one account
                                if (numberOfAccountsPerClient == 1)
                                {
                                    chargeDays = physicalDays; //we shouldn't use modifiedPhysicalDays here because we have to charge everything on this account if only account is disabled
                                    entries = totalEntries;
                                    hours = totalHours;
                                }
                                else
                                {
                                    // Multiple accounts during this period

                                    chargeDays = (accountDays > physicalDays) ? physicalDays : accountDays;

                                    // [2015-12-01 jg] It is possible for physicalDays to be zero at this point if the user is a grower/observer and
                                    // only has one org with multiple accounts, and has room usage but no tool usage. In this case physicalDays will
                                    // be set to zero in the Grower/Observer check below (on the first iteration).

                                    entries = 0;
                                    hours = 0;

                                    if (physicalDays > 0)
                                    {
                                        entries = physicalDays == 0 ? 0 : totalEntries * (chargeDays / physicalDays);
                                        hours = physicalDays == 0 ? 0 : totalHours * (chargeDays / physicalDays);
                                    }

                                    // Get default apportion value for this client/room/account
                                    DataRow[] rowDefaultApportion = dtDefault.Select(string.Format("ClientID = {0} AND RoomID = {1} AND AccountID = {2}", cid, roomId, accountId));
                                    if (rowDefaultApportion.Length == 1)
                                        defaultPercentage = Convert.ToDouble(rowDefaultApportion[0]["Percentage"]) / 100D;
                                    else
                                    {
                                        // No default value, so we have to treat it as zero always
                                        // either this is a new account added to user in this period
                                        // or the user has not done any default apportionment before.
                                        defaultPercentage = 0;
                                    }
                                }

                                DataRow newRow = dtResult.NewRow();
                                newRow["Period"] = period;
                                newRow["ClientID"] = cid;
                                newRow["RoomID"] = roomId;
                                newRow["AccountID"] = accountId;
                                newRow["OrgID"] = arow["OrgID"];
                                newRow["ChargeTypeID"] = arow["ChargeTypeID"];
                                newRow["BillingTypeID"] = billingTypeId;

                                // Grower/Observer would have different physical days

                                // [2013-05-14 jg] Removed AccountDays > 0. On May 23, 2012 I added this but I'm not sure why. I did not
                                // comment the code or add a note in subversion. I'm removing this now because it causes problems with
                                // Grower/Observer charges. A Grower/Observer who enters the lab but has no tool usage is still charged
                                // because AccountDays = 0 in this case. Therefore the code inside the if never executes (i.e. PhysicalDays
                                // and ChargeDays are never set to 0).

                                //if (BillingTypeID == BillingType.GetID("Grower/Observer") && AccountDays > 0) //<- removed AccountDays > 0
                                if (billingTypeId == BillingType.Grower_Observer)
                                {
                                    DataRow[] rowsToolUsageData = dtToolUsageData.Select(string.Format("ClientID = {0} AND RoomID = {1}", cid, roomId));
                                    if (rowsToolUsageData.Length > 0)
                                    {
                                        physicalDays = rowsToolUsageData[0].Field<int>("PhysicalDays");
                                        chargeDays = physicalDays;
                                    }
                                    else
                                    {
                                        //2010-08 for Organics bay, we always charge no matter what
                                        if (roomId != (int)Rooms.OrganicsBay)
                                        {
                                            // if there are mulitple accounts these will be zero on the next iteration, don't divide by zero!
                                            physicalDays = 0;
                                            chargeDays = 0;
                                        }
                                    }
                                }

                                newRow["PhysicalDays"] = physicalDays;
                                newRow["ChargeDays"] = chargeDays;
                                newRow["AccountDays"] = accountDays;
                                newRow["Entries"] = entries;
                                newRow["Hours"] = hours;
                                newRow["isDefault"] = true;
                                newRow["MonthlyRoomCharge"] = totalMonthlyRoomCharge; //this is always zero

                                // We store the rate, it's for historical data integrity reason, since we know rate changes over time
                                DataRow[] costrows = dtRoomCost.Select(string.Format("ChargeTypeID = {0} AND RecordID = {1}", arow["ChargeTypeID"], roomId));
                                newRow["RoomRate"] = costrows[0]["MulVal"];
                                newRow["EntryRate"] = costrows[0]["AddVal"];

                                //temporary column
                                newRow["Percentage"] = defaultPercentage;
                                dtResult.Rows.Add(newRow);

                            } //end of account table loop (drowsAccountWithRemote)

                            //delete all the remote accounts again because the next room needs clean data
                            foreach (int remoteAccountId in remoteAccounts)
                            {
                                DataRow[] drowsAccountRemote = dtAccount.Select(string.Format("ClientID = {0} AND AccountID = {1}", cid, remoteAccountId));
                                drowsAccountRemote[0].Delete();
                            }
                            remoteAccounts.Clear();

                            //Apportionment value optimization for multiple accounts users
                            if (numberOfAccountsPerClient > 1)
                            {
                                decimal offDays = 0;

                                DataRow[] resultRows = dtResult.Select(string.Format("ClientID = {0} AND RoomID = {1}", cid, roomId));

                                //We have to see if there are data in RoomBillingUserApportionData.  if we do, we must use those values
                                foreach (DataRow drow in resultRows)
                                {
                                    DataRow[] userDataRows = dtUserApportionData.Select(string.Format("ClientID = {0} AND RoomID = {1} AND AccountID = {2}", cid, roomId, drow["AccountID"]));
                                    if (userDataRows.Length == 1)
                                        drow["ChargeDays"] = userDataRows[0]["ChargeDays"];
                                }

                                int totalChargeDays = Convert.ToInt32(dtResult.Compute("SUM(ChargeDays)", string.Format("ClientID = {0} AND RoomID = {1}", cid, roomId)));
                                int totalPhysicalDays = Convert.ToInt32(dtResult.Compute("SUM(PhysicalDays)", string.Format("", cid, roomId)));
                                offDays = physicalDays - totalChargeDays;

                                if (offDays > 0) //we cannot allow ChargeDays be less than Physical Days
                                {
                                    //since we know we need to add more days to ChargeDays at this moment, so we have to find out the proper distribution
                                    //of days on current accounts.  We find out the real percentage among all accounts.  Then we divide individual account
                                    //with this real Total Percentage
                                    decimal realTotalPercentage = Convert.ToDecimal(dtResult.Compute("SUM(Percentage)", string.Format("ClientID = {0} AND RoomID = {1}", cid, roomId)));

                                    if (realTotalPercentage <= 0)
                                    {
                                        //divide equally, it means no default apportion on any of accounts
                                        foreach (DataRow drow in resultRows)
                                        {
                                            drow["ChargeDays"] = drow.Field<decimal>("ChargeDays") + offDays / resultRows.Length;
                                        }
                                    }
                                    else
                                    {
                                        foreach (DataRow drow in resultRows)
                                        {
                                            //We can only add days to non-zero accounts, so percentage zero accounts are skipped
                                            decimal pct = Convert.ToDecimal(drow.Field<double>("Percentage"));
                                            if (pct > 0)
                                                drow["ChargeDays"] = drow.Field<decimal>("ChargeDays") + (offDays * (pct / realTotalPercentage));
                                        }
                                    }
                                }
                                else if (offDays < 0)
                                {
                                    //Chargedays are more than physical days

                                    //Now we have to check each org and make sure no single org pays more than the physical days
                                    foreach (int orgId in orgsPerClient)
                                    {
                                        int totalChargeDaysPerOrg = Convert.ToInt32(dtResult.Compute("SUM(ChargeDays)", string.Format("ClientID = {0} AND RoomID = {1} AND OrgID = {2}", cid, roomId, orgId)));
                                        offDays = physicalDays - totalChargeDaysPerOrg;

                                        if (offDays < 0)
                                        {
                                            //We have to reduce the offDays
                                            resultRows = dtResult.Select(string.Format("ClientID = {0} AND RoomID = {1} AND OrgID = {2}", cid, roomId, orgId));
                                            decimal TotalPercentage = Convert.ToDecimal(dtResult.Compute("SUM(Percentage)", string.Format("ClientID = {0} AND RoomID = {1} AND OrgID = {2}", cid, roomId, orgId)));

                                            if (TotalPercentage > 0)
                                            {
                                                foreach (DataRow frow in resultRows)
                                                {
                                                    decimal pct = Convert.ToDecimal(frow.Field<double>("Percentage"));
                                                    if (pct > 0)
                                                    {
                                                        frow["ChargeDays"] = frow.Field<decimal>("ChargeDays") - (offDays * -1) * (pct / TotalPercentage);

                                                        if (frow.Field<decimal>("ChargeDays") < 0)
                                                            frow["ChargeDays"] = 0;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                //divide equally, since there is no more clue
                                                //first, we need to find out how many rows have data
                                                int numOfRowsHaveData = 0;
                                                foreach (DataRow frow in resultRows)
                                                {
                                                    if (Convert.ToDouble(frow["ChargeDays"]) > 0)
                                                        numOfRowsHaveData += 1;
                                                }

                                                decimal leftover = 0;
                                                foreach (DataRow frow in resultRows)
                                                {
                                                    if (Convert.ToDouble(frow["ChargeDays"]) > 0)
                                                    {
                                                        frow["ChargeDays"] = frow.Field<decimal>("ChargeDays") - (offDays * -1) / numOfRowsHaveData;
                                                        frow["ChargeDays"] = frow.Field<decimal>("ChargeDays") - leftover;
                                                        leftover = 0;
                                                        if (Convert.ToDouble(frow["ChargeDays"]) < 0)
                                                        {
                                                            leftover = frow.Field<decimal>("ChargeDays") * -1;
                                                            frow["ChargeDays"] = 0;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                //Entries and Hours re-calculation - the idea here is both are apportioned according to the ChargeDays, so we cannot do anything about Entries until ChargeDays are correct
                                totalChargeDays = Convert.ToInt32(dtResult.Compute("SUM(ChargeDays)", string.Format("ClientID = {0} AND RoomID = {1}", cid, roomId)));
                                decimal finalPercentage = 0;

                                foreach (DataRow drow in resultRows)
                                {
                                    //Users can now apportion room entries so get any previously saved data
                                    DataRow[] userDataRows = dtUserApportionData.Select(string.Format("ClientID = {0} AND RoomID = {1} AND AccountID = {2}", cid, roomId, drow["AccountID"]));

                                    if (totalChargeDays > 0)
                                    {
                                        finalPercentage = drow.Field<decimal>("ChargeDays") / totalChargeDays;

                                        if (userDataRows.Length == 1)
                                        {
                                            drow.SetField("Entries", userDataRows.First().Field<double?>("Entries"));
                                        }
                                        else
                                            drow.SetField("Entries", totalEntries * finalPercentage);

                                        drow.SetField("Hours", totalHours * finalPercentage);
                                    }
                                    else
                                    {
                                        if (drow.Field<int>("BillingTypeID") == BillingType.Grower_Observer)
                                        {
                                            //It's possible to have zero charge days for observer/growser
                                            if (userDataRows.Length == 1)
                                                drow.SetField("Entries", userDataRows.First().Field<double>("Entries"));
                                            else
                                                drow.SetField("Entries", totalEntries / resultRows.Length);

                                            drow.SetField("Hours", totalHours / resultRows.Length);
                                        }
                                        else
                                        {
                                            //It's possible to have zero charge days for observer/grower
                                            drow.SetField("Entries", 0);
                                            drow.SetField("Hours", 0);

                                            ExceptionManager exp = new ExceptionManager { TimeStamp = DateTime.Now, ExpName = "User has zero charge days", AppName = typeof(BillingDataProcessStep1).Assembly.GetName().Name, FunctionName = "CommonTools-PopulateRoomBilling" };
                                            exp.CustomData = string.Format("ClientID = {0}, Period = '{1}'", cid, period);
                                            exp.LogException();
                                        }
                                    }
                                }
                            } //if accounts per client > 1
                        } //if this room is being used by the user
                    } //end of room table loop
                } //end of user table loop

                count = dtResult.Rows.Count;

                return dtResult;
            }
        }

        #region Possible replacement code for RoomBilling
        /// <summary>
        /// Creates parent room records in the RoomBilling data (the RoomApportionmentInDaysMonthly table)
        /// </summary>
        private static void PopulateRoomBillingParentRooms(DateTime period, bool isTemp, int clientId = 0, bool delete = true)
        {
            int count = 0;

            using (LogTaskTimer.Start("BillingDataProcessStep1.PopulateRoomBillingParentRooms", "period = '{0:yyyy-MM-dd}', isTemp = {1}, clientId = {2}, delete = {3}, count = {4}", () => new object[] { period, isTemp, clientId, delete, count }))
            {
                Room[] parents = DA.Current.Query<Room>().Where(x => x.ParentID == 0 && x.Billable && x.Active).ToArray();

                List<IRoomBilling> parentRoomBilling = new List<IRoomBilling>();

                List<IRoomBilling> temp;

                foreach (Room p in parents)
                {
                    int[] children = DA.Current.Query<Room>().Where(x => x.ParentID == p.RoomID).Select(x => x.RoomID).ToArray();

                    if (children.Length > 0)
                    {
                        if (delete)
                        {
                            //delete existing rows
                            IRoomBilling[] existing = GetExistingRoomBilling(period, isTemp, p.RoomID);
                            DA.Current.Delete(existing);
                        }

                        IRoomBilling[] roomBilling;

                        if (isTemp)
                        {
                            if (clientId == 0)
                                roomBilling = DA.Current.Query<RoomBillingTemp>().Where(x => x.Period == period && children.Contains(x.RoomID)).ToArray();
                            else
                                roomBilling = DA.Current.Query<RoomBillingTemp>().Where(x => x.Period == period && x.ClientID == clientId && children.Contains(x.RoomID)).ToArray();
                        }
                        else
                        {
                            if (clientId == 0)
                                roomBilling = DA.Current.Query<RoomBilling>().Where(x => x.Period == period && children.Contains(x.RoomID)).ToArray();
                            else
                                roomBilling = DA.Current.Query<RoomBilling>().Where(x => x.Period == period && x.ClientID == clientId && children.Contains(x.RoomID)).ToArray();
                        }

                        int[] clients = roomBilling.Select(x => x.ClientID).Distinct().ToArray();

                        foreach (int c in clients)
                        {
                            temp = new List<IRoomBilling>();
                            Client client = DA.Current.Single<Client>(c);
                            int[] accounts = roomBilling.Where(x => x.ClientID == client.ClientID).Select(x => x.AccountID).Distinct().ToArray();

                            foreach (int a in accounts)
                            {
                                BillingType bt = DA.Current.Single<BillingType>(roomBilling.First(x => x.ClientID == c && x.AccountID == a).BillingTypeID);
                                temp.Add(GetRoomBilling(p, client, DA.Current.Single<Account>(a), bt, period, isTemp));
                            }

                            if (temp.Count > 0)
                            {
                                //amounts need to be distrubuted between the user's accounts
                                DistributeRoomBillingAmounts(temp, p, client, period, isTemp);
                                parentRoomBilling.AddRange(temp);
                            }
                        }
                    }
                }

                DA.Current.Insert(parentRoomBilling);

                count = parentRoomBilling.Count;
            }
        }

        private static IRoomBilling[] GetExistingRoomBilling(DateTime period, bool isTemp, int roomId)
        {
            if (isTemp)
                return DA.Current.Query<RoomBilling>().Where(x => x.Period == period && x.RoomID == roomId).ToArray();
            else
                return DA.Current.Query<RoomBillingTemp>().Where(x => x.Period == period && x.RoomID == roomId).ToArray();
        }

        private static void DistributeRoomBillingAmounts(IEnumerable<IRoomBilling> roomBilling, Room room, Client client, DateTime period, bool isTemp)
        {
            RoomBillingAmounts amounts = GetRoomBillingAmounts(room, client, period);
            IRoomBilling[] items = roomBilling.Where(x => x.RoomID == room.RoomID && x.ClientID == client.ClientID && x.Period == period).ToArray();

            //1st distribute evenly (PhysicalDays won't change)
            foreach (var i in items)
            {
                i.PhysicalDays = amounts.PhysicalDays;
                i.Hours = amounts.Hours / items.Length;
                i.Entries = amounts.Entries / items.Length;
            }

            //2nd look at user default settings
            ApportionmentDefault[] defs = DA.Current.Query<ApportionmentDefault>().Where(x => x.Client == client && x.Room == room).ToArray();
            if (defs.Length > 0)
            {
                foreach (var i in items)
                {
                    decimal pct = 0;
                    var d = defs.FirstOrDefault(x => x.Account.AccountID == i.AccountID);
                    if (d != null) pct = d.Percentage / 100;
                    i.ChargeDays = amounts.PhysicalDays * pct;
                    i.Hours = amounts.Hours * pct;
                    i.Entries = amounts.Entries * pct;
                }
            }
            else
            {
                //no defaults - distribute charge days evenly
                foreach (var i in items)
                {
                    i.ChargeDays = amounts.PhysicalDays / items.Length;
                }
            }

            //3rd look at perviously apportioned data
            RoomBillingUserApportionData[] apps = DA.Current.Query<RoomBillingUserApportionData>().Where(x => x.Client == client && x.Room == room).ToArray();
            if (apps.Length > 0)
            {
                foreach (var i in items)
                {
                    var a = apps.FirstOrDefault(x => x.Account.AccountID == i.AccountID);
                    if (a != null)
                    {
                        i.ChargeDays = a.ChargeDays;
                        i.Entries = a.Entries;
                    }
                }
            }
        }

        private static RoomBillingAmounts GetRoomBillingAmounts(Room room, Client client, DateTime period)
        {
            //note: these amounts are all independent of Account

            RoomData[] roomData = DA.Current.Query<RoomData>()
                .Where(x => x.Period == period && x.Client == client && x.Room.ParentID == room.RoomID || x.Room.RoomID == room.RoomID).ToArray();

            int physicalDays = roomData.Select(x => x.EvtDate).Distinct().Count();
            decimal entries = roomData.Sum(x => x.Entries);
            decimal hours = roomData.Sum(x => x.Hours);

            return new RoomBillingAmounts(physicalDays, entries, hours);
        }

        private static IRoomBilling GetRoomBilling(Room room, Client client, Account account, BillingType billingType, DateTime period, bool isTemp)
        {
            //leave PhysicalDays, Entries, and Hours all zero, at this point we only want to populate Account dependent data

            ToolData[] toolData = DA.Current.Query<ToolData>()
                .Where(x => x.Period == period && x.ClientID == client.ClientID && x.AccountID == account.AccountID).ToArray();

            Cost[] roomCost = DA.Current.Query<Cost>()
                .Where(x => x.TableNameOrDescription == "RoomCost"
                    && x.RecordID == room.RoomID
                    && x.EffDate < period.AddMonths(1)
                    && x.ChargeTypeID == account.Org.OrgType.ChargeType.ChargeTypeID)
                .ToArray();

            Cost currentCost = roomCost.OrderBy(x => x.EffDate).LastOrDefault();

            int accountDays = toolData.Select(x => x.ActDate).Distinct().Count();
            decimal roomRate = currentCost == null ? 0 : currentCost.MulVal;
            decimal entryRate = currentCost == null ? 0 : currentCost.AddVal;

            IRoomBilling result = null;
            if (isTemp)
                result = new RoomBillingTemp();
            else
                result = new RoomBilling();

            result.Period = period;
            result.ClientID = client.ClientID;
            result.RoomID = room.RoomID;
            result.AccountID = account.AccountID;
            result.ChargeTypeID = account.Org.OrgType.ChargeType.ChargeTypeID;
            result.BillingTypeID = billingType.BillingTypeID;
            result.OrgID = account.Org.OrgID;
            result.ChargeDays = 0;
            result.PhysicalDays = 0;
            result.AccountDays = accountDays;
            result.Entries = 0;
            result.Hours = 0;
            result.IsDefault = true;
            result.RoomRate = roomRate;
            result.EntryRate = entryRate;
            result.MonthlyRoomCharge = 0; //always zero (no longer used?)
            result.RoomCharge = 0; //computed column
            result.EntryCharge = 0; //computed column
            result.SubsidyDiscount = 0;

            return result;
        }
        #endregion

        /// <summary>
        /// Get all the necessary tables to do the monthly apportionment processing
        /// </summary>
        public static DataSet GetDataForRoomBilling(DateTime period, string option = null)
        {
            //Don't pass clientId here, always return everything even if we are only running this for one client.
            //This is better than modifying the stored procedure.

            using (var timer = LogTaskTimer.Start("BillingDataProcessStep1.GetDataForRoomBilling", "period = '{0:yyyy-MM-dd}', option = {1}", () => new object[] { period, option }))
            using (var dba = DA.Current.GetAdapter())
            {
                return dba.ApplyParameters(new { Period = period, Option = option }).FillDataSet("RoomApportionmentInDaysMonthly_Populate");
            }
        }

        /// <summary>
        /// Get the schema of Apportionment table, we don't want any data
        /// </summary>
        private static DataTable GetApportionmentTableSchema()
        {
            using (var dba = DA.Current.GetAdapter())
                return dba.ApplyParameters(new { Action = "ForApportion", Period = DateTime.Now, ClientID = -1, RoomID = -1 }).FillDataTable("RoomApportionmentInDaysMonthly_Select");
        }

        public static int DeleteRoomBillingData(DateTime period, int clientId, bool temp)
        {
            using (var dba = DA.Current.GetAdapter())
            {
                string sp = (temp) ? "RoomBillingTemp_Delete" : "RoomApportionmentInDaysMonthly_Delete";

                object parameters;

                if (clientId == 0)
                    parameters = new { Action = "DeleteCurrentRange", Period = period };
                else
                    parameters = new { Action = "DeleteCurrentRange", Period = period, ClientID = clientId };

                return dba.SelectCommand
                    .ApplyParameters(parameters)
                    .ExecuteNonQuery(sp);
            }
        }

        public static int SaveRoomBillingData(DataTable dtIn, bool temp)
        {
            int result = 0;

            //Store data back to DB
            if (dtIn.Rows.Count > 0)
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
                        .AddParameter("@OrgID", SqlDbType.Int)
                        .AddParameter("@ChargeDays", SqlDbType.Float)
                        .AddParameter("@PhysicalDays", SqlDbType.Float)
                        .AddParameter("@AccountDays", SqlDbType.Float)
                        .AddParameter("@Entries", SqlDbType.Float)
                        .AddParameter("@Hours", SqlDbType.Float)
                        .AddParameter("@isDefault", SqlDbType.Bit)
                        .AddParameter("@RoomRate", SqlDbType.Float)
                        .AddParameter("@EntryRate", SqlDbType.Float)
                        .AddParameter("@MonthlyRoomCharge", SqlDbType.Float);

                    string sp = (temp) ? "RoomBillingTemp_Insert" : "RoomApportionmentInDaysMonthly_Insert";

                    bool debug = false;

                    result = dba.UpdateDataTable(dtIn, sp);

                    if (result >= 0)
                    {
                        if (debug)
                            Providers.Email.SendMessage(0, "LNF.CommonTools.BillingDataProcessStep1.SaveRoomBillingData(DataTable dtIn, bool temp)", string.Format("Processing Apportionment Successful - saving to database - {0} [{1}]", sp, DateTime.Now), string.Empty, SendEmail.SystemEmail, SendEmail.DeveloperEmails);
                    }
                    else
                    {
                        if (debug)
                            Providers.Email.SendMessage(0, "LNF.CommonTools.BillingDataProcessStep1.SaveRoomBillingData(DataTable dtIn, bool temp)", string.Format("Error in Processing Apportionment - saving to database - {0} [{1}]", sp, DateTime.Now), string.Empty, SendEmail.SystemEmail, SendEmail.DeveloperEmails);
                    }
                }
            }

            return result;
        }
        #endregion

        #region ToolBilling
        public static void PopulateToolBilling(DateTime period, int clientId, bool isTemp)
        {
            int rowsSelectedFromToolData = 0;
            int rowsDeletedFromToolBilling = 0;
            int rowsInsertedIntoToolBilling = 0;

            using (LogTaskTimer.Start("BillingDataProcessStep1.PopulateToolBilling", "period = '{0:yyyy-MM-dd}', clientId = {1}, isTemp = {2}, rowsSelectedFromToolData = {3}, rowsDeletedFromToolBilling = {4}, rowsInsertedIntoToolBilling = {5}", () => new object[] { period, clientId, isTemp, rowsSelectedFromToolData, rowsDeletedFromToolBilling, rowsInsertedIntoToolBilling }))
            {
                IToolBilling[] source;

                // Must use a DataTable here because the stored proc returns new ToolBilling rows, without ToolBillingID, which causes problems
                using (var dba = DA.Current.GetAdapter())
                {
                    dba.AddParameter("@Action", "ForToolBilling");
                    dba.AddParameter("@Period", period);
                    dba.AddParameterIf("@ClientID", clientId > 0, clientId);
                    DataTable dt = dba.FillDataTable("Billing.dbo.ToolData_Select");
                    source = dt.AsEnumerable().Select(x => ToolBillingUtility.CreateToolBillingFromDataRow(x, isTemp)).ToArray();
                }

                rowsSelectedFromToolData = source.Count();

                foreach (IToolBilling tb in source)
                    CalculateToolBillingCharges(tb, isTemp);

                //Delete appropriate data
                rowsDeletedFromToolBilling = DeleteToolBillingData(period, isTemp, clientId);

                //Insert new rows
                rowsInsertedIntoToolBilling = InsertToolBillingData(source, isTemp);
            }
        }

        private static void CalculateToolBillingCharges(IToolBilling tb, bool isTemp)
        {
            tb.CalculateReservationFee();
            tb.CalculateUsageFeeCharged();
            tb.CalculateBookingFee();
        }

        //Get source data from ToolData
        private static DataSet GetNecessaryDataTablesForToolUsage(DateTime period)
        {
            using (var dba = DA.Current.GetAdapter())
                return dba.ApplyParameters(new { Action = "ForToolBillingGeneration", Period = period }).FillDataSet("ToolData_Select");
        }

        private static int DeleteToolBillingData(DateTime period, bool temp, int clientId = 0)
        {
            using (var dba = DA.Current.GetAdapter())
            {
                string sp = (temp) ? "ToolBillingTemp_Delete" : "ToolBilling_Delete";
                return dba.SelectCommand
                    .AddParameter("@Action", "DeleteCurrentRange")
                    .AddParameter("@Period", period)
                    .AddParameterIf("@ClientID", clientId > 0, clientId)
                    .ExecuteNonQuery(sp);
            }
        }

        private static int InsertToolBillingData(IEnumerable<IToolBilling> items, bool isTemp)
        {
            DataTable dt = CreateToolBillingTable();
            FillToolBillingDataTable(dt, items);

            using (var bcp = CreateToolBillingBulkCopy(isTemp))
                bcp.WriteToServer(dt);

            return dt.Rows.Count;
        }
        #endregion

        #region StoreBilling
        public static void PopulateStoreBilling(DateTime period, bool temp)
        {
            int rowsSelectedFromSource = 0;
            int rowsDeletedFromStoreBilling = 0;
            int rowsInsertedIntoStoreBilling = 0;

            using (LogTaskTimer.Start("BillingDataProcessStep1.PopulateStoreBilling", "period = '{0:yyyy-MM-dd}', temp = {1}, rowsSelectedFromSource = {2}, rowsDeletedFromStoreBilling = {3}, rowsInsertedIntoStoreBilling = {4}", () => new object[] { period, temp, rowsSelectedFromSource, rowsDeletedFromStoreBilling, rowsInsertedIntoStoreBilling }))
            {
                //Get data from ToolData table
                DataSet dsRaw = GetNecessaryDataTablesForStoreUsage(period);
                DataTable dtSource = dsRaw.Tables[0];
                DataTable dtNew = dsRaw.Tables[1];
                DataTable dtCost = dsRaw.Tables[2];

                //Add data to new table
                DataRow newRow;
                int chargeTypeId;

                rowsSelectedFromSource = dtSource.Rows.Count;

                foreach (DataRow row in dtSource.Rows)
                {
                    chargeTypeId = Convert.ToInt32(row["ChargeTypeID"]);

                    newRow = dtNew.NewRow();

                    newRow["Period"] = row["Period"];
                    newRow["ClientID"] = row["ClientID"];
                    newRow["AccountID"] = row["AccountID"];
                    newRow["ChargeTypeID"] = chargeTypeId;
                    newRow["ItemID"] = row["ItemID"];
                    newRow["StatusChangeDate"] = row["StatusChangeDate"];
                    newRow["Quantity"] = row["Quantity"];
                    newRow["UnitCost"] = row["UnitCost"];
                    newRow["CategoryID"] = row["CategoryID"];

                    DataRow[] temprows = dtCost.Select(string.Format("ChargeTypeID = {0}", chargeTypeId));
                    newRow["CostMultiplier"] = temprows[0]["MulVal"];
                    Array.Clear(temprows, 0, temprows.Length);

                    dtNew.Rows.Add(newRow);
                }

                //Delete appropriate data
                rowsDeletedFromStoreBilling = DeleteStoreBillingData(period, temp);

                //Save to ToolBilling Table
                rowsInsertedIntoStoreBilling = SaveStoreBillingData(dtNew, temp);
            }
        }

        private static DataSet GetNecessaryDataTablesForStoreUsage(DateTime period)
        {
            using (var dba = DA.Current.GetAdapter())
                return dba.ApplyParameters(new { Action = "ForStoreBillingGeneration", Period = period }).FillDataSet("StoreData_Select");
        }

        private static int DeleteStoreBillingData(DateTime period, bool temp)
        {
            using (var dba = DA.Current.GetAdapter())
            {
                string sp = (temp) ? "StoreBillingTemp_Delete" : "StoreBilling_Delete";
                return dba.SelectCommand.ApplyParameters(new { Period = period }).ExecuteNonQuery(sp);
            }
        }

        private static int SaveStoreBillingData(DataTable dtIn, bool temp)
        {
            using (var dba = DA.Current.GetAdapter())
            {
                //Insert prepration - it's necessary because we may have to add new account that is a remote account
                dba.InsertCommand
                    .AddParameter("@Period", SqlDbType.DateTime)
                    .AddParameter("@ClientID", SqlDbType.Int)
                    .AddParameter("@AccountID", SqlDbType.Int)
                    .AddParameter("@ChargeTypeID", SqlDbType.Int)
                    .AddParameter("@ItemID", SqlDbType.Int)
                    .AddParameter("@StatusChangeDate", SqlDbType.DateTime)
                    .AddParameter("@Quantity", SqlDbType.Float)
                    .AddParameter("@UnitCost", SqlDbType.Float)
                    .AddParameter("@CategoryID", SqlDbType.Int)
                    .AddParameter("@CostMultiplier", SqlDbType.Float);

                string sp = (temp) ? "StoreBillingTemp_Insert" : "StoreBilling_Insert";

                bool debug = false;

                int count = dba.UpdateDataTable(dtIn, sp);

                if (count >= 0)
                {
                    if (debug)
                        Providers.Email.SendMessage(0, "LNF.CommonTools.BillingDataProcessStep1.SaveStoreBillingData(DataTable dtIn, bool temp)", string.Format("Processing StoreBilling Successful - saving to database - {0} [{1}]", sp, DateTime.Now), string.Empty, SendEmail.SystemEmail, SendEmail.DeveloperEmails);
                }
                else
                {
                    if (debug)
                        Providers.Email.SendMessage(0, "LNF.CommonTools.BillingDataProcessStep1.SaveStoreBillingData(DataTable dtIn, bool temp)", string.Format("Error in Processing StoreBilling - saving to database portion failed - {0} [{1}]", sp, DateTime.Now), string.Empty, SendEmail.SystemEmail, SendEmail.DeveloperEmails);
                }

                return count;
            }
        }

        private static DataTable CreateToolBillingTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Period", typeof(DateTime));
            dt.Columns.Add("ReservationID", typeof(int));
            dt.Columns.Add("ClientID", typeof(int));
            dt.Columns.Add("AccountID", typeof(int));
            dt.Columns.Add("ChargeTypeID", typeof(int));
            dt.Columns.Add("BillingTypeID", typeof(int));
            dt.Columns.Add("RoomID", typeof(int));
            dt.Columns.Add("ResourceID", typeof(int));
            dt.Columns.Add("ActDate", typeof(DateTime));
            dt.Columns.Add("IsStarted", typeof(bool));
            dt.Columns.Add("IsActive", typeof(bool));
            dt.Columns.Add("IsFiftyPenalty", typeof(bool));
            dt.Columns.Add("ChargeMultiplier", typeof(double));
            dt.Columns.Add("Uses", typeof(double));
            dt.Columns.Add("SchedDuration", typeof(double));
            dt.Columns.Add("ActDuration", typeof(double));
            dt.Columns.Add("ChargeDuration", typeof(double));
            dt.Columns.Add("TransferredDuration", typeof(double));
            dt.Columns.Add("MaxReservedDuration", typeof(double));
            dt.Columns.Add("OverTime", typeof(decimal));
            dt.Columns.Add("RatePeriod", typeof(string));
            dt.Columns.Add("PerUseRate", typeof(decimal));
            dt.Columns.Add("ResourceRate", typeof(decimal));
            dt.Columns.Add("ReservationRate", typeof(decimal));
            dt.Columns.Add("OverTimePenaltyPercentage", typeof(double));
            dt.Columns.Add("UncancelledPenaltyPercentage", typeof(double));
            dt.Columns.Add("UsageFeeCharged", typeof(decimal));
            dt.Columns.Add("BookingFee", typeof(decimal));
            dt.Columns.Add("SubsidyDiscount", typeof(decimal));
            dt.Columns.Add("IsCancelledBeforeAllowedTime", typeof(bool));
            dt.Columns.Add("ReservationFee2", typeof(decimal));
            return dt;
        }

        private static void FillToolBillingDataTable(DataTable dt, IEnumerable<IToolBilling> items)
        {
            foreach (var tb in items)
            {
                var ndr = dt.NewRow();
                ndr.SetField("Period", tb.Period);
                ndr.SetField("ReservationID", tb.ReservationID);
                ndr.SetField("ClientID", tb.ClientID);
                ndr.SetField("AccountID", tb.AccountID);
                ndr.SetField("ChargeTypeID", tb.ChargeTypeID);
                ndr.SetField("BillingTypeID", tb.BillingTypeID);
                ndr.SetField("RoomID", tb.RoomID);
                ndr.SetField("ResourceID", tb.ResourceID);
                ndr.SetField("ActDate", tb.ActDate);
                ndr.SetField("IsStarted", tb.IsStarted);
                ndr.SetField("IsActive", tb.IsActive);
                ndr.SetField("IsFiftyPenalty", tb.IsFiftyPenalty);
                ndr.SetField("ChargeMultiplier", tb.ChargeMultiplier);
                ndr.SetField("Uses", tb.Uses);
                ndr.SetField("SchedDuration", tb.SchedDuration);
                ndr.SetField("ActDuration", tb.ActDuration);
                ndr.SetField("ChargeDuration", tb.ChargeDuration);
                ndr.SetField("TransferredDuration", tb.TransferredDuration);
                ndr.SetField("MaxReservedDuration", tb.MaxReservedDuration);
                ndr.SetField("OverTime", tb.OverTime);
                ndr.SetField("RatePeriod", tb.RatePeriod);
                ndr.SetField("PerUseRate", tb.PerUseRate);
                ndr.SetField("ResourceRate", tb.ResourceRate);
                ndr.SetField("ReservationRate", tb.ReservationRate);
                ndr.SetField("OverTimePenaltyPercentage", tb.OverTimePenaltyPercentage);
                ndr.SetField("UncancelledPenaltyPercentage", tb.UncancelledPenaltyPercentage);
                ndr.SetField("UsageFeeCharged", tb.UsageFeeCharged);
                ndr.SetField("BookingFee", tb.BookingFee);
                ndr.SetField("SubsidyDiscount", tb.SubsidyDiscount);
                ndr.SetField("IsCancelledBeforeAllowedTime", tb.IsCancelledBeforeAllowedTime);
                ndr.SetField("ReservationFee2", tb.ReservationFee2);
                dt.Rows.Add(ndr);
            }
        }

        private static IBulkCopy CreateToolBillingBulkCopy(bool isTemp)
        {
            IBulkCopy bcp = DA.Current.GetBulkCopy(isTemp ? "dbo.ToolBillingTemp" : "dbo.ToolBilling");
            bcp.AddColumnMapping("Period");
            bcp.AddColumnMapping("ReservationID");
            bcp.AddColumnMapping("ClientID");
            bcp.AddColumnMapping("AccountID");
            bcp.AddColumnMapping("ChargeTypeID");
            bcp.AddColumnMapping("BillingTypeID");
            bcp.AddColumnMapping("RoomID");
            bcp.AddColumnMapping("ResourceID");
            bcp.AddColumnMapping("ActDate");
            bcp.AddColumnMapping("IsStarted");
            bcp.AddColumnMapping("IsActive");
            bcp.AddColumnMapping("IsFiftyPenalty");
            bcp.AddColumnMapping("ChargeMultiplier");
            bcp.AddColumnMapping("Uses");
            bcp.AddColumnMapping("SchedDuration");
            bcp.AddColumnMapping("ActDuration");
            bcp.AddColumnMapping("ChargeDuration");
            bcp.AddColumnMapping("TransferredDuration");
            bcp.AddColumnMapping("MaxReservedDuration");
            bcp.AddColumnMapping("OverTime");
            bcp.AddColumnMapping("RatePeriod");
            bcp.AddColumnMapping("PerUseRate");
            bcp.AddColumnMapping("ResourceRate");
            bcp.AddColumnMapping("ReservationRate");
            bcp.AddColumnMapping("OverTimePenaltyPercentage");
            bcp.AddColumnMapping("UncancelledPenaltyPercentage");
            bcp.AddColumnMapping("UsageFeeCharged");
            bcp.AddColumnMapping("BookingFee");
            bcp.AddColumnMapping("SubsidyDiscount");
            bcp.AddColumnMapping("IsCancelledBeforeAllowedTime");
            bcp.AddColumnMapping("ReservationFee2");
            return bcp;
        }
        #endregion
    }

}
