using System;
using System.Data;
using System.Linq;

namespace LNF.Impl.Billing
{
    public abstract class ToolDataTransformer
    {
        public DateTime Period { get; }
        public int ClientID { get; }
        public int ResourceID { get; }
        public DataTable ActivitiesTable { get; }
        public virtual int[] CancelledReservations() => new int[0];

        public ToolDataTransformer(DateTime period, int clientId, int resourceId, DataTable dtActivities)
        {
            Period = period;
            ClientID = clientId;
            ResourceID = resourceId;
            ActivitiesTable = dtActivities;
        }

        public DataTable DailyToolData(DataTable dtToolDataClean)
        {
            DataTable dtOutput = CreateToolDataTable();

            //the select statement pulls in all reservations that overlap the period
            //only write those whose ActDate is within the period

            if (dtToolDataClean.Rows.Count == 0)
                return dtOutput;

            ProcessCleanData(dtToolDataClean);
            CalculateTransferTime(dtToolDataClean);
            FillOutputTable(dtOutput, dtToolDataClean);

            return dtOutput;
        }

        public abstract void ProcessCleanData(DataTable dtToolDataClean);

        public abstract void CalculateTransferTime(DataTable dtToolDataClean);

        protected virtual DataTable CreateToolDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Period", typeof(DateTime));
            dt.Columns.Add("ClientID", typeof(int));
            dt.Columns.Add("ResourceID", typeof(int));
            dt.Columns.Add("RoomID", typeof(int));
            dt.Columns.Add("ActDate", typeof(DateTime));
            dt.Columns.Add("AccountID", typeof(int));
            dt.Columns.Add("Uses", typeof(double));
            dt.Columns.Add("SchedDuration", typeof(double));
            dt.Columns.Add("ActDuration", typeof(double));
            dt.Columns.Add("Overtime", typeof(double));
            dt.Columns.Add("Days", typeof(double));
            dt.Columns.Add("Months", typeof(double));
            dt.Columns.Add("IsStarted", typeof(bool));
            dt.Columns.Add("ChargeMultiplier", typeof(double));
            dt.Columns.Add("ReservationID", typeof(int)); //2010-05 we need this to forgive charge on tools
            dt.Columns.Add("ChargeDuration", typeof(double));
            dt.Columns.Add("TransferredDuration", typeof(double));
            dt.Columns.Add("MaxReservedDuration", typeof(double));
            dt.Columns.Add("ChargeBeginDateTime", typeof(DateTime));
            dt.Columns.Add("ChargeEndDateTime", typeof(DateTime));
            dt.Columns.Add("IsActive", typeof(bool));
            dt.Columns.Add("IsCancelledBeforeAllowedTime", typeof(bool));

            return dt;
        }

        protected virtual void FillOutputTable(DataTable dtOutput, DataTable dtToolDataClean)
        {
            DateTime sd = Period;
            DateTime ed = Period.AddMonths(1);

            foreach (DataRow dr in dtToolDataClean.Rows)
            {
                if (dr.RowState != DataRowState.Deleted)
                {
                    // ClientID check. dtToolDataClean contains data for all clients even when ClientID > 0 because of transferred duration calculation.
                    int clientId = dr.Field<int>("ClientID");
                    if (ClientID == 0 || ClientID == clientId)
                    {
                        DateTime actDate = Convert.ToDateTime(dr["BeginDateTime"]).Date;
                        double schedDuration = dr.Field<double>("SchedDuration");
                        if ((actDate >= sd && actDate < ed) && schedDuration != 0)
                        {
                            DataRow ndr = dtOutput.NewRow();
                            ndr.SetField("Period", Period);
                            ndr.SetField("ClientID", clientId);
                            ndr.SetField("ResourceID", dr.Field<int>("ResourceID"));
                            ndr.SetField("RoomID", dr.Field<int>("RoomID"));
                            ndr.SetField("ActDate", actDate);
                            ndr.SetField("AccountID", dr.Field<int>("AccountID"));
                            ndr.SetField("Uses", dr.Field<double>("Uses"));
                            ndr.SetField("SchedDuration", schedDuration);
                            ndr.SetField("ActDuration", dr.Field<double>("ActDuration"));
                            ndr.SetField("Overtime", dr.Field<double>("Overtime"));
                            ndr.SetField("Days", 0D); //calulated in ToolDataAdjust
                            ndr.SetField("Months", 0D); //calulated in ToolDataAdjust
                            ndr.SetField("IsStarted", dr.Field<bool>("IsStarted"));
                            ndr.SetField("ChargeMultiplier", dr.Field<double>("ChargeMultiplier"));
                            ndr.SetField("ReservationID", dr.Field<int>("ReservationID"));
                            ndr.SetField("ChargeDuration", dr.Field<double>("ChargeDuration"));
                            ndr.SetField("TransferredDuration", dr.Field<double>("TransferredDuration"));
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

        //public abstract bool IsCancelledBeforeAllowedTime(DataRow dr);
        //{
        //    var reservationId = dr.Field<int>("ReservationID");
        //    var beginDateTime = dr.Field<DateTime>("BeginDateTime");
        //    var cancelledDateTime = dr.Field<DateTime?>("CancelledDateTime");

        //    if (!cancelledDateTime.HasValue)
        //    {
        //        //for accuracy in generating 2011-04-01 tool data, this checking below is necessary
        //        var newBillingDate = GetNewBillingDate();
        //        if (CancelledReservations().FirstOrDefault(x => reservationId == x) > 0 && Period.Year == newBillingDate.Year && Period.Month == newBillingDate.Month)
        //            return true;
        //        else
        //            return false;
        //    }
        //    else
        //    {
        //        //determined by 2 hours before reservation time
        //        if (cancelledDateTime.Value.AddHours(2) < beginDateTime)
        //            return true;
        //        else
        //            return false;
        //    }
        //}

        protected virtual double GetMaxReservedDuration(DataRow dr)
        {
            // Is this a good idea? Seems like it would likely be multipled by Uses again later (what's the point of the Uses column).
            var result = dr.Field<double>("MaxReservedDuration") * dr.Field<double>("Uses");
            return result;
        }

        protected virtual bool IsChargeable(DataRow dr)
        {
            var activityId = dr.Field<int>("ActivityID");
            var result = ActivitiesTable.Rows.Find(activityId).Field<bool>("Chargeable");
            return result;
        }
    }
}
