using LNF.Repository;
using LNF.Repository.Data;
using System.Linq;

namespace LNF.Data
{
    public class ChargeTypeManager : ManagerBase, IChargeTypeManager
    {
        public ChargeTypeManager(ISession session) : base(session) { }

        public Account GetAccount(ChargeType item)
        {
            return Session.Single<Account>(item.AccountID);
        }

        public IQueryable<OrgType> OrgTypes(ChargeType item)
        {
            return Session.Query<OrgType>().Where(x => x.ChargeType.ChargeTypeID == item.ChargeTypeID);
        }
    }
}
