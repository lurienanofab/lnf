using System;

namespace LNF.Models.Scheduler
{
    /// <summary>
    /// A reservation recurrence by a client on a lab resource
    /// </summary>
    public class ReservationRecurrenceItem : IReservationRecurrence
    {
        public int RecurrenceID { get; set; }

        public int ResourceID { get; set; }

        public string ResourceName { get; set; }

        /// <summary>
        /// The id for the Client who made the reservation recurrence
        /// </summary>
        public int ClientID { get; set; }

        /// <summary>
        /// The last name of the Client who made the reservation recurrence
        /// </summary>
        public string LName { get; set; }

        /// <summary>
        /// The first name of the Client who made the reservation recurrence
        /// </summary>
        public string FName { get; set; }

        public DateTime EndTime { get; set; }

        public bool IsActive { get; set; }

        public int AccountID { get; set; }

        public int ActivityID { get; set; }

        public string AccountName { get; set; }

        public string ActivityName { get; set; }

        public DateTime CreatedOn { get; set; }

        public bool AutoEnd { get; set; }

        public bool KeepAlive { get; set; }

        public int PatternID { get; set; }

        public string PatternName { get; set; }

        public int PatternParam1 { get; set; }

        public int? PatternParam2 { get; set; }

        public DateTime BeginDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime BeginTime { get; set; }

        public double Duration { get; set; }

        public string Notes { get; set; }
    }
}
