using LNF.Repository;
using System;
using System.Data;

namespace LNF.CommonTools
{
    public class ReadMiscDataManager
    {
        internal ReadMiscDataManager() { }

        public DataTable ReadMiscData(DateTime period)
        {
            using (var dba = DA.Current.GetAdapter())
            {
                var dt = dba
                    .ApplyParameters(new { Action = "GetAllByPeriod", Period = period })
                    .FillDataTable("MiscBillingCharge_Select");

                dt.TableName = "MiscExp";

                return dt;
            }
        }
    }
}
