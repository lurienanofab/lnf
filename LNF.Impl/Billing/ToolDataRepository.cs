using LNF.Billing;
using LNF.Data;
using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Data;
using LNF.Impl.Repository.Scheduler;
using LNF.Scheduler;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace LNF.Impl.Billing
{
    public class ToolDataRepository : RepositoryBase, IToolDataRepository
    {
        private readonly IBillingTypeRepository _billingType;

        public ToolDataRepository(ISessionManager mgr, IBillingTypeRepository billingType) : base(mgr)
        {
            _billingType = billingType;
        }

        //RoomID might be null
        public IRoom GetRoom(IToolData item)
        {
            if (item.RoomID.HasValue)
                return Require<Room>(item.RoomID.Value).CreateModel<IRoom>();
            else
                return null;
        }

        //ReservationID might be null
        public IReservation GetReservation(IToolData item)
        {
            if (item.ReservationID.HasValue)
                return Require<Reservation>(item.ReservationID.Value).CreateModel<IReservation>();
            else
                return null;
        }

        public IBillingType GetBillingType(IToolData item)
        {
            return _billingType.GetBillingType(item.ClientID, item.AccountID, item.Period);
        }

        public DataTable ReadToolData(DateTime period, int clientId = 0, int resourceId = 0)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var result = GetReader(conn).ReadToolData(period, clientId, resourceId);
                conn.Close();
                return result;
            }
        }

        public DataSet ReadToolDataClean(DateTime sd, DateTime ed, int clientId = 0, int resourceId = 0)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var result = GetReader(conn).ReadToolDataClean(sd, ed, clientId, resourceId);
                conn.Close();
                return result;
            }
        }

        public DataTable ReadToolDataRaw(DateTime sd, DateTime ed, int clientId = 0)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var result = GetReader(conn).ReadToolDataRaw(sd, ed, clientId);
                conn.Close();
                return result;
            }
        }

        public DataTable ReadToolUtilization(string sumCol, bool includeForgiven, DateTime sd, DateTime ed)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                var result = GetReader(conn).ReadToolUtilization(sumCol, includeForgiven, sd, ed);
                conn.Close();
                return result;
            }
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["cnSselData"].ConnectionString);
        }

        private ToolDataReader GetReader(SqlConnection conn)
        {
            return new ToolDataReader(conn);
        }
    }
}
