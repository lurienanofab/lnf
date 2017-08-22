using LNF.Repository.Scheduler;
using System;

namespace LNF.Billing
{
    public class ReservationDurationItem
    {
        public ReservationDurationItem(ReservationDateRange.Reservation rsv, TimeSpan utilizedDuration)
        {
            Reservation = rsv;
            UtilizedDuration = utilizedDuration;
        }

        public ReservationDateRange.Reservation Reservation { get; }

        /// <summary>
        /// The total chargeable time minus the transferred duration (includes overtime).
        /// </summary>
        public TimeSpan UtilizedDuration { get; }

        /// <summary>
        /// The scheduled reservation time.
        /// </summary>
        public TimeSpan ScheduledDuration
        {
            get { return Reservation.EndDateTime - Reservation.BeginDateTime; }
        }

        /// <summary>
        /// The actual time used.
        /// </summary>
        public TimeSpan ActualDuration
        {
            get
            {
                if (Reservation.ActualEndDateTime.HasValue && Reservation.ActualBeginDateTime.HasValue)
                    return Reservation.ActualEndDateTime.Value - Reservation.ActualBeginDateTime.Value;
                else
                    return TimeSpan.Zero;
            }
        }

        /// <summary>
        /// The maximum chargeable time. 
        /// </summary>
        public TimeSpan ChargeDuration
        {
            get { return Reservation.ChargeEndDateTime - Reservation.ChargeBeginDateTime; }
        }

        /// <summary>
        /// The time the reservation exceeded the scheduled end time. Will be zero when reservations end early.
        /// </summary>
        public TimeSpan OverTimeDuration
        {
            get
            {
                if (Reservation.ActualEndDateTime.HasValue && Reservation.ActualEndDateTime.Value > Reservation.EndDateTime)
                    return Reservation.ActualEndDateTime.Value - Reservation.EndDateTime;
                else
                    return TimeSpan.Zero;
            }
        }

        /// <summary>
        /// The time covered by overlapping reservations.
        /// </summary>
        public TimeSpan TransferredDuration
        {
            get { return ChargeDuration - UtilizedDuration; }
        }

        /// <summary>
        /// The time billed at the standard rate (excludes overtime and transferred).
        /// </summary>
        public TimeSpan StandardDuration
        {
            get { return UtilizedDuration - OverTimeDuration; }
        }
    }
}
