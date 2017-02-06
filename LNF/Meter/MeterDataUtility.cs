using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using LNF;
using LNF.Meter;
using LNF.Repository;
using LNF.Repository.Meter;

namespace LNF.Meter
{
    public static class MeterDataUtility
    {
        public static int Import(FileImport import, DataTable dt)
        {
            if (dt == null) return 0;

            List<MeterData> items = new List<MeterData>();

            foreach (DataRow dr in dt.Rows)
            {
                items.Add(new MeterData()
                {
                    FileImport = import,
                    Header = dr["Header"].ToString(),
                    ImportFileName = dr["ImportFileName"].ToString(),
                    LineIndex = Convert.ToInt32(dr["LineIndex"]),
                    TimeStamp = Convert.ToDateTime(dr["TimeStamp"]),
                    Value = Convert.ToDouble(dr["Value"])
                });
            }

            DA.Current.Insert(items);

            return items.Count;
        }
    }
}
