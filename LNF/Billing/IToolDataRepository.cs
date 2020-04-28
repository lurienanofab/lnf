using LNF.Data;
using LNF.Scheduler;
using System;
using System.Collections.Generic;
using System.Data;

namespace LNF.Billing
{
    public interface IToolDataRepository
    {
        IBillingType GetBillingType(IToolData item);
        IReservation GetReservation(IToolData item);
        IRoom GetRoom(IToolData item);
        DataTable ReadToolDataRaw(DateTime sd, DateTime ed, int clientId = 0);
        DataSet ReadToolDataClean(DateTime sd, DateTime ed, int clientId = 0, int resourceId = 0);
        DataTable ReadToolData(DateTime period, int clientId = 0, int reservationId = 0);
        DataTable ReadToolUtilization(string sumCol, bool includeForgiven, DateTime sd, DateTime ed);
    }
}