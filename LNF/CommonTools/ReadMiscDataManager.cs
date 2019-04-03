using LNF.Repository;
using System;
using System.Data;

namespace LNF.CommonTools
{
    public class ReadMiscDataManager : ManagerBase, IReadMiscDataManager
    {
        public ReadMiscDataManager(IProvider provider) : base(provider) { }

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
