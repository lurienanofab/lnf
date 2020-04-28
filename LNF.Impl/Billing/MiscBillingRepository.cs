using LNF.Billing;
using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Billing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Impl.Billing
{
    public class MiscBillingRepository : RepositoryBase, IMiscBillingRepository
    {
        public MiscBillingRepository(ISessionManager mgr) : base(mgr) { }

        public int CreateMiscBillingCharge(MiscBillingChargeCreateArgs args)
        {
            return Session.CreateSQLQuery("EXEC sselData.dbo.MiscBillingCharge_Insert @AccountID = :AccountID, @ActDate = :ActDate, @ClientID = :ClientID, @Description = :Description, @Period = :Period, @Quantity = :Quantity, @SUBType = :SUBType, @UnitCost = :UnitCost")
                .SetParameter("AccountID", args.AccountID)
                .SetParameter("ActDate", args.ActDate)
                .SetParameter("ClientID", args.ClientID)
                .SetParameter("Description", args.Description)
                .SetParameter("Period", args.Period)
                .SetParameter("Quantity", args.Quantity)
                .SetParameter("SUBType", args.SUBType)
                .SetParameter("UnitCost", args.UnitCost)
                .UniqueResult<int>();
        }

        public int UpdateMiscBilling(MiscBillingChargeUpdateArgs args)
        {
            return Session.CreateSQLQuery("EXEC sselData.dbo.MiscBillingCharge_Update @Description = :Description, @ExpID = :ExpID, @Period = :Period, @Quantity = :Quantity, @UnitCost = :UnitCost")
                .SetParameter("Description", args.Description)
                .SetParameter("ExpID", args.ExpID)
                .SetParameter("Period", args.Period)
                .SetParameter("Quantity", args.Quantity)
                .SetParameter("UnitCost", args.UnitCost)
                .ExecuteUpdate();
        }

        public int DeleteMiscBillingCharge(int expId)
        {
            return Session.CreateSQLQuery("EXEC sselData.dbo.MiscBillingCharge_Delete @ExpID = :ExpID")
                .SetParameter("ExpID", expId)
                .ExecuteUpdate();
        }

        public IMiscBillingCharge GetMiscBillingCharge(int expId)
        {
            return Session.CreateSQLQuery("EXEC sselData.dbo.MiscBillingCharge_Select @Action = 'ByExpID', @ExpID = :ExpID")
                .SetParameter("ExpID", expId)
                .List<MiscBillingCharge>()
                .FirstOrDefault()
                .CreateModel<IMiscBillingCharge>();
        }

        public IEnumerable<IMiscBillingCharge> GetMiscBillingCharges(DateTime period, int clientId = 0, int accountId = 0, string[] types = null, bool? active = null)
        {
            var query = Session.CreateSQLQuery("EXEC sselData.dbo.MiscBillingCharge_Select @Action = 'Search', @Period = :Period, @ClientID = :ClientID, @AccountID = :AccountID, @Active = :Active")
                .SetParameter("Period", period)
                .SetParameter("Active", active);

            if (clientId > 0)
                query.SetParameter("ClientID", clientId);
            else
                query.SetParameter("ClientID", null);

            if (accountId > 0)
                query.SetParameter("AccountID", accountId);
            else
                query.SetParameter("AccountID", null);

            var list = query.List<MiscBillingCharge>();

            var result = list.Where(x => types.Contains(x.SUBType)).CreateModels<IMiscBillingCharge>();

            return result;
        }
    }
}
