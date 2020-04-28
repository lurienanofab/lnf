using LNF.Billing;
using LNF.Data;
using LNF.Impl.DataAccess;
using LNF.Impl.Repository.Billing;
using LNF.Impl.Repository.Data;
using LNF.PhysicalAccess;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace LNF.Impl.Billing
{
    public class RoomDataRepository : BillingRepository, IRoomDataRepository
    {
        public RoomDataRepository(ISessionManager mgr) : base(mgr) { }

        public DataSet ReadRoomDataRaw(DateTime sd, DateTime ed, int clientId = 0, int roomId = 0)
        {
            using (var conn = NewConnection())
            {
                conn.Open();
                var result = GetReader(conn).ReadRoomDataRaw(sd, ed, clientId, roomId);
                conn.Close();
                return result;
            }
        }

        public DataSet ReadRoomDataClean(DateTime sd, DateTime ed, int clientId = 0, int roomId = 0)
        {
            using (var conn = NewConnection())
            {
                conn.Open();
                var result = GetReader(conn).ReadRoomDataClean(sd, ed, clientId, roomId);
                conn.Close();
                return result;
            }
        }

        public DataTable ReadRoomData(DateTime period, int clientId = 0, int roomId = 0)
        {
            using (var conn = NewConnection())
            {
                conn.Open();
                var result = GetReader(conn).ReadRoomData(period, clientId, roomId);
                conn.Close();
                return result;
            }
        }

        public DataSet ReadRoomDataForUpdate(DateTime period, int clientId = 0, int roomId = 0)
        {
            using (var conn = NewConnection())
            {
                conn.Open();
                var result = GetReader(conn).ReadRoomDataForUpdate(period, clientId, roomId);
                conn.Close();
                return result;
            }
        }

        public IRoomDataClean AddRoomDataClean(int clientId, int roomId, DateTime entryDT, DateTime exitDT, double duration)
        {
            var rdc = new RoomDataClean
            {
                ClientID = clientId,
                RoomID = roomId,
                EntryDT = entryDT,
                ExitDT = exitDT,
                Duration = duration
            };

            Session.Update(rdc);

            return rdc.CreateModel<IRoomDataClean>();
        }

        public IRoomData AddRoomData(IRoomDataClean rdc)
        {
            throw new NotImplementedException();
        }

        public string GetEmail(IRoomData item)
        {
            var ca = Session
                .Query<ClientAccountInfo>()
                .First(x => x.ClientID == item.ClientID && x.AccountID == item.AccountID);

            return ca.Email;
        }

        private RoomDataReader GetReader(SqlConnection conn)
        {
            return new RoomDataReader(conn);
        }
    }
}
