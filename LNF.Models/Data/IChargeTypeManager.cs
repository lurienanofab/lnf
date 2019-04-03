using System.Collections.Generic;

namespace LNF.Models.Data
{
    public interface IChargeTypeManager
    {
        IAccount GetAccount(IChargeType chargeType);
        IEnumerable<IOrgType> OrgTypes(int chargeTypeId);
    }
}