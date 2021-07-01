using LNF.Ordering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;

namespace LNF.Impl.Ordering
{
    public static class Extensions
    {
        public static IEnumerable<ITracking> CreateModels(this IQueryable<Repository.Ordering.Tracking> query)
        {
            var list = query.ToList();
            var result = list.Select(CreateModel).ToList();
            return result;
        }

        public static ITracking CreateModel(this Repository.Ordering.Tracking src)
        {
            return new Tracking
            {
                TrackingID = src.TrackingID,
                TrackingCheckpointID = src.TrackingCheckpoint.TrackingCheckpointID,
                POID = src.PurchaseOrder.POID,
                ClientID = src.Client.ClientID,
                TrackingData = src.TrackingData,
                TrackingDateTime = src.TrackingDateTime,
                CheckpointName = src.TrackingCheckpoint.CheckpointName,
                LName = src.Client.LName,
                FName = src.Client.FName
            };
        }
    }
}
