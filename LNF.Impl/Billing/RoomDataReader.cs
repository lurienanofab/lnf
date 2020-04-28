using LNF.CommonTools;
using LNF.PhysicalAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace LNF.Impl.Billing
{
    //sdate must always be first of the month. edate can never be > first of next month
    //raw means straight from the DB
    //filtered means that extraneous data have been removed - this is stored in RoomData
    //clean means read from clean table
    public class RoomDataReader : ReaderBase
    {
        public RoomDataReader(SqlConnection conn) : base(conn) { }

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

        public DataSet NewReadRoomDataRaw(List<Event> events, int clientId)
        {
            DataSet ds = new DataSet();
            ds.Tables.Add("raw");
            ds.Tables.Add("clients");
            //ds.Tables.Add("rooms");

            //List<Event> result = PhysicalAccess.GetEvents(sd, ed, clientId).ToList();

            DataTable dtRaw = ds.Tables["raw"];
            dtRaw.Columns.Add("ClientID", typeof(int));

            events.ForEach(x =>
            {
                dtRaw.Rows.Add("");
            });

            int[] clients = events.Where(x => x.ClientID == clientId).Select(x => x.ClientID).Distinct().ToArray();
            DataTable dtClients = ds.Tables["clients"];
            dtClients.Columns.Add("ClientID", typeof(int));

            foreach (int id in clients)
                dtClients.Rows.Add(id);

            using (var cmd = new SqlCommand("SELECT * FROM sselData.dbo.v_ChargeRoom", Connection) { CommandType = CommandType.Text })
            using (var adap = new SqlDataAdapter(cmd))
            {
                var dt = new DataTable();
                adap.Fill(dt);

                dt.TableName = "rooms";
                ds.Tables.Add(dt);
            }

            return ds;
        }

        public DataSet ReadRoomDataClean(DateTime sd, DateTime ed, int clientId = 0, int roomId = 0)
        {
            using (var cmd = new SqlCommand("dbo.RoomDataClean_Select", Connection) { CommandType = CommandType.StoredProcedure })
            using (var adap = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("Action", "ByDateRange");
                cmd.Parameters.AddWithValue("sDate", sd);
                cmd.Parameters.AddWithValue("eDate", ed);
                AddParameterIf(cmd, "ClientID", clientId > 0, clientId);
                AddParameterIf(cmd, "RoomID", roomId > 0, roomId);

                var ds = new DataSet();
                adap.Fill(ds);

                // Three rows are returned:
                //      0) RoomDataClean rows for the date range
                //      1) Distinct clients in the RoomDataClean rows
                //      2) Distinct rooms in the RoomDataClean rows

                ds.Tables[0].TableName = "RoomDataClean";
                ds.Tables[1].TableName = "Client";
                ds.Tables[2].TableName = "Room";

                return ds;
            }
        }

        public DataTable ReadRoomData(DateTime period, int clientId = 0, int roomId = 0)
        {
            using (var cmd = new SqlCommand("dbo.RoomData_Select", Connection) { CommandType = CommandType.StoredProcedure })
            using (var adap = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("Action", "AggByMonthRoom");
                cmd.Parameters.AddWithValue("Period", period);
                AddParameterIf(cmd, "ClientID", clientId > 0, clientId);
                AddParameterIf(cmd, "RoomID", roomId > 0, roomId);

                var dt = new DataTable();
                adap.Fill(dt);

                dt.TableName = "RoomUsage";

                return dt;
            }
        }

        public DataSet ReadRoomDataForUpdate(DateTime period, int clientId = 0, int roomId = 0)
        {
            using (var cmd = new SqlCommand("dbo.RoomDataClean_Select", Connection) { CommandType = CommandType.StoredProcedure })
            using (var adap = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("Action", "TargetDate");
                cmd.Parameters.AddWithValue("TargetDate", period);
                AddParameterIf(cmd, "ClientID", clientId > 0, clientId);
                AddParameterIf(cmd, "RoomID", roomId > 0, roomId);

                var ds = new DataSet();
                adap.Fill(ds);

                return ds;
            }
        }
    }
}
