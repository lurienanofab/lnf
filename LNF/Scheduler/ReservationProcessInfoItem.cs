using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LNF.Repository;
using LNF.Repository.Scheduler;
using LNF.Cache;

namespace LNF.Scheduler
{
    public class ReservationProcessInfoItem
    {
        public int ReservationProcessInfoID { get; set; }
        public int ReservationID { get; set; }
        public int ProcessInfoLineID { get; set; }
        public int ProcessInfoID { get; set; }
        public double Value { get; set; }
        public bool Special { get; set; }
        public int RunNumber { get; set; }
        public double ChargeMultiplier { get; set; }
        public bool Active { get; set; }

        public static ReservationProcessInfoItem Create(ReservationProcessInfo source)
        {
            var pil = CacheManager.Current.ProcessInfoLines(source.ProcessInfoLine.ProcessInfoID).First(x => x.ProcessInfoLineID == source.ProcessInfoLine.ProcessInfoLineID);

            var result = new ReservationProcessInfoItem()
            {
                ReservationProcessInfoID = source.ReservationProcessInfoID,
                ReservationID = source.Reservation.ReservationID,
                ProcessInfoLineID = pil.ProcessInfoLineID,
                ProcessInfoID = pil.ProcessInfoID,
                Value = source.Value,
                Special = source.Special,
                RunNumber = source.RunNumber,
                ChargeMultiplier = source.ChargeMultiplier,
                Active = source.Active
            };

            return result;
        }

        public void Insert()
        {
            // This happens when a new reservation is created.

            if (ProcessInfoLineID > 0)
            {
                var rpi = new ReservationProcessInfo()
                {
                    ProcessInfoLine = DA.Current.Single<ProcessInfoLine>(ProcessInfoLineID),
                    Reservation = DA.Current.Single<Reservation>(ReservationID),
                    Value = Value,
                    Special = Special,
                    RunNumber = RunNumber,
                    ChargeMultiplier = ChargeMultiplier,
                    Active = Active
                };

                DA.Current.Insert(rpi);
                ReservationProcessInfoID = rpi.ReservationProcessInfoID;
            }
        }

        public void Update()
        {
            var rpi = DA.Current.Single<ReservationProcessInfo>(ReservationProcessInfoID);

            if (rpi != null)
            {
                if (ProcessInfoLineID == 0)
                {
                    // Delete the record.
                    // This happens when an existing record is changed to "None" (i.e. removed).
                    DA.Current.Delete(rpi);
                    return;
                }
            }
            else
            {
                // Insert a new record if it doesn't exist.
                // This happens when a reservation is modified and an addtional process info is selected.
                rpi = new ReservationProcessInfo()
                {
                    ProcessInfoLine = DA.Current.Single<ProcessInfoLine>(ProcessInfoLineID),
                    Reservation = DA.Current.Single<Reservation>(ReservationID)
                };

                DA.Current.Insert(rpi);
                ReservationProcessInfoID = rpi.ReservationProcessInfoID;
            }

            // still here?
            
            if (ProcessInfoLineID != rpi.ProcessInfoLine.ProcessInfoLineID)
            { 
                // This happens when the ProcessInfo is changed to different ProcessInfoLine
                var pil = DA.Current.Single<ProcessInfoLine>(ProcessInfoLineID);
                rpi.ProcessInfoLine = pil;
            }

            rpi.Value = Value;
            rpi.Special = Special;
            rpi.RunNumber = RunNumber;
            rpi.ChargeMultiplier = ChargeMultiplier;
            rpi.Active = Active;
        }
    }
}
