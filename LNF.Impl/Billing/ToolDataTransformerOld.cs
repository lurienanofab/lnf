using System;
using System.Data;

namespace LNF.Impl.Billing
{
    public class ToolDataTransformerOld : ToolDataTransformer
    {
        public ToolDataTransformerOld(DateTime period, int clientId, int resourceId, DataTable dtActivities) : base(period, clientId, resourceId, dtActivities) { }

        public override void ProcessCleanData(DataTable dtToolDataClean)
        {
            if (dtToolDataClean.Rows.Count > 0)
            {
                dtToolDataClean.Columns.Add("Uses", typeof(double));
                dtToolDataClean.Columns.Add("ChargeDuration", typeof(double));
                dtToolDataClean.Columns.Add("ChargeBeginDateTime", typeof(DateTime));
                dtToolDataClean.Columns.Add("ChargeEndDateTime", typeof(DateTime));
                dtToolDataClean.Columns.Add("IsCancelledBeforeAllowedTime", typeof(bool));

                //handled started separately from unstarted
                int toolDataCleanRowCount = dtToolDataClean.Rows.Count;
                for (int i = 0; i < toolDataCleanRowCount; i++)
                {
                    DataRow dr = dtToolDataClean.Rows[i];

                    // ClientID check. dtToolDataClean contains data for all clients even when ClientID > 0 because of transferred duration calculation.
                    if (ClientID == 0 || ClientID == dr.Field<int>("ClientID"))
                    {
                        //only chargeable activities get written to Tooldata
                        if (IsChargeable(dr))
                        {
                            //this means reservation was started
                            double schedDuration = dr.Field<double>("SchedDuration");
                            double actDuration = dr.Field<double>("ActDuration");
                            DateTime beginDateTime = dr.Field<DateTime>("BeginDateTime");
                            DateTime endDateTime = beginDateTime.AddMinutes(schedDuration);
                            DateTime actualBeginDateTime = dr.Field<DateTime>("ActualBeginDateTime");
                            DateTime actualEndDateTime = dr.Field<DateTime>("ActualEndDateTime");

                            dr["IsCancelledBeforeAllowedTime"] = false; // for the old billing method only active reservations were selected so none were cancelled

                            bool isStarted = dr.Field<bool>("IsStarted");
                            DateTime begDate = isStarted ? actualBeginDateTime : beginDateTime;
                            DateTime finDate = isStarted ? actualEndDateTime : endDateTime;

                            var chargeBeginDateTime = begDate;
                            var chargeEndDateTime = finDate;
                            var chargeDuration = (chargeEndDateTime - chargeBeginDateTime).TotalMinutes;
                            dr["ChargeBeginDateTime"] = begDate;
                            dr["ChargeEndDateTime"] = finDate;
                            dr["ChargeDuration"] = chargeDuration;

                            begDate = begDate.Date;
                            finDate = finDate.Date;

                            if (begDate != finDate)
                            {
                                //handle case where resrevation starts and ends on different days
                                double runningOverTime = 0;

                                for (int j = 0; j <= finDate.Subtract(begDate).Days; j++)
                                {
                                    DateTime bDate = begDate.AddDays(j);
                                    DateTime fDate = begDate.AddDays(j + 1);

                                    DataRow adr = dtToolDataClean.NewRow();
                                    adr.ItemArray = dr.ItemArray; //start by copying everything

                                    if (Convert.ToBoolean(dr["IsStarted"]))
                                    {
                                        adr["ActualBeginDateTime"] = (bDate > actualBeginDateTime) ? bDate : actualBeginDateTime;
                                        adr["ActualEndDateTime"] = (fDate < actualEndDateTime) ? fDate : actualEndDateTime;
                                        adr["BeginDateTime"] = adr["ActualBeginDateTime"]; //for convenience below when writing new table
                                        adr["ActDuration"] = adr.Field<DateTime>("ActualEndDateTime").Subtract(adr.Field<DateTime>("ActualBeginDateTime")).TotalMinutes;
                                        adr["Uses"] = adr.Field<double>("ActDuration") / actDuration;

                                        if (adr.Field<DateTime>("ActualEndDateTime") > endDateTime)
                                        {
                                            adr["OverTime"] = adr.Field<DateTime>("ActualEndDateTime").Subtract(endDateTime).TotalMinutes - runningOverTime;
                                            runningOverTime += adr.Field<double>("OverTime");
                                        }
                                        else
                                            adr["OverTime"] = 0D;
                                    }
                                    else
                                    {
                                        adr["BeginDateTime"] = (bDate > beginDateTime) ? bDate : beginDateTime;
                                        adr["EndDateTime"] = (fDate < endDateTime) ? fDate : endDateTime;
                                        adr["SchedDuration"] = adr.Field<DateTime>("EndDateTime").Subtract(adr.Field<DateTime>("BeginDateTime")).TotalMinutes;
                                        adr["ActDuration"] = adr["SchedDuration"]; //should be 0, but the adjust SP would be way too complex
                                        adr["Uses"] = adr.Field<double>("SchedDuration") / schedDuration;
                                    }

                                    adr["ChargeBeginDateTime"] = (bDate > chargeBeginDateTime) ? bDate : chargeBeginDateTime;
                                    adr["ChargeEndDateTime"] = (fDate < chargeEndDateTime) ? fDate : chargeEndDateTime;
                                    adr["ChargeDuration"] = (adr.Field<DateTime>("ChargeEndDateTime") - adr.Field<DateTime>("ChargeBeginDateTime")).TotalMinutes;
                                    adr["MaxReservedDuration"] = GetMaxReservedDuration(adr);

                                    dtToolDataClean.Rows.Add(adr); //will add to end
                                }
                                dr.Delete(); //remove original, multi-day row
                            }
                            else
                            {
                                dr["Uses"] = 1D;
                                if (!dr.Field<bool>("IsStarted"))
                                    dr["ActDuration"] = dr["SchedDuration"];
                                dr["MaxReservedDuration"] = GetMaxReservedDuration(dr);
                            }
                        }
                        else
                            dr.Delete(); //not chargeable, so remove
                    }
                }
            }
        }

