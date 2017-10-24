using LNF.Data;
using LNF.Logging;
using LNF.Repository;
using System;
using System.Data;

namespace LNF.CommonTools
{
    public class WriteStoreDataManager
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ClientID { get; set; }
        public int ItemID { get; set; }

        // force using the static constructor
        private WriteStoreDataManager() { }

        /// <summary>
        /// Creates a new instance of WriteStoreDataManager with the given parameters.
        /// </summary>
        public static WriteStoreDataManager Create(DateTime startDate, DateTime endDate, int clientId = 0, int itemId = 0)
        {
            return new WriteStoreDataManager()
            {
                StartDate = startDate,
                EndDate = endDate,
                ClientID = clientId,
                ItemID = itemId
            };
        }

        /// <summary>
        /// This method will:
        ///     1) Select records from Store in the date range to insert.
        ///     2) Delete records from StoreDataClean in the date range.
        ///     3) Insert records from Store into StoreDataClean.
        ///     4) Insert records for DryBox into StoreDataClean.
        /// </summary>
        public void WriteStoreDataClean()
        {
            int rowsSelectedFromStore = 0;
            int rowsDeletedFromStoreDataClean = 0;
            int rowsInsertedIntoStoreDataClean = 0;
            int rowsInsertedIntoStoreDataCleanForDryBox = 0;

            using (LogTaskTimer.Start("WriteStoreDataManager.WriteStoreDataClean", "ClientID = {0}, ItemID = {1}, StartDate= '{2:yyyy-MM-dd}', EndDate = '{3:yyyy-MM-dd}', RowsSelectedFromStore = {4}, RowsDeletedFromStoreDataClean = {5}, RowsInsertedIntoStoreDataClean = {6}, RowsInsertedIntoStoreDataCleanForDryBox = {7}", () => new object[] { ClientID, ItemID, StartDate, EndDate, rowsSelectedFromStore, rowsDeletedFromStoreDataClean, rowsInsertedIntoStoreDataClean, rowsInsertedIntoStoreDataCleanForDryBox }))
            {
                //all database interaction for StoreDataClean must be done through DA.Current.GetAdapater so that it shares the
                //transaction. Otherwise the the call to StoreDataCleanUtility.LoadDryBoxBilling(...) will fail because of 
                //table locking

                //always write all items at the same time
                var reader = new ReadStoreDataManager();
                var dtSource = reader.ReadStoreDataFiltered(StartDate, EndDate, ClientID);

                rowsSelectedFromStore = dtSource.Rows.Count;

                if (rowsSelectedFromStore > 0)
                {
                    rowsDeletedFromStoreDataClean = DeleteStoreDataClean();

                    DataTable dtClean = Utility.CopyDT(dtSource);

                    //insert the table into the DB
                    //need to use DA.Current.GetAdapter() because of DryBox data
                    using (var dba = DA.Current.GetAdapter())
                    {
                        dba.InsertCommand
                            .AddParameter("@ClientID", SqlDbType.Int)
                            .AddParameter("@ItemID", SqlDbType.Int)
                            .AddParameter("@OrderDate", SqlDbType.DateTime)
                            .AddParameter("@AccountID", SqlDbType.Int)
                            .AddParameter("@Quantity", SqlDbType.Float)
                            .AddParameter("@UnitCost", SqlDbType.Float)
                            .AddParameter("@CategoryID", SqlDbType.Int)
                            .AddParameter("@RechargeItem", SqlDbType.Bit)
                            .AddParameter("@StatusChangeDate", SqlDbType.DateTime);

                        rowsInsertedIntoStoreDataClean = dba.UpdateDataTable(dtClean, "StoreDataClean_Insert");
                    }
                }

                rowsInsertedIntoStoreDataCleanForDryBox = StoreDataCleanUtility.LoadDryBoxBilling(StartDate, EndDate);
            }
        }

        public int DeleteStoreDataClean()
        {
            //need to use DA.Current.GetAdapter() because of DryBox data
            using (var dba = DA.Current.GetAdapter())
            {
                return dba.SelectCommand
                    .AddParameter("@sDate", StartDate)
                    .AddParameter("@eDate", EndDate)
                    .AddParameterIf("@ClientID", ClientID > 0, ClientID)
                    .AddParameterIf("@ItemID", ItemID > 0, ItemID)
                    .ExecuteNonQuery("StoreDataClean_Delete");
            }
        }

        /// <summary>
        /// This method will:
        ///     1) Delete records from StoreData in the date range.
        ///     2) Select records from StoreDataClean in the date range to insert.
        ///     3) Insert records from StoreDataClean records into StoreData.
        /// </summary>
        public void WriteStoreData()
        {
            int rowsDeletedFromStoreData = 0;
            int rowsSelectedFromStoreDataClean = 0;
            int rowsInsertedIntoStoreData = 0;

            using (LogTaskTimer.Start("WriteStoreDataManager.WriteStoreData", "ClientID = {0}, ItemID = {1}, StartDate= '{2:yyyy-MM-dd}', EndDate = '{3:yyyy-MM-dd}', RowsDeletedFromStoreData = {4}, RowsSelectedFromStoreDataClean = {5}, RowsInsertedIntoStoreData = {6}", () => new object[] { ClientID, ItemID, StartDate, EndDate, rowsDeletedFromStoreData, rowsSelectedFromStoreDataClean, rowsInsertedIntoStoreData }))
            {
                //get rid of any non-user entered entries
                using (var dba = DA.Current.GetAdapter())
                {
                    rowsDeletedFromStoreData = dba.SelectCommand
                        .AddParameter("@Period", StartDate)
                        .AddParameterIf("@ClientID", ClientID > 0, ClientID)
                        .AddParameterIf("@ItemID", ItemID > 0, ItemID)
                        .ExecuteNonQuery("StoreData_Delete");
                }

                //get all store data for period
                ReadStoreDataManager readStoreData = new ReadStoreDataManager();
                DataTable dtStoreDataClean = readStoreData.ReadStoreDataClean(ReadStoreDataManager.StoreDataCleanOption.RechargeItems, StartDate, EndDate, ClientID, ItemID);

                rowsSelectedFromStoreDataClean = dtStoreDataClean.Rows.Count;

                DataTable dtStoreData = Utility.CopyDT(dtStoreDataClean);

                using (var dba = DA.Current.GetAdapter())
                {
                    dba.InsertCommand
                        .AddParameter("@Period", StartDate)
                        .AddParameter("@ClientID", SqlDbType.Int)
                        .AddParameter("@ItemID", SqlDbType.Int)
                        .AddParameter("@OrderDate", SqlDbType.DateTime)
                        .AddParameter("@AccountID", SqlDbType.Int)
                        .AddParameter("@Quantity", SqlDbType.Float)
                        .AddParameter("@UnitCost", SqlDbType.Float)
                        .AddParameter("@CategoryID", SqlDbType.Int)
                        .AddParameter("@StatusChangeDate", SqlDbType.DateTime);

                    rowsInsertedIntoStoreData = dba.UpdateDataTable(dtStoreData, "StoreData_Insert");
                }
            }
        }
    }
}
