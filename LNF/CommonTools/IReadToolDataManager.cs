using LNF.Repository;
using System;
using System.Data;

namespace LNF.CommonTools
{
    public interface IReadToolDataManager : IManager
    {
        DataTable ReadToolDataRaw(DateTime sd, DateTime ed, int clientId = 0);
        DataSet ReadToolDataClean(DateTime sd, DateTime ed, int clientId = 0, int resourceId = 0);
        DataTable ReadToolData(DateTime period, int clientId = 0, int resourceId = 0);
        DataTable ReadToolUtilization(string sumCol, bool includeForgiven, DateTime sd, DateTime ed);
    }
}