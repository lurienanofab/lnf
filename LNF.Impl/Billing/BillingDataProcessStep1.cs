using LNF.Billing;
using LNF.Billing.Process;
using LNF.CommonTools;
using LNF.Data;
using LNF.DataAccess;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace LNF.Impl.Billing
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

    public class Step1Config
    {
        public SqlConnection Connection { get; set; }
        public string Context { get; set; }
        public DateTime Period { get; set; }
        public DateTime Now { get; set; }
        public int ClientID { get; set; }
        public bool IsTemp { get; set; }
    }

    //This is the main class to process the billing information since 2009-07-01
    //This class will popuate the RoomBilling, ToolBilling, StoreBilling and all associated temporary tables
    public class BillingDataProcessStep1 : ReaderBase
    {
        protected IToolBillingRepository ToolBilling { get; }

        private readonly Step1Config _config;

        public BillingDataProcessStep1(Step1Config cfg) : base(cfg.Connection)
        {
            _config = cfg;
        }

        public DateTime Period => _config.Period;
        public DateTime Now => _config.Now;
        public int ClientID => _config.ClientID;
        public bool IsTemp => _config.IsTemp;

        #region Room Billing
        public const string FOR_PARENT_ROOMS = "ForParentRooms";

        ///<summary>
        ///The main process that loads data into the RoomBilling table.
        ///Note: This table is called RoomApportionmentInDaysMonthly.
        ///</summary>
        public PopulateRoomBillingResult PopulateRoomBilling()
        {
            var result = new PopulateRoomBillingResult
            {
                UseParentRooms = GlobalSettings.Current.UseParentRooms,

                //Before saving to DB, we have to delete the old data in the same period
                //This must be done first because PopulateRoomBilling is now a two step process
                RowsDeleted = DeleteRoomBillingData()
            };

            DataSet ds;
            DataTable dt;

            ds = GetRoomData();
            dt = LoadRoomBilling(ds);
            result.RowsExtracted = dt.Rows.Count;
            result.RowsLoaded = SaveRoomBillingData(dt);

            if (result.UseParentRooms)
            {
                ds = GetRoomData(FOR_PARENT_ROOMS);
                dt = LoadRoomBilling(ds);
                result.RowsExtractedForParentRooms = dt.Rows.Count;
                result.RowsLoadedForParentRooms = SaveRoomBillingData(dt);
                UpdateEntries(ds.Tables[1]);
            }

            return result;
        }

        private void UpdateEntries(DataTable dt)
        {
            // This is the same code that is called when apportionment is saved (step1).
            // Call it here to set intial values.
            var repo = new LNF.Billing.Apportionment.Repository(Connection);
            foreach(DataRow dr in dt.Rows)
            {
                var roomId = dr.Field<int>("RoomID");
                repo.UpdateChildRoomEntryApportionment(Period, ClientID, roomId);
            }
        }

        public DataTable LoadRoomBilling(DataSet dsSource)
        {
            //Step 1: Get users and rooms for this period and loop through each of them
            //Step 1.1: Get all the necessary data from database to save the round trip cost to DB connection
            //The code below will get a total of 11 tables
            DataTable dtUser = dsSource.Tables[0]; //all users who used the lab
            DataTable dtRoom = dsSource.Tables[1]; //all rooms we can bill in this period
            DataTable dtAccount = dsSource.Tables[2]; //all active accounts for each user in table #0
            DataTable dtRoomDay = dsSource.Tables[3]; //RoomDataDayView data - aggreate based on daily data of room usage
            DataTable dtRoomMonth = dsSource.Tables[4]; //RoomDataMonthView data - aggregate based on monthly data of room usage (the view column TotalDaysPerMonth is aliased PhysicalDays
            DataTable dtToolDayByAcct = dsSource.Tables[5]; //ToolDataDayView - get to know accounts used for tool, we get AccountDays from this table
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

            dtAccount.Columns.Add("IsGrowerObserver", typeof(bool), string.Format("BillingTypeID = {0}", BillingTypes.Grower_Observer));

            int cid = 0;
            int rid = 0;
            int accountId = 0;
            int chargeTypeId = 0;
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
            if (ClientID == 0)
                userRows = dtUser.Select("1 = 1");
            else
                userRows = dtUser.Select($"ClientID = {ClientID}");

            foreach (DataRow urow in userRows)
            {
                cid = urow.Field<int>("ClientID");
                DataRow[] drowsAccountOriginal = dtAccount.Select($"ClientID = {cid}");

                numberOfAccountsPerClient = drowsAccountOriginal.Length;

                foreach (DataRow rrow in dtRoom.Rows)
                {
                    rid = rrow.Field<int>("RoomID");
                    DataRow[] rowsRoomMonth = dtRoomMonth.Select($"ClientID = {cid} AND RoomID = {rid}");

                    if (rowsRoomMonth.Length > 0)
                    {
                        physicalDays = rowsRoomMonth[0].Field<int>("PhysicalDays");
                        totalHours = Convert.ToDecimal(rowsRoomMonth[0].Field<double>("TotalHoursPerMonth"));

                        //hard code the roomID here.  No better alternative [really??]
                        int[] napRooms = { 2, 4 };
                        if (napRooms.Contains(rid))
                        {
                            DataRow[] rowsRoomClean = dtRoomDataClean.Select($"ClientID = {cid} AND RoomID = {rid}");
                            if (rowsRoomClean.Length == 0)
                                totalEntries = 0; // how is this possible?
                            else
                                totalEntries = rowsRoomClean[0].Field<int>("TotalEntriesForNAPRoom");
                        }
                        else
                            totalEntries = Convert.ToDecimal(rowsRoomMonth[0].Field<double>("TotalEntriesPerMonth"));

                        //2009-07-21 remote processing accounts need to be added as well. Remote processing is room specific,
                        //  so we have to use RoomID as a filter and allows remote accounts based on the room
                        DataRow[] rowsUsedAccountInTool = dtAccountsUsedInTool.Select($"ClientID = {cid} AND RoomID = {rid}");
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

                                int aid = dr.Field<int>("AccountID");
                                DataRow[] drsClientRemote = dtClientRemote.Select($"ClientID = {cid} AND AccountID = {aid}");

                                if (drsClientRemote.Length > 0)
                                    ndr["BillingTypeID"] = BillingTypes.Remote;
                                else
                                    ndr["BillingTypeID"] = BillingTypes.RegularException;

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
                        bool isGrowerObserver = drowsAccountWithRemote.Any(x => x.Field<int>("BillingTypeID") == BillingTypes.Grower_Observer);
                        int numberOfOrgs = drowsAccountWithRemote.Select(x => x.Field<int>("OrgID")).Distinct().Count();

                        //if (isGrowerObserver && numberOfOrgs > 1)
                        //    throw new InvalidOperationException(string.Format("A grower/observer has multiple orgs but this is not allowed. [Period = {0:yyyy-MM-dd}, ClientID = {1}, RoomID = {2}]", period, cid, roomId));

                        foreach (DataRow arow in drowsAccountWithRemote)
                        {
                            int orgId = arow.Field<int>("OrgID");

                            if (!orgsPerClient.Contains(orgId))
                                orgsPerClient.Add(orgId);

                            accountId = arow.Field<int>("AccountID");
                            chargeTypeId = arow.Field<int>("ChargeTypeID");
                            billingTypeId = arow.Field<int>("BillingTypeID");

                            // At this point, we have right physicalDays value (account in full month of partial month),
                            // now we need to get the accoutDays, which is not affected by the life time of accounts association
                            DataRow[] rowsToolDay = dtToolDayByAcct.Select(string.Format("ClientID = {0} AND RoomID = {1} AND AccountID = {2}", cid, rid, accountId));

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

                                //ChargeDays = IIf(AccountDays > PhysicalDays, PhysicalDays, AccountDays)
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
                                DataRow[] rowDefaultApportion = dtDefault.Select(string.Format("ClientID = {0} AND RoomID = {1} AND AccountID = {2}", cid, rid, accountId));
                                if (rowDefaultApportion.Length > 0)
                                {
                                    // [2021-03-05 jg] In case there are multiple entires use the last one because this will be the most recently added.
                                    var lastRow = rowDefaultApportion.Last();
                                    defaultPercentage = Convert.ToDouble(lastRow["Percentage"]) / 100D;
                                }
                                else
                                {
                                    // No default value, so we have to treat it as zero always
                                    // either this is a new account added to user in this period
                                    // or the user has not done any default apportionment before.
                                    defaultPercentage = 0;
                                }
                            }

                            DataRow newRow = dtResult.NewRow();
                            newRow["Period"] = Period;
                            newRow["ClientID"] = cid;
                            newRow["RoomID"] = rid;
                            newRow["AccountID"] = accountId;
                            newRow["OrgID"] = orgId;
                            newRow["ChargeTypeID"] = chargeTypeId;
                            newRow["BillingTypeID"] = billingTypeId;

                            // Grower/Observer would have different physical days

                            // [2013-05-14 jg] Removed AccountDays > 0. On May 23, 2012 I added this but I'm not sure why. I did not
                            // comment the code or add a note in subversion. I'm removing this now because it causes problems with
                            // Grower/Observer charges. A Grower/Observer who enters the lab but has no tool usage is still charged
                            // because AccountDays = 0 in this case. Therefore the code inside the if never executes (i.e. PhysicalDays
                            // and ChargeDays are never set to 0).

                            //if (BillingTypeID == BillingType.GetID("Grower/Observer") && AccountDays > 0) //<- removed AccountDays > 0
                            if (billingTypeId == BillingTypes.Grower_Observer)
                            {
                                DataRow[] rowsToolUsageData = dtToolUsageData.Select(string.Format("ClientID = {0} AND RoomID = {1}", cid, rid));
                                if (rowsToolUsageData.Length > 0)
                                {
                                    physicalDays = rowsToolUsageData[0].Field<int>("PhysicalDays");
                                    chargeDays = physicalDays;
                                }
                                else
                                {
                                    //2010-08 for Organics bay, we always charge no matter what
                                    if (rid != Rooms.OrganicsBay.RoomID)
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
                            DataRow[] costrows = dtRoomCost.Select(string.Format("ChargeTypeID = {0} AND RecordID = {1}", chargeTypeId, rid));
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

                            DataRow[] resultRows = dtResult.Select(string.Format("ClientID = {0} AND RoomID = {1}", cid, rid));

                            //We have to see if there are data in RoomBillingUserApportionData.  if we do, we must use those values
                            foreach (DataRow drow in resultRows)
                            {
                                DataRow[] userDataRows = dtUserApportionData.Select(string.Format("ClientID = {0} AND RoomID = {1} AND AccountID = {2}", cid, rid, drow["AccountID"]));
                                if (userDataRows.Length == 1)
                                    drow["ChargeDays"] = userDataRows[0]["ChargeDays"];
                            }

                            int totalChargeDays = Convert.ToInt32(dtResult.Compute("SUM(ChargeDays)", string.Format("ClientID = {0} AND RoomID = {1}", cid, rid)));

                            // totalChargeDays is the sum of charge days for each account. This can be greater than physicalDays, for example
                            // if two accounts both have a reservation on the same day totalChargeDays will be 2 and physicalDays will be 1

                            offDays = physicalDays - totalChargeDays;

                            if (offDays > 0) //we cannot allow ChargeDays be less than Physical Days
                            {
                                //since we know we need to add more days to ChargeDays at this moment, so we have to find out the proper distribution
                                //of days on current accounts.  We find out the real percentage among all accounts.  Then we divide individual account
                                //with this real Total Percentage
                                decimal realTotalPercentage = Convert.ToDecimal(dtResult.Compute("SUM(Percentage)", string.Format("ClientID = {0} AND RoomID = {1}", cid, rid)));

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
                                    int totalChargeDaysPerOrg = Convert.ToInt32(dtResult.Compute("SUM(ChargeDays)", string.Format("ClientID = {0} AND RoomID = {1} AND OrgID = {2}", cid, rid, orgId)));
                                    offDays = physicalDays - totalChargeDaysPerOrg;

                                    if (offDays < 0)
                                    {
                                        //We have to reduce the offDays
                                        resultRows = dtResult.Select(string.Format("ClientID = {0} AND RoomID = {1} AND OrgID = {2}", cid, rid, orgId));
                                        decimal totalPercentage = Convert.ToDecimal(dtResult.Compute("SUM(Percentage)", string.Format("ClientID = {0} AND RoomID = {1} AND OrgID = {2}", cid, rid, orgId)));

                                        if (totalPercentage > 0)
                                        {
                                            foreach (DataRow frow in resultRows)
                                            {
                                                decimal percentage = Convert.ToDecimal(frow.Field<double>("Percentage"));
                                                if (percentage > 0)
                                                {
                                                    //frow("ChargeDays") -= CType(offDays * -1, Double) * (frow("Percentage") / TotalPercentage)
                                                    decimal value = frow.Field<decimal>("ChargeDays");
                                                    decimal adjustment = -offDays * (percentage / totalPercentage);
                                                    frow["ChargeDays"] = value - adjustment;

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
                            totalChargeDays = Convert.ToInt32(dtResult.Compute("SUM(ChargeDays)", string.Format("ClientID = {0} AND RoomID = {1}", cid, rid)));
                            decimal finalPercentage = 0;

                            foreach (DataRow drow in resultRows)
                            {
                                //Users can now apportion room entries so get any previously saved data
                                DataRow[] userDataRows = dtUserApportionData.Select(string.Format("ClientID = {0} AND RoomID = {1} AND AccountID = {2}", cid, rid, drow["AccountID"]));

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
                                    if (drow.Field<int>("BillingTypeID") == BillingTypes.Grower_Observer)
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

                                        ExceptionManager exp = new ExceptionManager { TimeStamp = Now, ExpName = "User has zero charge days", AppName = typeof(BillingDataProcessStep1).Assembly.GetName().Name, FunctionName = "CommonTools-PopulateRoomBilling" };
                                        exp.CustomData = $"ClientID = {cid}, Period = '{Period}'";
                                        exp.LogException(Connection);
                                    }
                                }
                            }
                        } //if accounts per client > 1
                    } //if this room is being used by the user
                } //end of room table loop
            } //end of user table loop

            return dtResult;
        }

        /// <summary>
        /// Get all the necessary tables to do the monthly apportionment processing
        /// </summary>
        public DataSet GetRoomData(string option = null)
        {
            //Don't pass clientId here, always return everything even if we are only running this for one client.
            //This is better than modifying the stored procedure.
            using (var cmd = new SqlCommand("dbo.RoomApportionmentInDaysMonthly_Populate", Connection) { CommandType = CommandType.StoredProcedure })
            using (var adap = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("Period", Period);
                cmd.Parameters.AddWithValue("Option", option);

                var ds = new DataSet();
                adap.Fill(ds);

                return ds;
            }
        }

        public int DeleteRoomBillingData()
        {
            string proc = (IsTemp) ? "dbo.RoomBillingTemp_Delete" : "dbo.RoomApportionmentInDaysMonthly_Delete";

            using (var cmd = new SqlCommand(proc, Connection) { CommandType = CommandType.StoredProcedure })
            {
                AddParameter(cmd, "Action", "DeleteCurrentRange", SqlDbType.NVarChar, 50);
                AddParameter(cmd, "Period", Period, SqlDbType.DateTime);
                AddParameterIf(cmd, "ClientID", ClientID > 0, ClientID, SqlDbType.Int);
                AddParameter(cmd, "Context", _config.Context, SqlDbType.NVarChar, 50);
                var result = cmd.ExecuteNonQuery();
                return result;
            }
        }

        public int SaveRoomBillingData(DataTable dtIn)
        {
            int count = 0;

            //Store data back to DB
            if (dtIn.Rows.Count > 0)
            {
                //Insert prepration - it's necessary because we may have to add new account that is a remote account

                var proc = (IsTemp) ? "dbo.RoomBillingTemp_Insert" : "dbo.RoomApportionmentInDaysMonthly_Insert";

                using (var insert = new SqlCommand(proc, Connection) { CommandType = CommandType.StoredProcedure })
                using (var adap = new SqlDataAdapter { InsertCommand = insert })
                {
                    insert.Parameters.Add("Period", SqlDbType.DateTime, 0, "Period");
                    insert.Parameters.Add("ClientID", SqlDbType.Int, 0, "ClientID");
                    insert.Parameters.Add("RoomID", SqlDbType.Int, 0, "RoomID");
                    insert.Parameters.Add("AccountID", SqlDbType.Int, 0, "AccountID");
                    insert.Parameters.Add("ChargeTypeID", SqlDbType.Int, 0, "ChargeTypeID");
                    insert.Parameters.Add("BillingTypeID", SqlDbType.Int, 0, "BillingTypeID");
                    insert.Parameters.Add("OrgID", SqlDbType.Int, 0, "OrgID");
                    insert.Parameters.Add("ChargeDays", SqlDbType.Float, 0, "ChargeDays");
                    insert.Parameters.Add("PhysicalDays", SqlDbType.Float, 0, "PhysicalDays");
                    insert.Parameters.Add("AccountDays", SqlDbType.Float, 0, "AccountDays");
                    insert.Parameters.Add("Entries", SqlDbType.Float, 0, "Entries");
                    insert.Parameters.Add("Hours", SqlDbType.Float, 0, "Hours");
                    insert.Parameters.Add("isDefault", SqlDbType.Bit, 0, "isDefault");
                    insert.Parameters.Add("RoomRate", SqlDbType.Float, 0, "RoomRate");
                    insert.Parameters.Add("EntryRate", SqlDbType.Float, 0, "EntryRate");
                    insert.Parameters.Add("MonthlyRoomCharge", SqlDbType.Float, 0, "MonthlyRoomCharge");

                    count = adap.Update(dtIn);
                }

                bool debug = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["Debug"]) && Convert.ToBoolean(ConfigurationManager.AppSettings["Debug"]);

                if (debug)
                {
                    string subj;

                    if (count >= 0)
                        subj = $"Processing Apportionment Successful - saving to database - {proc} [{Now:yyyy-MM-dd HH:mm:ss}]";
                    else
                        subj = $"Error in Processing Apportionment - saving to database - {proc} [{Now:yyyy-MM-dd HH:mm:ss}]";

                    SendEmail.SendDeveloperEmail("LNF.CommonTools.BillingDataProcessStep1.SaveRoomBillingData", subj);
                }
            }

            return count;
        }
        #endregion

        #region ToolBilling
        public PopulateToolBillingResult PopulateToolBilling()
        {
            var result = new PopulateToolBillingResult
            {
                Period = Period,
                ClientID = ClientID,
                IsTemp = IsTemp
            };

            IToolBilling[] source = GetToolData(0);

            result.RowsExtracted = source.Length;

            foreach (var tb in source)
                ToolBillingUtility.CalculateToolBillingCharges(tb);

            //Delete appropriate data
            result.RowsDeleted = DeleteToolBillingData();

            //Insert new rows
            result.RowsLoaded = InsertToolBillingData(source);

            return result;
        }

        //Get source data from ToolData
        public IToolBilling[] GetToolData(int reservationId)
        {
            // Must use a DataTable here because the stored proc returns new ToolBilling rows, without ToolBillingID, which causes problems
            var reader = new ToolDataReader(Connection);
            var dt = reader.ReadToolData(Period, ClientID, reservationId);
            var source = ToolBillingUtility.CreateToolBillingFromDataTable(dt, IsTemp).ToArray();
            return source;
        }

        private int DeleteToolBillingData()
        {
            string proc = (IsTemp) ? "ToolBillingTemp_Delete" : "ToolBilling_Delete";

            using (var cmd = new SqlCommand(proc, Connection) { CommandType = CommandType.StoredProcedure })
            {
                AddParameter(cmd, "Action", "DeleteCurrentRange", SqlDbType.NVarChar, 50);
                AddParameter(cmd, "Period", Period, SqlDbType.DateTime);
                AddParameterIf(cmd, "ClientID", ClientID > 0, ClientID, SqlDbType.Int);
                AddParameter(cmd, "Context", _config.Context, SqlDbType.NVarChar, 50);
                var result = cmd.ExecuteNonQuery();
                return result;
            }
        }

        private int InsertToolBillingData(IEnumerable<IToolBilling> items)
        {
            DataTable dt = CreateToolBillingTable();
            FillToolBillingDataTable(dt, items);

            using (var bcp = CreateToolBillingBulkCopy())
                bcp.WriteToServer(dt);

            return dt.Rows.Count;
        }
        #endregion

        #region StoreBilling
        public PopulateStoreBillingResult PopulateStoreBilling()
        {
            var result = new PopulateStoreBillingResult
            {
                Period = Period,
                IsTemp = IsTemp
            };

            var dt = GetStoreData();

            result.RowsExtracted = dt.Rows.Count;

            //Delete appropriate data
            result.RowsDeleted = DeleteStoreBillingData();

            //Save to ToolBilling Table
            result.RowsLoaded = SaveStoreBillingData(dt);

            return result;
        }

        public DataTable GetStoreData()
        {
            //Get data from ToolData table
            DataSet dsRaw = GetNecessaryDataTablesForStoreUsage();
            DataTable dtSource = dsRaw.Tables[0];
            DataTable dtNew = dsRaw.Tables[1];
            DataTable dtCost = dsRaw.Tables[2];

            //Add data to new table
            DataRow newRow;
            int chargeTypeId;

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

            return dtNew;
        }

        private DataSet GetNecessaryDataTablesForStoreUsage()
        {
            using (var cmd = new SqlCommand("dbo.StoreData_Select", Connection) { CommandType = CommandType.StoredProcedure })
            using (var adap = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("Action", "ForStoreBillingGeneration");
                cmd.Parameters.AddWithValue("Period", Period);

                var ds = new DataSet();
                adap.Fill(ds);

                return ds;
            }
        }

        private int DeleteStoreBillingData()
        {
            string proc = IsTemp ? "dbo.StoreBillingTemp_Delete" : "dbo.StoreBilling_Delete";

            using (var cmd = new SqlCommand(proc, Connection) { CommandType = CommandType.StoredProcedure })
            {
                AddParameter(cmd, "Period", Period, SqlDbType.DateTime);
                AddParameter(cmd, "Context", _config.Context, SqlDbType.NVarChar, 50);
                var result = cmd.ExecuteNonQuery();
                return result;
            }
        }

        private int SaveStoreBillingData(DataTable dtIn)
        {
            //Insert prepration - it's necessary because we may have to add new account that is a remote account

            string proc = IsTemp ? "dbo.StoreBillingTemp_Insert" : "dbo.StoreBilling_Insert";

            using (var insert = new SqlCommand(proc, Connection) { CommandType = CommandType.StoredProcedure })
            using (var adap = new SqlDataAdapter { InsertCommand = insert })
            {
                insert.Parameters.Add("Period", SqlDbType.DateTime, 0, "Period");
                insert.Parameters.Add("ClientID", SqlDbType.Int, 0, "ClientID");
                insert.Parameters.Add("AccountID", SqlDbType.Int, 0, "AccountID");
                insert.Parameters.Add("ChargeTypeID", SqlDbType.Int, 0, "ChargeTypeID");
                insert.Parameters.Add("ItemID", SqlDbType.Int, 0, "ItemID");
                insert.Parameters.Add("StatusChangeDate", SqlDbType.DateTime, 0, "StatusChangeDate");
                insert.Parameters.Add("Quantity", SqlDbType.Float, 0, "Quantity");
                insert.Parameters.Add("UnitCost", SqlDbType.Float, 0, "UnitCost");
                insert.Parameters.Add("CategoryID", SqlDbType.Int, 0, "CategoryID");
                insert.Parameters.Add("CostMultiplier", SqlDbType.Float, 0, "CostMultiplier");

                int count = adap.Update(dtIn);

                bool debug = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["Debug"]) && Convert.ToBoolean(ConfigurationManager.AppSettings["Debug"]);

                if (debug)
                {
                    string subj;

                    if (count >= 0)
                        subj = $"Processing StoreBilling Successful - saving to database - {proc} [{Now:yyyy-MM-dd HH:mm:ss}]";
                    else
                        subj = $"Error in Processing StoreBilling - saving to database portion failed - {proc} [{Now:yyyy-MM-dd HH:mm:ss}]";

                    SendEmail.SendDeveloperEmail("LNF.CommonTools.BillingDataProcessStep1.SaveStoreBillingData", subj);
                }

                return count;
            }
        }

        private DataTable CreateToolBillingTable()
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

        private void FillToolBillingDataTable(DataTable dt, IEnumerable<IToolBilling> items)
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

        private IBulkCopy CreateToolBillingBulkCopy()
        {
            IBulkCopy bcp = new DefaultBulkCopy(IsTemp ? "dbo.ToolBillingTemp" : "dbo.ToolBilling");
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
