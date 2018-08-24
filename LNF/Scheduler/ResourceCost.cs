using LNF.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Scheduler
{
    public class ResourceCost
    {
        public int ResourceID => Cost?.RecordID ?? 0;
        public int ChargeTypeID => Cost?.ChargeTypeID ?? 0;
        public string AcctPer => Cost?.AcctPer ?? string.Empty;
        public decimal AddVal => Cost?.AddVal ?? 0;
        public decimal MulVal => Cost?.MulVal ?? 0;

        public CostItem Cost { get; }

        public CostItem OverTimeCost { get; }

        private ResourceCost(CostItem cost, CostItem overTimeCost)
        {
            Cost = cost;
            OverTimeCost = overTimeCost;
        }

        public decimal PerUseRate()
        {
            decimal result = AddVal;

            if (AcctPer == "Per Use")
                result += MulVal;

            return result;
        }

        public decimal HourlyRate()
        {
            if (AcctPer == "Hourly")
                return MulVal;
            else
                return 0;
        }

        public decimal BookingFeeMultiplier()
        {
            return 0.1M; // should get this from the db - is this ToolMissedReservCost in the Cost table?
        }

        public decimal BookingFeeRate()
        {
            if (AcctPer == "Hourly")
                return MulVal * BookingFeeMultiplier();
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

        public override string ToString()
        {
            return string.Format("{0:C}/use + {1:C}/hr", PerUseRate(), HourlyRate());
        }

        /// <summary>
        /// Creates ResourceCost objects for the given CostItem objects.
        /// </summary>
        /// <param name="costs">A filtered list of CostItem objects. This should only contain the most recent costs for each Resource/ChargeType combination.</param>
        public static IEnumerable<ResourceCost> CreateResourceCosts(IEnumerable<CostItem> costs)
        {
            if (costs == null)
                throw new ArgumentNullException("costs");

            IList<ResourceCost> result = new List<ResourceCost>();

            var overtime = costs.Where(x => x.TableNameOrDescription == "ToolOvertimeCost");

            foreach(var c in costs)
            {
                var otc = overtime.FirstOrDefault(x => x.ChargeTypeID == c.ChargeTypeID);
                result.Add(new ResourceCost(c, otc));
            }

            return result;
        }
    }
}
