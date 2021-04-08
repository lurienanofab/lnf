using LNF.DataAccess;
using LNF.Scheduler;

namespace LNF.Impl.Repository.Scheduler
{
    public class ReservationProcessInfoInfo : IDataItem, IReservationProcessInfo
    {
        public virtual int ReservationProcessInfoID { get; set; }
        public virtual int ReservationID { get; set; }
        public virtual double Value { get; set; }
        public virtual bool Special { get; set; }
        public virtual int RunNumber { get; set; }
        public virtual double ChargeMultiplier { get; set; }
        public virtual bool Active { get; set; }
        public virtual int ProcessInfoLineID { get; set; }
        public virtual string Param { get; set; }
        public virtual int ProcessInfoID { get; set; }
        public virtual string ProcessInfoName { get; set; }

        public override string ToString()
        {
            // ProcessInfo.ProcessInfoName
            // ProcessInfoLine.Param
            // ReservationProcessInfo.ReservationID
            // ReservationProcessInfo.Value
            return $"{ProcessInfoName}/{Param}/{ReservationID}/{Value} [{ReservationProcessInfoID}]";
        }
    }
}
