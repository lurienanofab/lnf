using LNF.Models.PhysicalAccess;
using LNF.Repository;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.CommonTools
{
    //sdate must always be first of the month. edate can never be > first of next month
    //raw means straight from the DB
    //filtered means that extraneous data have been removed - this is stored in RoomData
    //clean means read from clean table
    public class ReadRoomDataManager : ManagerBase, IReadRoomDataManager
    {
        public ReadRoomDataManager(IProvider provider) : base(provider) { }

        //used above and in mscModTIL
        //RoomID and ClientID are passed in because this can be called from an app
        //  and without these, the returned dataset is too big

        //2007-11-08 Wen: The strange thing here is this function will also set a global dataset, so the return data tables are not limited to the raw data
        //from prowatch, but also room data and client data for this month as well.
        public DataSet ReadRoomDataRaw(DateTime sd, DateTime ed, int clientId = 0, int roomId = 0)
        {
            using (var reader = new RoomDataImportReader(sd, ed, clientId, roomId))
            {
                reader.SelectRoomDataImportItems();
                var result = reader.AsDataSet();
                return result;
            }
        }

        public DataSet NewReadRoomDataRaw(DateTime sd, DateTime ed, int clientId = 0)
        {
            DataSet ds = new DataSet();
            ds.Tables.Add("raw");
            ds.Tables.Add("clients");
            ds.Tables.Add("rooms");

            List<Event> result = Provider.PhysicalAccess.GetEvents(sd, ed, clientId).ToList();

            DataTable dtRaw = ds.Tables["raw"];
            dtRaw.Columns.Add("ClientID", typeof(int));

            result.ForEach(x =>
            {
                dtRaw.Rows.Add("");
            });

            int[] clients = result.Where(x => x.ClientID == clientId).Select(x => x.ClientID).Distinct().ToArray();
            DataTable dtClients = ds.Tables["clients"];
            dtClients.Columns.Add("ClientID", typeof(int));

            foreach (int id in clients)
                dtClients.Rows.Add(id);

            var rooms = Session.Query<Room>().Where(x => x.Active).ToArray()
                .Join(Provider.CostManager.FindCosts(new[] { "RoomCost" }, ed), x => x.RoomID, y => y.RecordID, (x, y) => new
                {
                    x.RoomID,
                    x.RoomName,
                    x.PassbackRoom,
                    IsChargeRoom = (y.AcctPer != "None")
                });

            DataTable dtRooms = ds.Tables["rooms"];
            dtRooms.Columns.Add("RoomID", typeof(int));
            dtRooms.Columns.Add("RoomName", typeof(string));
            dtRooms.Columns.Add("PassbackRoom", typeof(bool));
            dtRooms.Columns.Add("IsChargeRoom", typeof(bool));

            foreach (var r in rooms)
                dtRooms.Rows.Add(r.RoomID, r.RoomName, r.PassbackRoom, r.IsChargeRoom);

            return ds;
        }

        public DataSet ReadRoomDataClean(DateTime sd, DateTime ed, int clientId = 0, int roomId = 0)
        {
            var ds = Command()
                .Param("Action", "ByDateRange")
                .Param("sDate", sd)
                .Param("eDate", ed)
                .Param("ClientID", clientId > 0, clientId)
                .Param("RoomID", roomId > 0, roomId)
                .FillDataSet("dbo.RoomDataClean_Select");

            // Three rows are returned:
            //      0) RoomDataClean rows for the date range
            //      1) Distinct clients in the RoomDataClean rows
            //      2) Distinct rooms in the RoomDataClean rows

            ds.Tables[0].TableName = "RoomDataClean";
            ds.Tables[1].TableName = "Client";
            ds.Tables[2].TableName = "Room";

            return ds;
        }

        public DataTable ReadRoomData(DateTime period, int clientId = 0, int roomId = 0)
        {
            var dt = Command()
                .Param("Action", "AggByMonthRoom")
                .Param("Period", period)
                .Param("ClientID", clientId > 0, clientId)
                .Param("RoomID", roomId > 0, roomId)
                .FillDataTable("dbo.RoomData_Select");

            dt.TableName = "RoomUsage";

            return dt;
        }

        public DataSet ReadRoomDataForUpdate(DateTime targetDate, int clientId = 0, int roomId = 0)
        {
            return Command()
                .Param("Action", "TargetDate")
                .Param("TargetDate", targetDate)
                .Param("ClientID", clientId > 0, clientId)
                .Param("RoomID", roomId > 0, roomId)
                .FillDataSet("dbo.RoomDataClean_Select");
        }
    }
}
