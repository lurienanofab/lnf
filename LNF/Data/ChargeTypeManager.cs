using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Data;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Data
{
    public class ChargeTypeManager : ManagerBase, IChargeTypeManager
    {
        public ChargeTypeManager(IProvider provider) : base(provider) { }

        public IAccount GetAccount(IChargeType chargeType)
        {
            return Session.Single<AccountInfo>(chargeType.AccountID).CreateModel<IAccount>();
        }

        public IEnumerable<IOrgType> OrgTypes(int chargeTypeId)
        {
            return Session.Query<OrgType>().Where(x => x.ChargeType.ChargeTypeID == chargeTypeId).CreateModels<IOrgType>();
        }
    }
}
