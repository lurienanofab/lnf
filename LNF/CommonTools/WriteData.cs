using LNF.Models.Billing;
using LNF.Repository;
using System;

namespace LNF.CommonTools
{
    public class WriteData
    {
        //Update all tables
        public void UpdateTables(string[] types, UpdateDataType updateTypes, DateTime period, int clientId)
        {
            // isDailyImport means that we are importing a single day's worth of data.
            // So eDate should be 00:00:00 of the current day and sDate will be the max 

            foreach (string t in types)
            {
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

                DateTime sDate, eDate;
                string funcName;

                foreach (string dType in dTypes)
                {
                    sDate = DateTime.MinValue;
                    funcName = string.Empty;

                    string dataType = $"{t}Data{dType}"; //e.g. StoreData, StoreDataClean OR RoomData, RoomDataClean
                    funcName = "Write" + dataType; //e.g. WriteStoreData, WriteStoreDataClean OR WriteRoomData, WriteRoomDataClean

                    //always do clean first (called from within the select)
                    if (dType == "Clean")
                    {
                        //This stored procedure will delete all records that are of today
                        //and will return the max date after the deletion (so the return date should be yesterday)

                        string procName = $"{dataType}_Select";
                        sDate = DA.Command().Param("Action", "LastUpdate").Param("ClientID", clientId > 0, clientId).ExecuteScalar<DateTime>(procName);

                        if (sDate == default(DateTime))
                            throw new Exception($"Cannot get sDate from {procName}");

                        // [2018-09-10 jg] Now just pass in the period so this is much simpler
                        sDate = sDate.AddDays(1).Date;
                        eDate = period.FirstOfMonth().AddMonths(1).Date;
                    }
                    else
                    {
                        // For Data always import the whole period.
                        sDate = period.FirstOfMonth();
                        eDate = sDate.AddMonths(1);
                    }

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
                                    new WriteRoomDataCleanProcess(sDate, eDate, clientId).Start();
                                    break;
                                case "WriteRoomData":
                                    new WriteRoomDataProcess(sDate, clientId, 0).Start();
                                    break;
                                case "WriteToolDataClean":
                                    new WriteToolDataCleanProcess(sDate, eDate, clientId).Start();
                                    break;
                                case "WriteToolData":
                                    new WriteToolDataProcess(sDate, clientId, 0).Start();
                                    break;
                                case "WriteStoreDataClean":
                                    new WriteStoreDataCleanProcess(sDate, eDate, clientId).Start();
                                    break;
                                case "WriteStoreData":
                                    new WriteStoreDataProcess(sd, clientId, 0).Start();
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            // 2010-02-03 Test code to track who calls this function
                            // [2016-09-28 jg] Only call when there's an error
                            string body = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] function = {funcName}, ClientID = {clientId}, sd = {sd}, ed = {ed}"
                                + Environment.NewLine + Environment.NewLine + ex.ToString();

                            string subj = $"Call from CommonTools.WriteData.UpdateTable [{t + dType}] [{DateTime.Now:yyyy-MM-dd HH:mm:ss}]";

                            ServiceProvider.Current.Email.SendMessage(
                                clientId: 0,
                                caller: "LNF.CommonTools.WriteData.UpdateTables",
                                subject: subj,
                                body: body,
                                from: SendEmail.SystemEmail,
                                to: SendEmail.DeveloperEmails,
                                isHtml: false
                            );
                        }

                        lDate = lDate.AddMonths(1);
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