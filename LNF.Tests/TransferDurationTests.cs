using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using LNF.Billing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LNF.Tests
{
    [TestClass]
    public class TransferDurationTests : TestBase
    {
        [TestMethod]
        public void CanGetTransferDruation()
        {
            int resourceId = 14021;
            var dateRange = DateRange.GetDateRange(DateTime.Parse("2018-05-01"));

            // step 1: get ReservationDateRangeItems
            var costs = Provider.Data.Cost.FindToolCosts(resourceId, dateRange.EndDate);
            var toolBillingReservations = Provider.Billing.Tool.SelectReservations(dateRange.StartDate, dateRange.EndDate, resourceId);
            var reservations = ReservationDateRangeItem.GetReservationDateRangeItems(toolBillingReservations, costs);

            // step 2: get ReservationDurations
            var range = new ReservationDateRange(reservations);
            var durations = new ReservationDurations(range);

            var item = durations.First(x => x.Reservation.ReservationID == 833138);
            Assert.AreEqual(TimeSpan.Zero, item.TransferredDuration);
            //Assert.IsTrue(item.TransferredDuration.TotalMinutes > 0);
        }

        private IEnumerable<ReservationDateRangeItem> GetReservationDateRangeItems(DataTable dtToolDataClean, DateTime period, int resourceId)
        {
            var cutoff = period.AddMonths(1);

            var costs = Provider.Data.Cost.FindToolCosts(resourceId, cutoff);

            var result = new List<ReservationDateRangeItem>();

            DataTable dtAccounts;

            using (var conn = NewConnection())
            using (var cmd = new SqlCommand("SELECT acct.AccountID, acct.Name AS AccountName, acct.ShortCode, org.OrgID, ot.ChargeTypeID FROM sselData.dbo.Account acct INNER JOIN sselData.dbo.Org org ON org.OrgID = acct.OrgID INNER JOIN sselData.dbo.OrgType ot ON ot.OrgTypeID = org.OrgTypeID", conn) { CommandType = CommandType.Text })
            using (var adap = new SqlDataAdapter(cmd))
            {
                dtAccounts = new DataTable();
                adap.Fill(dtAccounts);
                dtAccounts.PrimaryKey = new[] { dtAccounts.Columns["AccountID"] };
            }

            foreach (DataRow dr in dtToolDataClean.Rows)
            {
                var accountId = dr.Field<int>("AccountID");
                var drAcct = dtAccounts.Rows.Find(accountId);

                if (drAcct == null)
                    throw new Exception($"No Account found for AccountID = {accountId}");

                result.Add(ReservationDateRangeItem.Create(
                    dr.Field<int>("ReservationID"),
                    dr.Field<int>("ResourceID"),
                    dr.Field<string>("ResourceName"),
                    dr.Field<int>("ProcessTechID"),
                    dr.Field<string>("ProcessTechName"),
                    dr.Field<int>("ClientID"),
                    dr.Field<string>("UserName"),
                    dr.Field<string>("LName"),
                    dr.Field<string>("FName"),
                    dr.Field<int>("ActivityID"),
                    dr.Field<string>("ActivityName"),
                    accountId,
                    drAcct.Field<string>("AccountName"),
                    drAcct.Field<string>("ShortCode"),
                    drAcct.Field<int>("ChargeTypeID"),
                    dr.Field<bool>("IsActive"),
                    dr.Field<bool>("IsStarted"),
                    dr.Field<DateTime>("BeginDateTime"),
                    dr.Field<DateTime>("EndDateTime"),
                    dr.Field<DateTime>("ActualBeginDateTime"),
                    dr.Field<DateTime>("ActualEndDateTime"),
                    dr.Field<DateTime>("LastModifiedOn"),
                    dr.Field<DateTime?>("CancelledDateTime"),
                    dr.Field<double>("ChargeMultiplier"),
                    costs));
            }

            return result;
        }

        private IEnumerable<ReservationDateRangeItem> GetReservationDateRangeItems(DateRange range, int resourceId)
        {
            var costs = Provider.Data.Cost.FindToolCosts(resourceId, range.EndDate);
            var reservations = Provider.Billing.Tool.SelectReservations(range.StartDate, range.EndDate, resourceId);
            var reservationDateRangeItems = ReservationDateRangeItem.GetReservationDateRangeItems(reservations, costs);
            return reservationDateRangeItems;
        }
    }
}