        public override void CalculateTransferTime(DataTable dtToolDataClean)
        {
            // do nothing
        }

        protected override void FillOutputTable(DataTable dtOutput, DataTable dtToolDataClean)
        {
            foreach (DataRow dr in dtToolDataClean.Rows)
            {
                if (dr.RowState != DataRowState.Deleted)
                {
                    // ClientID check. dtToolDataClean contains data for all clients even when ClientID > 0 because of transferred duration calculation.
                    int clientId = dr.Field<int>("ClientID");
                    if (ClientID == 0 || ClientID == clientId)
                    {
                        DateTime actDate = dr.Field<DateTime>("BeginDateTime").Date;
                        if ((actDate >= Period && actDate < Period.AddMonths(1)) && dr.Field<double>("SchedDuration") != 0D)
                        {
                            DataRow ndr = dtOutput.NewRow();
                            ndr.SetField("Period", new DateTime(actDate.Year, actDate.Month, 1));
                            ndr.SetField("ClientID", clientId);
                            ndr.SetField("ResourceID", dr.Field<int>("ResourceID"));
                            ndr.SetField("RoomID", dr.Field<int>("RoomID"));
                            ndr.SetField("ActDate", actDate);
                            ndr.SetField("AccountID", dr.Field<int>("AccountID"));
                            ndr.SetField("Uses", dr.Field<double>("Uses"));
                            ndr.SetField("SchedDuration", dr.Field<double>("SchedDuration"));
                            ndr.SetField("ActDuration", dr.Field<double>("ActDuration"));
                            ndr.SetField("Overtime", dr.Field<double>("Overtime"));
                            ndr.SetField("Days", 0D); //calculated in ToolDataAdjust
                            ndr.SetField("Months", 0D); //calulated in ToolDataAdjust
                            ndr.SetField("IsStarted", dr.Field<bool>("IsStarted"));
                            ndr.SetField("ChargeMultiplier", dr.Field<double>("ChargeMultiplier"));
                            ndr.SetField("ReservationID", dr.Field<int>("ReservationID"));
                            ndr.SetField("ChargeDuration", dr.Field<double>("ChargeDuration"));
                            ndr.SetField("TransferredDuration", 0D); //not applicable in old billing model
                            ndr.SetField("MaxReservedDuration", dr.Field<double>("MaxReservedDuration"));
                            ndr.SetField("ChargeBeginDateTime", dr.Field<DateTime>("ChargeBeginDateTime"));
                            ndr.SetField("ChargeEndDateTime", dr.Field<DateTime>("ChargeEndDateTime"));
                            ndr.SetField("IsActive", dr.Field<bool>("IsActive"));
                            ndr.SetField("IsCancelledBeforeAllowedTime", dr.Field<bool>("IsCancelledBeforeAllowedTime"));
                            dtOutput.Rows.Add(ndr);
                        }
                    }
                }
            }
        }
    }
}
