using System;

namespace LNF.Reporting.Individual
{
    public class ResourceDetailItem
    {
        public int ReservationID { get; set; }
        public DateTime ActDate { get; set; }
        public string Started { get; set; }
        public string Cancelled { get; set; }
        public string CancelledBeforeCutoff { get; set; }
        public double ActivatedUsed { get; set; }
        public double ActivatedUnused { get; set; }
        public double Overtime { get; set; }
        public decimal OvertimeFee { get; set; }
        public double UnstartedUnused { get; set; }
        public decimal BookingFee { get; set; }
        public double Transferred { get; set; }
        public double Forgiven { get; set; }
        public decimal ResourceRate { get; set; }
        public decimal LineTotal { get; set; }

        public override string ToString() => ReservationID.ToString();
    }
}
