using LNF.Billing;
using LNF.CommonTools;
using LNF.Impl.DataAccess;
using LNF.Impl.Repository.Billing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Impl.Billing
{
    public class RoomBillingRepository : BillingRepository, IRoomBillingRepository
    {
        public RoomBillingRepository(ISessionManager mgr) : base(mgr) { }

        public IEnumerable<IRoomBilling> CreateRoomBilling(DateTime period, int clientId = 0)
        {
            // Does the same processing as BillingDataProcessStep1.PopulateRoomBilling (transforms
            // a RoomData record into a RoomBilling record) without saving anything to the database.
            using (var conn = NewConnection())
            {
                DataSet ds;
                DataTable dt;

                var useParentRooms = bool.Parse(Utility.GetRequiredAppSetting("UseParentRooms"));
                var temp = period == DateTime.Now.FirstOfMonth();

                var result = new List<IRoomBilling>();

                DateTime now = DateTime.Now;

                var step1 = new BillingDataProcessStep1(new Step1Config { Connection = conn, Context = "RoomBillingRepository.CreateRoomBilling", Period = period, Now = now, ClientID = clientId, IsTemp = temp });

                ds = step1.GetRoomData();
                dt = step1.LoadRoomBilling(ds);
                result.AddRange(CreateRoomBillingItems(dt, temp));

                if (useParentRooms)
                {
                    ds = step1.GetRoomData(BillingDataProcessStep1.FOR_PARENT_ROOMS);
                    dt = step1.LoadRoomBilling(ds);
                    result.AddRange(CreateRoomBillingItems(dt, temp));
                }

                return result;
            }
        }

        public IEnumerable<IRoomData> CreateRoomData(DateTime period, int clientId = 0, int roomId = 0)
        {
            // Does the processing without saving anything to the database.
            using (var conn = NewConnection())
            {
                var proc = new WriteRoomDataProcess(new WriteRoomDataConfig { Connection = conn, Context = "RoomBillingRepository.CreateRoomData", Period = period, ClientID = clientId, RoomID = roomId });
                var dtExtract = proc.Extract();
                var dtTransform = proc.Transform(dtExtract);

                var result = dtTransform.AsEnumerable().Select(x => new RoomDataItem
                {
                    RoomDataID = x.Field<int>("RoomDataID"),
                    Period = x.Field<DateTime>("Period"),
                    ClientID = x.Field<int>("ClientID"),
                    RoomID = x.Field<int>("RoomID"),
                    ParentID = x.Field<int?>("ParentID"),
                    PassbackRoom = x.Field<bool>("PassbackRoom"),
                    EvtDate = x.Field<DateTime>("EvtDate"),
                    AccountID = x.Field<int>("AccountID"),
                    Entries = x.Field<double>("Entries"),
                    Hours = x.Field<double>("Hours"),
                    Days = x.Field<double>("Days"),
                    Months = x.Field<double>("Months"),
                    DataSource = x.Field<int>("DataSource"),
                    HasToolUsage = x.Field<bool>("HasToolUsage"),
                }).ToList();

                return result;
            }
        }

        public IEnumerable<IRoomDataImportLog> GetImportLogs(DateTime sd, DateTime ed)
        {
            var query = Session.Query<RoomDataImportLog>().Where(x => x.ImportDateTime >= sd && x.ImportDateTime < ed);
            var result = query.CreateModels<IRoomDataImportLog>();
            return result;
        }

        public int UpdateBillingType(int clientId, int accountId, int billingTypeId, DateTime period) => Session.UpdateRoomBillingType(clientId, accountId, billingTypeId, period);

        public IEnumerable<IRoomBilling> GetRoomBilling(DateTime period, int clientId = 0, int roomId = 0, int accountId = 0)
        {
            var temp = period == DateTime.Now.FirstOfMonth();

            var query = GetRoomBillingQuery(temp).Where(x =>
                x.Period == period
                && x.ClientID == (clientId > 0 ? clientId : x.ClientID)
                && x.RoomID == (roomId > 0 ? roomId : x.RoomID)
                && x.AccountID == (accountId > 0 ? accountId : x.AccountID));

            var result = query.CreateRoomBillingItems();

            return result;
        }

        public IEnumerable<IRoomData> GetRoomData(DateTime period, int clientId = 0, int roomId = 0)
        {
            var query = Session.Query<RoomData>()
                .Where(x => x.Period == period
                    && x.ClientID == (clientId > 0 ? clientId : x.ClientID)
                    && x.RoomID == (roomId > 0 ? roomId : x.RoomID));

            var result = query.CreateRoomDataItems();

            return result;
        }

        public IEnumerable<IRoomDataClean> GetRoomDataClean(DateTime sd, DateTime ed, int clientId = 0, int roomId = 0)
        {
            var query = Session.Query<RoomDataClean>()
                .Where(x => x.EntryDT < ed && x.ExitDT > sd
                    && x.ClientID == (clientId > 0 ? clientId : x.ClientID)
                    && x.RoomID == (roomId > 0 ? roomId : x.RoomID));

            var result = query.CreateRoomDataCleanItems();

            return result;
        }

        private IQueryable<IRoomBilling> GetRoomBillingQuery(bool temp)
        {
            if (temp)
                return Session.Query<RoomBillingTemp>();
            else
                return Session.Query<RoomBilling>();
        }

        public static IRoomBilling CreateRoomBillingItem(bool isTemp)
        {
            IRoomBilling result;

            if (isTemp)
                result = new RoomBillingTemp();
            else
                result = new RoomBilling();

            return result;
        }

        public static IRoomBilling CreateRoomBillingFromDataRow(DataRow dr, bool isTemp)
        {
            // Using Convert.ToDecimal because values can be either decimal or double depending on if they are from RoomBilling or RoomBillingTemp.

            IRoomBilling item = CreateRoomBillingItem(isTemp);

            item.RoomBillingID = 0;
            item.Period = dr.Field<DateTime>("Period");
            item.ClientID = dr.Field<int>("ClientID");
            item.RoomID = dr.Field<int>("RoomID");
            item.AccountID = dr.Field<int>("AccountID");
            item.ChargeTypeID = dr.Field<int>("ChargeTypeID");
            item.BillingTypeID = dr.Field<int>("BillingTypeID");
            item.OrgID = dr.Field<int>("OrgID");
            item.ChargeDays = Convert.ToDecimal(dr["ChargeDays"]);
            item.PhysicalDays = Convert.ToDecimal(dr["PhysicalDays"]);
            item.AccountDays = Convert.ToDecimal(dr["AccountDays"]);
            item.Entries = Convert.ToDecimal(dr["Entries"]);
            item.Hours = Convert.ToDecimal(dr["Hours"]);
            item.IsDefault = GetValueOrDefault(dr, "IsDefault", false); //column may not be present
            item.RoomRate = dr.Field<decimal>("RoomRate");
            item.EntryRate = dr.Field<decimal>("EntryRate");
            item.MonthlyRoomCharge = dr.Field<decimal>("MonthlyRoomCharge");
            item.RoomCharge = dr.Field<decimal>("RoomCharge");
            item.EntryCharge = dr.Field<decimal>("EntryCharge");
            item.SubsidyDiscount = GetValueOrDefault(dr, "SubsidyDiscount", 0M); //column may not be present

            return item;
        }

        public static IEnumerable<IRoomBilling> CreateRoomBillingItems(DataTable dt, bool isTemp)
        {
            return dt.AsEnumerable().Select(x => CreateRoomBillingFromDataRow(x, isTemp)).ToList();
        }

        private static T GetValueOrDefault<T>(DataRow dr, string key, T defval)
        {
            if (dr.Table.Columns.Contains(key))
                return dr.Field<T>(key);
            else
                return defval;
        }
    }
}
