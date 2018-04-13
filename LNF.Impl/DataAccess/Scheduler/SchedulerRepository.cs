using LNF.Data;
using LNF.Models.Data;
using LNF.Models.Scheduler;
using LNF.Repository.Scheduler;
using LNF.Scheduler;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Impl.DataAccess.Scheduler
{
    public class SchedulerRepository : ISchedulerRepository
    {
        protected IContext Context { get; }
        protected IDataRepository DataRepository { get; }
        protected ISession Session { get; }

        public SchedulerRepository(IContext context, IDataRepository data, ISession session)
        {
            Context = context;
            DataRepository = data;
            Session = session;
        }

        public IEnumerable<ResourceCostModel> GetToolCosts(DateTime cutoff, int resourceId)
        {
            string key = "ResourceCosts#" + resourceId.ToString();

            IList<ResourceCostModel> result = Context.GetItem<IList<ResourceCostModel>>(key);

            if (result == null || result.Count == 0)
            {
                var costs = DataRepository.FindCosts("ToolCost", cutoff, null, resourceId);
                result = CreateResourceCostModels(costs).ToList();
                Context.SetItem(key, result);
            }

            return result;
        }

        public IEnumerable<ResourceCostModel> GetToolCosts(DateTime cutoff, int resourceId, int chargeTypeId)
        {
            return GetToolCosts(cutoff, resourceId).Where(x => x.ChargeTypeID == chargeTypeId).ToList();
        }

        private IEnumerable<ResourceCostModel> CreateResourceCostModels(IEnumerable<CostModel> source)
        {
            return source.Select(x => new ResourceCostModel()
            {
                ResourceID = x.RecordID,
                ChargeTypeID = x.ChargeTypeID,
                ChargeTypeName = x.ChargeTypeName,
                AddVal = x.AddVal,
                MulVal = x.MulVal
            }).ToList();
        }

        public IEnumerable<Reservation> SelectRecent(int resourceId)
        {
            // procReservationSelect @Action = 'SelectRecent'

            //SELECT TOP 6 Rv.ReservationID, Rv.BeginDateTime, Rv.EndDateTime,
            //  Rv.ClientID, sselData.dbo.udf_GetDisplayName(RV.ClientID) AS DisplayName
            //FROM dbo.Reservation Rv
            //WHERE Rv.ResourceID = @ResourceID
            //ORDER BY ABS (datediff (second, Rv.BeginDateTime, getdate())) ASC

            // Need to use criterion because of the complicated order by clause.

            var result = Session.CreateCriteria<Reservation>()
                .Add(Restrictions.Eq("Resource.ResourceID", resourceId))
                .AddOrder(Order.Asc(Projections.SqlFunction("abs", NHibernateUtil.Int32, DateProjections.DateDiff(DatePart.Second, Projections.Property<Reservation>(x => x.BeginDateTime), DateProjections.GetDate()))))
                .SetMaxResults(6)
                .List<Reservation>();

            return result;
        }
    }
}
