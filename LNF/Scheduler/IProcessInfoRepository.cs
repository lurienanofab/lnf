﻿using System.Collections.Generic;
using System.Data;

namespace LNF.Scheduler
{
    public interface IProcessInfoRepository
    {
        IEnumerable<IProcessInfo> GetProcessInfos(int resourceId);
        IEnumerable<IProcessInfoLine> GetProcessInfoLines(int resourceId);
        IEnumerable<IReservationProcessInfo> GetReservationProcessInfos(int reservationId);
        IEnumerable<IReservationProcessInfo> GetReservationProcessInfos(int[] reservations);
        void InsertReservationProcessInfo(IReservationProcessInfo item);
        void UpdateReservationProcessInfo(IReservationProcessInfo item);
        IProcessInfoLine GetProcessInfoLine(int processInfoLineId);
        IReservationProcessInfo AddReservationProcessInfo(int reservationId, int processInfoLineId, double value, bool special, int runNumber, double chargeMultiplier, bool active);
        void Update(IEnumerable<IProcessInfo> insert, IEnumerable<IProcessInfo> update, IEnumerable<IProcessInfo> delete);
        void Update(IEnumerable<IProcessInfoLine> insert, IEnumerable<IProcessInfoLine> update, IEnumerable<IProcessInfoLine> delete);
        void Update(IEnumerable<IReservationProcessInfo> insert, IEnumerable<IReservationProcessInfo> update, IEnumerable<IReservationProcessInfo> delete);
        IProcessInfo GetProcessInfo(int processInfoId);
        IProcessInfo Create(DataRow dr);
    }
}
