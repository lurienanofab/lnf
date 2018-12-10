using LNF.Models.Scheduler;
using LNF.Repository;
using System.Collections.Generic;

namespace LNF.Scheduler
{
    public interface IProcessInfoManager : IManager
    {
        IList<Models.Scheduler.ProcessInfoItem> GetProcessInfos(int resourceId);
        IList<Models.Scheduler.ProcessInfoLineItem> GetProcessInfoLines(int resourceId);
        IList<Models.Scheduler.ReservationProcessInfoItem> GetReservationProcessInfos(int reservationId);
        void InsertReservationProcessInfo(Models.Scheduler.ReservationProcessInfoItem item);
        void UpdateReservationProcessInfo(Models.Scheduler.ReservationProcessInfoItem item);
    }
}
