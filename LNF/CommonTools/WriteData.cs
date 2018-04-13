using LNF.Logging;
using LNF.Models.Billing;
using LNF.Repository;
using System;

namespace LNF.CommonTools
{
    public class WriteData
    {
        //Room functions
        [Obsolete("Use WriteRoomDataManager directly.")]
        public void WriteRoomData(DateTime sDate, DateTime eDate, int clientId = 0, int roomId = 0)
        {
            WriteRoomDataManager.Create(sDate, eDate, clientId, roomId).WriteRoomData();
        }

        [Obsolete("Use WriteRoomDataManager directly.")]
        public void WriteRoomDataClean(DateTime sDate, DateTime eDate, bool delete = true, int clientId = 0, int roomId = 0)
        {
            WriteRoomDataManager.Create(sDate, eDate, clientId, roomId).WriteRoomDataClean(delete);
        }

        //Store functions
        [Obsolete("Use WriteStoreDataManager directly.")]
        public void WriteStoreData(DateTime sDate, DateTime eDate, int clientId = 0, int itemId = 0)
        {
            WriteStoreDataManager.Create(sDate, eDate, clientId, itemId).WriteStoreData();
        }

        [Obsolete("Use WriteStoreDataManager directly.")]
        public void WriteStoreDataClean(DateTime sDate, DateTime eDate, int clientId = 0, int itemId = 0)
        {
            WriteStoreDataManager.Create(sDate, eDate, clientId, itemId).WriteStoreDataClean();
        }

        //Tool functions
        [Obsolete("Use WriteToolDataManager directly.")]
        public void WriteToolData(DateTime sDate, DateTime eDate, int clientId = 0, int resourceId = 0)
        {
            WriteToolDataManager.Create(sDate, eDate, clientId, resourceId).WriteToolData();
        }

        [Obsolete("Use WriteToolDataManager directly.")]
        public void WriteToolDataClean(DateTime sDate, DateTime eDate, int clientId = 0, int resourceId = 0)
        {
            WriteToolDataManager.Create(sDate, eDate, clientId, resourceId).WriteToolDataClean();
        }

