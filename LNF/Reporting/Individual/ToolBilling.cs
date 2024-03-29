﻿using LNF.Billing;
using LNF.Data;
using LNF.Scheduler;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Reporting.Individual
{
    public class ToolBilling
    {
        public IProvider Provider { get; }

        private IEnumerable<IRoom> _rooms;

        public ToolBilling(IProvider provider)
        {
            Provider = provider;
        }

        /// <summary>
        /// Used in User Usage Summary to get populate Tool Detail section.
        /// </summary>
        public DataTable GetAggreateByTool(IEnumerable<IToolBilling> query, IEnumerable<IResource> resources, IEnumerable<IAccount> accounts)
        {
            // This method creates a view of ToolBilling data where totals are aggregated by resource and account.
            // In addition extra columns are added for activated used, activated unused, and unstarted unused durations - which depend on the IsStarted and IsCancelledBeforeAllowedTime

            DataTable dt = new DataTable();
            dt.Columns.Add("ClientID", typeof(int));
            dt.Columns.Add("AccountID", typeof(int));
            dt.Columns.Add("AccountName", typeof(string));
            dt.Columns.Add("ShortCode", typeof(string));
            dt.Columns.Add("ResourceID", typeof(int));
            dt.Columns.Add("ResourceName", typeof(string));
            dt.Columns.Add("BillingTypeID", typeof(int));
            dt.Columns.Add("RoomID", typeof(int));
            dt.Columns.Add("RoomName", typeof(string));
            dt.Columns.Add("PerUseRate", typeof(decimal));
            dt.Columns.Add("ResourceRate", typeof(decimal));
            dt.Columns.Add("IsCancelledBeforeAllowedTime", typeof(bool));
            dt.Columns.Add("TotalUses", typeof(decimal));
            dt.Columns.Add("TotalSchedDuration", typeof(decimal));
            dt.Columns.Add("TotalActDuration", typeof(decimal));
            dt.Columns.Add("TotalChargeDuration", typeof(decimal));
            dt.Columns.Add("TotalTransferredDuration", typeof(decimal));
            dt.Columns.Add("TotalForgivenDuration", typeof(decimal));
            dt.Columns.Add("TotalOverTime", typeof(decimal));
            dt.Columns.Add("UsageFeeCharged", typeof(decimal));
            dt.Columns.Add("OverTimePenaltyFee", typeof(decimal));
            dt.Columns.Add("BookingFee", typeof(decimal));
            dt.Columns.Add("ActivatedUsed", typeof(double));
            dt.Columns.Add("ActivatedUnused", typeof(double));
            dt.Columns.Add("UnstartedUnused", typeof(double));
            dt.Columns.Add("LineCost", typeof(decimal));

            foreach (var item in query.OrderBy(x => x.AccountID).ThenBy(x => x.ResourceID))
            {
                DataRow dr = dt.Select(string.Format("ClientID = {0} AND ResourceID = {1} AND AccountID = {2}", item.ClientID, item.ResourceID, item.AccountID)).FirstOrDefault();

                IResource res = resources.First(x => x.ResourceID == item.ResourceID);

                if (dr == null)
                {
                    IAccount acct = accounts.FirstOrDefault(x => x.AccountID == item.AccountID);

                    string acctName, shortCode;

                    if (acct == null)
                    {
                        acctName = $"unknown account [{item.AccountID}]";
                        shortCode = string.Empty;
                    }
                    else
                    {
                        acctName = acct.AccountName;
                        shortCode = acct.ShortCode.Trim();
                    }

                    dr = dt.NewRow();
                    dr.SetField("ClientID", item.ClientID);
                    dr.SetField("AccountID", item.AccountID);
                    dr.SetField("AccountName", acctName);
                    dr.SetField("ShortCode", shortCode);
                    dr.SetField("ResourceID", res.ResourceID);
                    dr.SetField("ResourceName", res.ResourceName);
                    dr.SetField("BillingTypeID", item.BillingTypeID);
                    dr.SetField("RoomID", item.RoomID);
                    dr.SetField("RoomName", GetRoomDisplayName(item.RoomID));
                    dr.SetField("PerUseRate", item.PerUseRate);
                    dr.SetField("ResourceRate", item.ResourceRate);
                    dr.SetField("IsCancelledBeforeAllowedTime", item.IsCancelledBeforeAllowedTime);
                    dr.SetField("TotalUses", 0M);
                    dr.SetField("TotalSchedDuration", 0M);
                    dr.SetField("TotalActDuration", 0M);
                    dr.SetField("TotalChargeDuration", 0M);
                    dr.SetField("TotalTransferredDuration", 0M);
                    dr.SetField("TotalForgivenDuration", 0M);
                    dr.SetField("TotalOverTime", 0M);
                    dr.SetField("UsageFeeCharged", 0M);
                    dr.SetField("OverTimePenaltyFee", 0M);
                    dr.SetField("BookingFee", 0M);
                    dr.SetField("ActivatedUsed", 0D);
                    dr.SetField("ActivatedUnused", 0D);
                    dr.SetField("UnstartedUnused", 0D);
                    dr.SetField("LineCost", 0M);
                    dt.Rows.Add(dr);
                }

                AddToColumn(dr, "TotalUses", item.Uses);
                AddToColumn(dr, "TotalActDuration", item.ActDuration / 60);
                AddToColumn(dr, "TotalSchedDuration", item.SchedDuration / 60);
                AddToColumn(dr, "TotalChargeDuration", item.ChargeDuration / 60);
                AddToColumn(dr, "TotalTransferredDuration", item.TransferredDuration / 60);
                AddToColumn(dr, "TotalForgivenDuration", item.ForgivenDuration / 60);
                AddToColumn(dr, "TotalOverTime", item.OverTime / 60);
                AddToColumn(dr, "UsageFeeCharged", item.UsageFeeCharged);
                AddToColumn(dr, "OverTimePenaltyFee", item.OverTimePenaltyFee);
                AddToColumn(dr, "BookingFee", item.BookingFee); //no need to check IsCancelledBeforeAllowedTime, will be zero if not true

                // Activated Used (hours)
                var activatedUsed = item.ActivatedUsed().TotalHours;

                // Activated Unused (hours)
                var activatedUnused = item.ActivatedUnused().TotalHours;

                // Unactivated (hours)
                var unstartedUnused = item.UnstartedUnused().TotalHours;

                AddToColumn(dr, "ActivatedUsed", activatedUsed);
                AddToColumn(dr, "ActivatedUnused", activatedUnused);
                AddToColumn(dr, "UnstartedUnused", unstartedUnused);

                AddToColumn(dr, "LineCost", Provider.Billing.Tool.GetLineCost(new ToolLineCostParameters(item, res.ResourceName)));
            }

            dt.DefaultView.Sort = "RoomName ASC, ResourceName ASC";

            return dt;
        }

        public DataTable GetToolCharges(DataTable dtAggByTool)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ClientID", typeof(int));
            dt.Columns.Add("AccountID", typeof(int));
            dt.Columns.Add("AccountName", typeof(string));
            dt.Columns.Add("ShortCode", typeof(string));
            dt.Columns.Add("ResourceID", typeof(int));
            dt.Columns.Add("ResourceName", typeof(string));
            dt.Columns.Add("BillingTypeID", typeof(int));
            dt.Columns.Add("RoomID", typeof(int));
            dt.Columns.Add("RoomName", typeof(string));
            dt.Columns.Add("PerUseRate", typeof(decimal));
            dt.Columns.Add("HourlyRate", typeof(decimal));
            //dt.Columns.Add("IsCancelledBeforeAllowedTime", typeof(bool));
            //dt.Columns.Add("TotalUses", typeof(decimal));
            //dt.Columns.Add("TotalSchedDuration", typeof(decimal));
            //dt.Columns.Add("TotalActDuration", typeof(decimal));
            //dt.Columns.Add("TotalChargeDuration", typeof(decimal));
            //dt.Columns.Add("TotalTransferredDuration", typeof(decimal));
            //dt.Columns.Add("TotalForgivenDuration", typeof(decimal));
            //dt.Columns.Add("TotalOverTime", typeof(decimal));
            //dt.Columns.Add("UsageFeeCharged", typeof(decimal));
            //dt.Columns.Add("OverTimePenaltyFee", typeof(decimal));
            //dt.Columns.Add("ActivatedUsed", typeof(double));
            //dt.Columns.Add("ActivatedUnused", typeof(double));
            //dt.Columns.Add("UnstartedUnused", typeof(double));
            dt.Columns.Add("TotalDuration", typeof(double));
            dt.Columns.Add("BookingFee", typeof(decimal));
            dt.Columns.Add("PerUseCharge", typeof(decimal));
            dt.Columns.Add("HourlyCharge", typeof(decimal));
            dt.Columns.Add("LineTotal", typeof(decimal));

            foreach (DataRow dr in dtAggByTool.Rows)
            {
                decimal chargeDuration = dr.Field<decimal>("TotalChargeDuration");
                decimal overtimeDuration = dr.Field<decimal>("TotalOverTime");

                //decimal forgivenDuration = dr.Field<decimal>("TotalForgivenDuration");
                
                decimal totalDuration = chargeDuration + (1.5M * overtimeDuration);

                var ndr = dt.NewRow();
                ndr.SetField("ClientID", dr.Field<int>("ClientID"));
                ndr.SetField("AccountID", dr.Field<int>("AccountID"));
                ndr.SetField("AccountName", dr.Field<string>("AccountName"));
                ndr.SetField("ShortCode", dr.Field<string>("ShortCode"));
                ndr.SetField("ResourceID", dr.Field<int>("ResourceID"));
                ndr.SetField("ResourceName", dr.Field<string>("ResourceName"));
                ndr.SetField("BillingTypeID", dr.Field<int>("BillingTypeID"));
                ndr.SetField("RoomID", dr.Field<int>("RoomID"));
                ndr.SetField("RoomName", dr.Field<string>("RoomName"));
                ndr.SetField("PerUseRate", dr.Field<decimal>("PerUseRate"));
                ndr.SetField("HourlyRate", dr.Field<decimal>("ResourceRate"));
                //dr.SetField("IsCancelledBeforeAllowedTime", item.IsCancelledBeforeAllowedTime);
                //dr.SetField("TotalUses", 0M);
                //dr.SetField("TotalSchedDuration", 0M);
                //dr.SetField("TotalActDuration", 0M);
                //dr.SetField("TotalChargeDuration", 0M);
                //dr.SetField("TotalTransferredDuration", 0M);
                //dr.SetField("TotalForgivenDuration", 0M);
                //dr.SetField("TotalOverTime", 0M);
                //dr.SetField("UsageFeeCharged", 0M);
                //dr.SetField("OverTimePenaltyFee", 0M);
                //dr.SetField("ActivatedUsed", 0D);
                //dr.SetField("ActivatedUnused", 0D);
                //dr.SetField("UnstartedUnused", 0D);
                ndr.SetField("TotalDuration", totalDuration);
                ndr.SetField("BookingFee", dr.Field<decimal>("BookingFee"));
                ndr.SetField("PerUseCharge", 0M);
                ndr.SetField("HourlyCharge", 0M);
                ndr.SetField("LineTotal", dr.Field<decimal>("LineCost"));
                dt.Rows.Add(ndr);
            }

            dt.DefaultView.Sort = "RoomName ASC, ResourceName ASC";

            return dt;
        }

        public string GetRoomDisplayName(int roomId)
        {
            if (_rooms == null)
                _rooms = Provider.Data.Room.GetRooms();

            IRoom room = _rooms.FirstOrDefault(x => x.RoomID == roomId);

            string result = "n/a";

            if (room != null)
                result = room.RoomDisplayName;

            return result;
        }

        private void AddToColumn(DataRow dr, string columnName, double value)
        {
            try
            {
                dr.SetField(columnName, dr.Field<double>(columnName) + value);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Failed to cast column {0} to double while adding value {1}.", columnName, value), ex);
            }

        }

        private void AddToColumn(DataRow dr, string columnName, decimal value)
        {
            try
            {
                dr.SetField(columnName, dr.Field<decimal>(columnName) + value);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Failed to cast column {0} to decimal while adding value {1}.", columnName, value), ex);
            }
        }
    }
}
