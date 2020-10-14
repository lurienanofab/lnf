using LNF.Billing;
using LNF.CommonTools;
using LNF.Data;
using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Billing;
using LNF.Impl.Repository.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Caching;

namespace LNF.Impl.Billing
{
    public class BillingTypeRepository : RepositoryBase, IBillingTypeRepository
    {
        private readonly ObjectCache _cache = new MemoryCache("BillingTypeRepositoryCache");

        public BillingTypeRepository(ISessionManager mgr) : base(mgr) { }

        public IEnumerable<IBillingType> GetBillingTypes()
        {
            IEnumerable<IBillingType> result;

            if (_cache["all"] == null)
            {
                result = Session.Query<BillingType>().ToList();
                _cache.Add("all", result, DateTimeOffset.Now.AddHours(1));
            }
            else
            {
                result = (IEnumerable<IBillingType>)_cache["all"];
            }

            return result;
        }

        public IBillingType GetBillingType(int billingTypeId)
        {
            return Session.Get<BillingType>(billingTypeId);
        }

        public IBillingType GetBillingType(int clientId, int accountId, DateTime period) => Session.GetBillingType(clientId, accountId, period);

        public IBillingType GetBillingType(DateTime period, int clientId, int orgId, IEnumerable<IHoliday> holidays)
        {
            // always add one more month for period, because we allow changes made during the current month that will take effect
            // as long as it's before the 4th business day of business
            DateTime p = Utility.NextBusinessDay(period.AddMonths(1), holidays);

            var co = Session.Query<ClientOrg>().FirstOrDefault(x => x.Client.ClientID == clientId && x.Org.OrgID == orgId);

            var cobtLog = Session.Query<ClientOrgBillingTypeLog>().FirstOrDefault(x => x.ClientOrgID == co.ClientOrgID && x.EffDate < p && (x.DisableDate == null || x.DisableDate > p));

            if (cobtLog != null)
                return Require<BillingType>(cobtLog.BillingTypeID);
            else
                return BillingTypes.Instance.Default;
        }

        public void UpdateBilling(int clientId, DateTime period, DateTime now)
        {
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["cnSselData"].ConnectionString))
            {
                conn.Open();
                bool temp = Utility.IsCurrentPeriod(period);
                var step1 = new BillingDataProcessStep1(conn);
                step1.PopulateToolBilling(period, now, clientId, temp);
                step1.PopulateRoomBilling(period, now, clientId, temp);
                conn.Close();
            }
        }

        public IEnumerable<IToolBilling> SelectToolBillingData(int clientId, DateTime period, bool temp)
        {
            string sql = "EXEC Billing.dbo.ToolData_Select @Action = 'ForToolBilling', @Period = :Period, @ClientID = :ClientID";

            var query = Session.CreateSQLQuery(sql).SetParameter("Period", period).SetParameter("ClientID", clientId);

            IEnumerable<IToolBilling> result;

            if (temp)
                result = query.List<ToolBillingTemp>().CreateModels<IToolBilling>();
            else
                result = query.List<ToolBilling>().CreateModels<IToolBilling>();

            return result;
        }

        /// <summary>
        /// The final billed amount based on BillingType and Tool.
        /// </summary>
        public decimal GetLineCost(IToolBilling item) => ToolBillingUtility.GetLineCost(item);

        /// <summary>
        /// The final billed amount based on BillingType and Room.
        /// </summary>
        public decimal GetLineCost(IRoomBilling item) => RoomBillingUtility.GetLineCost(item);

        /// <summary>
        /// Calculate the true room cost based on billing types.
        /// </summary>
        public void CalculateRoomLineCost(DataTable dt) => RoomBillingUtility.CalculateRoomLineCost(dt);

        /// <summary>
        /// Calculate the true tool cost based on billing types.
        /// </summary>
        public void CalculateToolLineCost(DataTable dt) => ToolBillingUtility.CalculateToolLineCost(dt);

        public IEnumerable<IClientOrgBillingTypeLog> GetClientOrgBillingTypeLogs(int clientOrgId, DateTime? disableDate)
        {
            return Session.Query<ClientOrgBillingTypeLog>().Where(x => x.ClientOrgID == clientOrgId && x.DisableDate == disableDate).CreateModels<IClientOrgBillingTypeLog>();
        }

        public IEnumerable<IClientOrgBillingTypeLog> GetActiveClientOrgBillingTypeLogs(DateTime sd, DateTime ed)
        {
            return Session.ActiveClientOrgBillingTypeLogQuery(sd, ed).CreateModels<IClientOrgBillingTypeLog>();
        }

        public IClientOrgBillingTypeLog GetActiveClientOrgBillingTypeLog(int clientOrgId, DateTime sd, DateTime ed)
        {
            return Session.ActiveClientOrgBillingTypeLogQuery(sd, ed).FirstOrDefault(x => x.ClientOrgID == clientOrgId).CreateModel<IClientOrgBillingTypeLog>();
        }

        public IClientOrgBillingTypeLog UpdateClientOrgBillingTypeLog(int clientOrgId, int billingTypeId)
        {
            var existing = Session.Query<ClientOrgBillingTypeLog>().FirstOrDefault(x => x.ClientOrgID == clientOrgId && x.DisableDate == null);

            if (existing != null)
            {
                //disable the existing entry
                existing.DisableDate = DateTime.Now.Date;
                Session.Update(existing);
            }

            //create an entry for the new billing type
            var updated = new ClientOrgBillingTypeLog
            {
                ClientOrgID = clientOrgId,
                BillingTypeID = billingTypeId,
                EffDate = DateTime.Now,
                DisableDate = null
            };

            Session.Save(updated);

            return updated.CreateModel<IClientOrgBillingTypeLog>();
        }
    }
}
