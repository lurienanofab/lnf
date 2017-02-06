using LNF.Repository;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Scheduler
{
    public class ResourceCost
    {
        private IEnumerable<Cost> _costs;
        private IEnumerable<Cost> _overtime;

        public int ResourceID { get; }
        public ChargeType ChargeType { get; }

        public Cost Cost
        {
            get
            {
                return _costs.FirstOrDefault(c => c.ChargeType == ChargeType);
            }
        }

        public Cost OverTimeCost
        {
            get
            {
                return _overtime.FirstOrDefault(c => c.ChargeType == ChargeType);
            }
        }

        public ResourceCost(int resourceId, ChargeType chargeType, DateTime? cutoff = null)
        {
            ChargeType = chargeType;
            ResourceID = resourceId;

            string[] tableName = { "ToolCost", "ToolOvertimeCost" };
            IQueryable<Cost> query;

            if (cutoff.HasValue)
                query = DA.Current.Query<Cost>().Where(x => tableName.Contains(x.TableNameOrDescription) && x.EffDate <= cutoff.Value).OrderByDescending(c => c.EffDate);
            else
                query = DA.Current.Query<Cost>().Where(x => tableName.Contains(x.TableNameOrDescription)).OrderByDescending(c => c.EffDate);

            _costs = query.Where(x => x.TableNameOrDescription == "ToolCost" && x.RecordID == resourceId).ToList();
            _overtime = query.Where(x => x.TableNameOrDescription == "ToolOvertimeCost").ToList();
        }

        public ResourceCost(IEnumerable<Cost> costs, ChargeType chargeType)
        {
            _costs = costs;

            ChargeType = chargeType;

            if (costs.Count() > 0)
                ResourceID = costs.First().RecordID;
        }

        public decimal PerUseRate()
        {
            decimal result = Cost.AddVal;
            if (Cost.AcctPer == "Per Use")
                result += Cost.MulVal;
            return result;
        }

        public decimal HourlyRate()
        {
            if (Cost.AcctPer == "Hourly")
                return Cost.MulVal;
            else
                return 0;
        }

        public decimal BookingFeeMultiplier()
        {
            return 0.1M; // should get this from the db - is this ToolMissedReservCost in the Cost table?
        }

        public decimal BookingFeeRate()
        {
            if (Cost.AcctPer == "Hourly")
                return Cost.MulVal * BookingFeeMultiplier();
            else
                return 0;
        }

        public decimal PerUseBookingFeeRate()
        {
            return PerUseRate() * BookingFeeMultiplier();
        }

        public decimal OverTimeRate()
        {
            // This is how the overtime multiplier is calculated in Billing.dbo.ToolData_Select @Action = 'ForToolBilling'
            //      (SELECT c2.MulVal + 1.0 FROM @costs c2 WHERE c2.TableNameOrDescript = 'ToolOvertimeCost' AND c2.ChargeTypeID = ot.ChargeTypeID)

            decimal result = HourlyRate() * (OverTimeCost.MulVal + 1M);
            return result;
        }

        public static IList<ResourceCost> GetAll(int resourceId)
        {
            return DA.Current.Query<ChargeType>().Select(x => new ResourceCost(resourceId, x, null)).ToList();
        }
    }
}
