using LNF.Data;
using LNF.Models.Data;
using LNF.Repository.Data;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Impl.DataAccess.Data
{
    public class DataRepository : IDataRepository
    {
        protected ISession Session { get; }

        public DataRepository(ISession session)
        {
            Session = session;
        }

        public IEnumerable<CostModel> FindCosts(string tableNameOrDescription, DateTime chargeDate, int? chargeTypeId = null, int? recordId = null)
        {
            IQueryable<Cost> query;

            if (chargeTypeId.HasValue)
            {
                if (recordId.HasValue)
                    query = Session.Query<Cost>().Where(x => x.TableNameOrDescription == tableNameOrDescription && x.EffDate <= chargeDate && x.ChargeTypeID == chargeTypeId.Value && x.RecordID == recordId.Value);
                else
                    query = Session.Query<Cost>().Where(x => x.TableNameOrDescription == tableNameOrDescription && x.EffDate <= chargeDate && x.ChargeTypeID == chargeTypeId.Value);
            }
            else
            {
                if (recordId.HasValue)
                    query = Session.Query<Cost>().Where(x => x.TableNameOrDescription == tableNameOrDescription && x.EffDate <= chargeDate && x.RecordID == recordId.Value);
                else
                    query = Session.Query<Cost>().Where(x => x.TableNameOrDescription == tableNameOrDescription && x.EffDate <= chargeDate);
            }

            var models = CreateCostModels(query);

            var agg = models.GroupBy(c => new { c.ChargeTypeID, c.TableNameOrDescription, c.RecordID })
                .Select(g => new { g.Key.ChargeTypeID, TableNameOrDescription = g.Key.TableNameOrDescription, RecordID = g.Key.RecordID, EffDate = g.Max(m => m.EffDate) }).ToList();

            var result = models.Join(agg, o => new { o.ChargeTypeID, o.TableNameOrDescription, o.RecordID, o.EffDate }, i => new { i.ChargeTypeID, i.TableNameOrDescription, i.RecordID, i.EffDate }, (o, i) => o).ToList();

            return result;
        }

        private IEnumerable<CostModel> CreateCostModels(IQueryable<Cost> query)
        {
            var join = query.Join(Session.Query<ChargeType>(),
                o => o.ChargeTypeID,
                i => i.ChargeTypeID,
                (o, i) => new { Cost = o, ChargeType = i });

            return join.Select(x => new CostModel()
            {
                CostID = x.Cost.CostID,
                ChargeTypeID = x.ChargeType.ChargeTypeID,
                ChargeTypeName = x.ChargeType.ChargeTypeName,
                TableNameOrDescription = x.Cost.TableNameOrDescription,
                RecordID = x.Cost.RecordID,
                AcctPer = x.Cost.AcctPer,
                AddVal = x.Cost.AddVal,
                MulVal = x.Cost.MulVal,
                EffDate = x.Cost.EffDate,
                CreatedDate = x.Cost.CreatedDate
            }).ToList();
        }
    }
}
