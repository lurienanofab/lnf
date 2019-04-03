using System;

namespace LNF.Models.Scheduler
{
    public interface IReservationHistory
    {
        int ReservationID { get; set; }
        DateTime BeginDateTime { get; set; }
        DateTime EndDateTime { get; set; }
        int AccountID { get; set; }
        string ActionSource { get; set; }
        double ChargeMultiplier { get; set; }
        int? LinkedReservationID { get; set; }
        int? ModifiedByClientID { get; set; }
        DateTime ModifiedDateTime { get; set; }
        int ReservationHistoryID { get; set; }
        string UserAction { get; set; }
    }
}