        //Update all tables
        public void UpdateTables(string[] types, int clientId, int recordId, DateTime eDate, UpdateDataType updateTypes, bool delete, bool isDailyImport)
        {
            // isDailyImport means that we are importing a single day's worth of data. So eDate should be 00:00:00 of the current day and
            // sDate will be the max 

            //ISN'T THIS WHAT DATABASE TRANSACTIONS ARE FOR? If you want atomic database interaction use a transaction! Why reinvent the wheel?

            //before doing anything, make sure that we have exclusive write access to the tables
            //By Wen: This code doesn't work, the lock retrieving mechanism here is not atomic.
            //By Wen: we need fix this part
            //bool bHasLock = false;
            //SqlCommand cmdGetLock = new SqlCommand("DataUpdate_Lock", cnSselData);
            //cmdGetLock.CommandType = CommandType.StoredProcedure;
            //cmdGetLock.Parameters.AddWithValue("@Action", "Lock");

            //while (!bHasLock)
            //{
            //    cnSselData.Open();
            //    bHasLock = Convert.ToBoolean(cmdGetLock.ExecuteScalar());
            //    cnSselData.Close();
            //    if (!bHasLock)
            //        System.Threading.Thread.Sleep(5000);
            //}

            //currently, the types are "Tool", "Room", "Store", hardcoded in the Scheduler Serivce

            foreach (string t in types)
            {
                string recordParm = string.Empty;
                switch (t)
                {
                    case "Room":
                        recordParm = "@RoomID";
                        break;
                    case "Tool":
                        recordParm = "@ResourceID";
                        break;
                    case "Store":
                        recordParm = "@ItemID";
                        break;
                }

                //Clean data always first, then final data
                string[] dTypes = null;
                switch (updateTypes)
                {
                    case UpdateDataType.DataClean:
                        dTypes = new string[] { "Clean" };
                        break;
                    case UpdateDataType.Data:
                        dTypes = new string[] { "" };
                        break;
                    case UpdateDataType.DataClean | UpdateDataType.Data:
                        dTypes = new string[] { "Clean", "" };
                        break;
                }

                DateTime sDate;
                string funcName;

                foreach (string dType in dTypes)
                {
                    sDate = DateTime.MinValue;
                    funcName = string.Empty;

                    using (LogTaskTimer.Start("WriteData.UpdateTables", "sDate = {0}, eDate = {1}, isDailyImport = {2}, dType = {3}, funcName = {4}", () => new object[] { sDate, eDate, isDailyImport, dType, funcName }))
                    {
                        string dataType = string.Format("{0}Data{1}", t, dType); //e.g. StoreData, StoreDataClean OR RoomData, RoomDataClean
                        funcName = "Write" + dataType; //e.g. WriteStoreData, WriteStoreDataClean OR WriteRoomData, WriteRoomDataClean

                        //always do clean first (called from within the select)
                        if (dType == "Clean")
                        {
                            using (var dba = DA.Current.GetAdapter())
                            {
                                //This store procedure will delete all records that are of today
                                //and will return the max date after the deletion (so the return date should be yesterday)

                                string procName = string.Format("{0}_Select", dataType);

                                sDate = dba.SelectCommand
                                    .AddParameter("@Action", "LastUpdate")
                                    .AddParameterIf("@ClientID", clientId > 0, clientId)
                                    .AddParameterIf(recordParm, recordId > 0, recordId)
                                    .ExecuteScalar<DateTime>(procName);

                                if (sDate == default(DateTime))
                                    throw new Exception(string.Format("Cannot get sDate from {0}", procName));
                            }

                            //[2015-02-06 jg]
                            //  Adding one day makes sense in the context of ToolData, RoomData, and StoreData because these tables do not have date *and time*
                            //  fields like BeginDateTime and EntryDT. They only have the ActDate column which is always at midnight. So this makes sure
                            //  that sDate, which we know already exists in the table, is not re-imported. However when we are processing the daily import
                            //  sDate and eDate will be equal at this point so we need to decrease sDate by one day so the full previous day is imported.
                            sDate = sDate.AddDays(1).Date;

                            if (isDailyImport && sDate == eDate)
                                sDate = sDate.AddDays(-1);

                            //isDailyImport is only ever set by the scheduler service
                            //  this is needed because an intermediate update would cause sDate to be equal to eDate
                        }
                        else
                        {
                            // For Data always import the whole period.
                            sDate = eDate.FirstOfMonth();
                            eDate = sDate.AddMonths(1);
                        }

                        //2007-03-07 If it's from Scheduler Service, eDate is always today, if it's from user application,
                        //  then the eDate is always the first day of next month

                        //2007-03-08 Just realized that it's possible to have eDate = Now from user app as well, such as aggSumUsage

                        DateTime lDate = new DateTime(sDate.Year, sDate.Month, 1); //sDate is either today or yesterday if it's from scheduler service
                        DateTime sd, ed;

                        //while (lDate <= eDate)
                        while (lDate < eDate) // [2016-02-24 jg] I think this is correct because for Data, lDate and eDate will be equal on the 2nd pass which doesn't make sense.
                        {
                            sd = sDate;
                            ed = eDate;

                            if (lDate.AddMonths(1) < eDate)
                                ed = lDate.AddMonths(1);

                            if (lDate > sDate)
                                sd = lDate;

                            try
                            {
                                switch (funcName)
                                {
                                    case "WriteRoomDataClean":
                                        WriteRoomDataManager.Create(sd, ed, clientId, recordId).WriteRoomDataClean(delete);
                                        break;
                                    case "WriteRoomData":
                                        WriteRoomDataManager.Create(sd, ed, clientId, recordId).WriteRoomData();
                                        break;
                                    case "WriteToolDataClean":
                                        WriteToolDataManager.Create(sd, ed, clientId, recordId).WriteToolDataClean();
                                        break;
                                    case "WriteToolData":
                                        WriteToolDataManager.Create(sd, ed, clientId, recordId).WriteToolData();
                                        break;
                                    case "WriteStoreDataClean":
                                        WriteStoreDataManager.Create(sd, ed, clientId, recordId).WriteStoreDataClean();
                                        break;
                                    case "WriteStoreData":
                                        WriteStoreDataManager.Create(sd, ed, clientId, recordId).WriteStoreData();
                                        break;
                                }
                            }
                            catch (Exception ex)
                            {
                                // 2010-02-03 Test code to track who calls this function
                                // [2016-09-28 jg] Only call when there's an error
                                string body = string.Empty;

                                if (clientId != 0 && recordId != 0)
                                    body = string.Format("[{0}] function = {1}, ClientID = {2}, RecordID = {3}, sDate = {4}, eDate = {5}", DateTime.Now, funcName, clientId, recordId, sd, ed);
                                else
                                    body = string.Format("[{0}] function = {1}, sDate = {2}, eDate = {3}", DateTime.Now, funcName, sd, ed);

                                body += Environment.NewLine + Environment.NewLine + ex.ToString();

                                ServiceProvider.Current.Email.SendMessage(0,
                                    "LNF.CommonTools.WriteData.UpdateTables(string[] types, int clientId, int recordId, DateTime eDate, UpdateDataType updateTypes, bool delete, bool isDailyImport)",
                                    string.Format("Call from CommonTools.WriteData.UpdateTable [{0}] [{1}]", t + dType, DateTime.Now),
                                    body,
                                    SendEmail.SystemEmail,
                                    SendEmail.DeveloperEmails,
                                    isHtml: false
                                );
                            }

                            lDate = lDate.AddMonths(1);
                        }
                    }
                }
            }
        }

        private bool CompareUpdateDataTypes(UpdateDataType type1, UpdateDataType type2)
        {
            return (type1 & type2) > 0;
        }
    }
}