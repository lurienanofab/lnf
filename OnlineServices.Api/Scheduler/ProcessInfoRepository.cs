using LNF.Scheduler;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Scheduler
{
    public class ProcessInfoRepository : ApiClient, IProcessInfoRepository
    {
        public IReservationProcessInfo AddReservationProcessInfo(int reservationId, int processInfoLineId, double value, bool special, int runNumber, double chargeMultiplier, bool active)
        {
            throw new NotImplementedException();
        }

        public IProcessInfo GetProcessInfo(int processInfoId)
        {
            throw new NotImplementedException();
        }

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

        public void Update(IEnumerable<IProcessInfo> insert, IEnumerable<IProcessInfo> update, IEnumerable<IProcessInfo> delete)
        {
            throw new NotImplementedException();
        }

        public void Update(IEnumerable<IReservationProcessInfo> insert, IEnumerable<IReservationProcessInfo> update, IEnumerable<IReservationProcessInfo> delete)
        {
            throw new NotImplementedException();
        }

        public void Update(IEnumerable<IProcessInfoLine> insert, IEnumerable<IProcessInfoLine> update, IEnumerable<IProcessInfoLine> delete)
        {
            throw new NotImplementedException();
        }

        public void UpdateReservationProcessInfo(IReservationProcessInfo item)
        {
            throw new NotImplementedException();
        }
    }
}
