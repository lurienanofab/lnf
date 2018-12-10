using LNF.Repository;
using System;
using System.Data;

namespace LNF.CommonTools
{
    public class ReadMiscDataManager : ManagerBase, IReadMiscDataManager
    {
        public ReadMiscDataManager(ISession session) : base(session) { }

        public DataTable ReadMiscData(DateTime period)
        {
            var dt = new DataTable("MiscExp");

            Command()
                .Param(new { Action = "GetAllByPeriod", Period = period })
                .FillDataTable(dt, "dbo.MiscBillingCharge_Select");

            return dt;
        }
    }
}
