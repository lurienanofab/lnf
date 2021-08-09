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
            get
            {
                return Reservation.EndDateTime - Reservation.BeginDateTime;
            }
        }

        /// <summary>
        /// The actual time used.
        /// </summary>
        public TimeSpan ActualDuration
        {
            get
            {
                if (Reservation.ActualEndDateTime.HasValue && Reservation.ActualBeginDateTime.HasValue)
                {
                    return GetTimeSpan(Reservation.ActualBeginDateTime.Value, Reservation.ActualEndDateTime.Value);
                }

                return TimeSpan.Zero;
            }
        }

        /// <summary>
        /// The maximum chargeable time. 
        /// </summary>
        public TimeSpan ChargeDuration
        {
            get
            {
                return GetTimeSpan(Reservation.ChargeBeginDateTime, Reservation.ChargeEndDateTime);
                //var span = Reservation.ChargeEndDateTime - Reservation.ChargeBeginDateTime;
                //var totalSeconds = Convert.ToInt32(span.TotalSeconds);
                //var result = TimeSpan.FromSeconds(totalSeconds);
                //return result;
            }
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
                    return GetTimeSpan(Reservation.EndDateTime, Reservation.ActualEndDateTime.Value);

                    //var sd = DateTime.Parse(Reservation.EndDateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    //var ed = DateTime.Parse(Reservation.ActualEndDateTime.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                    //var result = ed - sd;
                    //return result;

                    //var span = Reservation.ActualEndDateTime.Value - Reservation.EndDateTime;
                    //var minutes = Math.Floor(span.TotalMinutes);
                    //return TimeSpan.FromMinutes(minutes);
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

        private TimeSpan GetTimeSpan(DateTime sd, DateTime ed)
        {
            var start = DateTime.Parse(sd.ToString("yyyy-MM-dd HH:mm:ss"));
            var end = DateTime.Parse(ed.ToString("yyyy-MM-dd HH:mm:ss"));
            var result = end - start;
            return result;

            //var span = ed - sd;
            //var totalSeconds = Convert.ToInt32(span.TotalSeconds);
            //var result = TimeSpan.FromSeconds(totalSeconds);
            //return result;
        }
    }
}
