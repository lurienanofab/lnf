using LNF.Models.Scheduler;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Scheduler
{
    public class ProcessInfoManager : ApiClient, IProcessInfoManager
    {
        public IProcessInfoLine GetProcessInfoLine(int processInfoLineId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IProcessInfoLine> GetProcessInfoLines(int resourceId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IProcessInfo> GetProcessInfos(int resourceId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IReservationProcessInfo> GetReservationProcessInfos(int reservationId)
        {
            throw new NotImplementedException();
        }

        public void InsertReservationProcessInfo(IReservationProcessInfo item)
        {
            throw new NotImplementedException();
        }

        public void UpdateReservationProcessInfo(IReservationProcessInfo item)
        {
            throw new NotImplementedException();
        }
    }
}
