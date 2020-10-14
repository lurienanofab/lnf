using LNF.Data;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Impl.Repository.Data
{
    public static class SessionExtensions
    {
        public static IEnumerable<CurrentCost> FindCurrentCosts(this ISession session, string[] tables, int recordId = 0, int chargeTypeId = 0)
        {
            return session.Query<CurrentCost>().Where(x =>
                tables.Contains(x.TableNameOrDescription)
                && (chargeTypeId == 0 || x.ChargeTypeID == chargeTypeId)
                && (recordId == 0 || x.RecordID == recordId || x.RecordID == null || x.RecordID == 0)).ToList();
        }

        public static IEnumerable<Cost> FindCosts(this ISession session, string[] tables, DateTime? cutoff = null, int recordId = 0, int chargeTypeId = 0)
        {
            var query = session.Query<Cost>().Where(x =>
                tables.Contains(x.TableNameOrDescription)
                && (cutoff == null || x.EffDate < cutoff)
                && (chargeTypeId == 0 || x.ChargeTypeID == chargeTypeId)
                && (recordId == 0 || x.RecordID == recordId || x.RecordID == null || x.RecordID == 0));

            var list = query.ToList();

            var agg = list.GroupBy(x => new { x.ChargeTypeID, x.TableNameOrDescription, x.RecordID })
                .Select(g => new
                {
                    g.Key.ChargeTypeID,
                    g.Key.TableNameOrDescription,
                    g.Key.RecordID,
                    EffDate = g.Max(m => m.EffDate)
                });

            var result = list.Join(agg,
                    o => new { o.ChargeTypeID, o.TableNameOrDescription, o.RecordID, o.EffDate },
                    i => new { i.ChargeTypeID, i.TableNameOrDescription, i.RecordID, i.EffDate },
                    (o, i) => o)
                .OrderBy(x => x.ChargeTypeID)
                .ThenBy(x => x.TableNameOrDescription)
                .ThenBy(x => x.RecordID)
                .ThenBy(x => x.EffDate)
                .ToList();

            return result;
        }

        public static GlobalCost SelectGlobalCost(this ISession session)
        {
            // At this time there is only one.
            // If missing for some reason throw an error.
            return session.SelectGlobalCosts().First();
        }

        public static IQueryable<GlobalCost> SelectGlobalCosts(this ISession session)
        {
            return session.Query<GlobalCost>();
        }

        public static IQueryable<ClientInfo> SelectClientInfoByPriv(this ISession session, ClientPrivilege priv)
        {
            var query = session.Query<ClientInfo>().Where(x => (x.Privs & priv) > 0);
            return query;
        }

        public static ClientInfo FindClientInfo(this ISession session, string username)
        {
            return session.Query<ClientInfo>().FirstOrDefault(x => x.UserName == username);
        }
    }
}
