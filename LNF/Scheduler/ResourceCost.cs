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
        public int ChargeTypeID { get; }

        public Cost Cost
        {
            get
            {
                return _costs.FirstOrDefault(c => c.ChargeTypeID == ChargeTypeID);
            }
        }

        public Cost OverTimeCost
        {
            get
            {
                return _overtime.FirstOrDefault(c => c.ChargeTypeID == ChargeTypeID);
            }
        }

        private ResourceCost(int resourceId, int chargeTypeId)
        {
            ResourceID = resourceId;
            ChargeTypeID = chargeTypeId;
        }

        public ResourceCost(IEnumerable<Cost> costs, int resourceId, int chargeTypeId, DateTime cutoff) : this(resourceId, chargeTypeId)
        {
            if (costs == null)
                throw new ArgumentNullException("costs");

            _costs = costs.Where(x => x.TableNameOrDescription == "ToolCost" && x.RecordID == resourceId && x.EffDate <= cutoff).OrderByDescending(x => x.EffDate).ToList();
            _overtime = costs.Where(x => x.TableNameOrDescription == "ToolOvertimeCost" && x.EffDate <= cutoff).OrderByDescending(x => x.EffDate).ToList();
        }

        public ResourceCost(IEnumerable<Cost> costs, int resourceId, int chargeTypeId) : this(resourceId, chargeTypeId)
        {
            if (costs == null)
                throw new ArgumentNullException("costs");

            _costs = costs.Where(x => x.TableNameOrDescription == "ToolCost" && x.RecordID == resourceId).OrderByDescending(x => x.EffDate).ToList();
            _overtime = costs.Where(x => x.TableNameOrDescription == "ToolOvertimeCost").OrderByDescending(x => x.EffDate).ToList();
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

        public static IList<ResourceCost> GetAll(IEnumerable<Cost> costs, int resourceId)
        {
            return DA.Current.Query<ChargeType>().Select(x => new ResourceCost(costs, resourceId, x.ChargeTypeID)).ToList();
        }
    }
}
