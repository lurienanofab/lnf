using LNF.Models.Data;
using System.Collections.Generic;

namespace OnlineServices.Api.Data
{
    public class ChargeTypeManager : IChargeTypeManager
    {
        public IAccount GetAccount(IChargeType chargeType)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IOrgType> OrgTypes(int chargeTypeId)
        {
            throw new System.NotImplementedException();
        }
    }
}