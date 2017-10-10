using System;

namespace LNF.Models.Data.Utility.BillingChecks
{
    public class AutoEndProblem
    {
        public int ReservationID { get; set; }

        public string AutoEndType { get; set; }

        public bool AutoEndReservation { get; set; }

        public TimeSpan AutoEndResource { get; set; }

        public TimeSpan Duration { get; set; }

        public DateTime EndDateTime { get; set; }

        public DateTime? ActualEndDateTime { get; set; }

        public DateTime ActualEndDateTimeExpected { get; set; }

        public TimeSpan Diff { get; set; }

        public DateTime ActualEndDateTimeCorrected { get; set; }

        public double ChargeMultiplier { get; set; }
    }
}
