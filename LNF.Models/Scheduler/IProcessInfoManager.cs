using System.Collections.Generic;

namespace LNF.Models.Scheduler
{
    public interface IProcessInfoManager
    {
        IEnumerable<IProcessInfo> GetProcessInfos(int resourceId);
        IEnumerable<IProcessInfoLine> GetProcessInfoLines(int resourceId);
        IEnumerable<IReservationProcessInfo> GetReservationProcessInfos(int reservationId);
        void InsertReservationProcessInfo(IReservationProcessInfo item);
        void UpdateReservationProcessInfo(IReservationProcessInfo item);
        IProcessInfoLine GetProcessInfoLine(int processInfoLineId);
    }
}
