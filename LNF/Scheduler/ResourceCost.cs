using LNF.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Scheduler
{
    public class ResourceCost
    {
        public int ResourceID { get; }
        public int ChargeTypeID { get; }

        public string AcctPer { get; }
        public decimal AddVal { get; }
        public decimal MulVal { get; }

        public decimal OvertimeMultiplier { get; }

        private ResourceCost(ICost cost, ICost overtime)
        {
            if (cost == null)
                throw new ArgumentNullException("cost");

            if (cost.TableNameOrDescription != "ToolCost")
                throw new ArgumentException($"Incorrect TableNameOrDescription: Expected ToolCost, got {cost.TableNameOrDescription}", "cost");

            if (overtime == null)
                throw new ArgumentNullException("overtime");

            if (overtime.TableNameOrDescription != "ToolOvertimeCost")
                throw new ArgumentException($"Incorrect TableNameOrDescription: Expected ToolOvertimeCost, got {overtime.TableNameOrDescription}", "overtime");

            if (cost.ChargeTypeID != overtime.ChargeTypeID)
                throw new ArgumentException($"ChargeTypeID mismatch: cost.ChargeTypeID = {cost.ChargeTypeID}, overtime.ChargeTypeID = {overtime.ChargeTypeID}");

            ResourceID = cost.RecordID;
            ChargeTypeID = cost.ChargeTypeID;
            AcctPer = cost.AcctPer;
            AddVal = cost.AddVal;
            MulVal = cost.MulVal;
            OvertimeMultiplier = overtime.MulVal;
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

        public decimal OverTimeRate()
        {
            // This is how the overtime multiplier is calculated in Billing.dbo.ToolData_Select @Action = 'ForToolBilling'
            //      (SELECT c2.MulVal + 1.0 FROM @costs c2 WHERE c2.TableNameOrDescript = 'ToolOvertimeCost' AND c2.ChargeTypeID = ot.ChargeTypeID)

            decimal result = HourlyRate() * (OvertimeMultiplier + 1M);
            return result;
        }

        public override string ToString()
        {
            return string.Format("{0:C}/use + {1:C}/hr", PerUseRate(), HourlyRate());
        }

        /// <summary>
        /// Creates ResourceCost objects for the given CostItem objects.
        /// </summary>
        public static IEnumerable<ResourceCost> CreateResourceCosts(IEnumerable<ICost> costs)
        {
            if (costs == null)
                throw new ArgumentNullException("costs");

            IList<ResourceCost> result = new List<ResourceCost>();

            var regular = costs.Where(x => x.TableNameOrDescription == "ToolCost").ToList();
            var overtime = costs.Where(x => x.TableNameOrDescription == "ToolOvertimeCost").ToList();

            foreach (var c in regular)
            {
                var otc = overtime.FirstOrDefault(x => x.ChargeTypeID == c.ChargeTypeID);
                result.Add(new ResourceCost(c, otc));
            }

            return result;
        }
    }
}
