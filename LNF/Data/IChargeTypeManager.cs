using System.Linq;
using LNF.Repository;
using LNF.Repository.Data;

namespace LNF.Data
{
    public interface IChargeTypeManager : IManager
    {
        Account GetAccount(ChargeType item);
        IQueryable<OrgType> OrgTypes(ChargeType item);
    }
}