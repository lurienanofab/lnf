using System;

namespace LNF.Billing
{
    public class ReservationDurationItem
    {
        public ReservationDurationItem(ReservationDateRangeItem rsv, TimeSpan utilizedDuration)
        {
            Reservation = rsv;
            UtilizedDuration = utilizedDuration;
        }

        public ReservationDateRangeItem Reservation { get; }

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

        public virtual TimeSpan ActivatedUsedDuration
        {
            get
            {
                // if a reservation is started IsCancelledBeforeAllowedTime must be false, right?
                double activatedUsed = (Reservation.IsStarted && !Reservation.IsCancelledBeforeCutoff) ? (ActualDuration.TotalMinutes - OverTimeDuration.TotalMinutes) : 0;
                return TimeSpan.FromMinutes(activatedUsed);
            }
        }

        public virtual TimeSpan ActivatedUnusedDuration
        {
            get
            {
                // if a reservation is started IsCancelledBeforeAllowedTime must be false, right?
                double activatedUnused = (Reservation.IsStarted && !Reservation.IsCancelledBeforeCutoff) ? Math.Max(ChargeDuration.TotalMinutes - ActualDuration.TotalMinutes, 0) : 0;
                return TimeSpan.FromMinutes(activatedUnused);
            }
        }

        public virtual TimeSpan UnactivatedDuration
        {
            get
            {
                // now it makes sense to check IsCancelledBeforeAllowedTime
                double unstartedUnused = (!Reservation.IsStarted && !Reservation.IsCancelledBeforeCutoff) ? ChargeDuration.TotalMinutes : 0;
                return TimeSpan.FromMinutes(unstartedUnused);
            }
        }

        /// <summary>
        /// The time the reservation exceeded the scheduled end time. Will be zero when reservations end early.
        /// </summary>
        public TimeSpan OverTimeDuration
        {
            get
            {
                // overtime is calculated in whole minutes (using floor)
                if (Reservation.ActualEndDateTime.HasValue && Reservation.ActualEndDateTime.Value > Reservation.EndDateTime)
                {
                    var ts = Reservation.ActualEndDateTime.Value - Reservation.EndDateTime;
                    var minutes = Math.Floor(ts.TotalMinutes);
                    return TimeSpan.FromMinutes(minutes);
                }
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

        public double GetForgivenPercentage()
        {
            return 1 - Reservation.ChargeMultiplier;
        }
    }
}
