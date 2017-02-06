using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Repository.Meter
{
    public class MeterData : IDataItem
    {
        public virtual int MeterDataID { get; set; }
        public virtual FileImport FileImport { get; set; }
        public virtual string ImportFileName { get; set; }
        public virtual int LineIndex { get; set; }
        public virtual DateTime TimeStamp { get; set; }
        public virtual string Header { get; set; }
        public virtual double Value { get; set; }

        public static int DeleteByFileImport(FileImport fileImport)
        {
            IList<MeterData> query = DA.Current.Query<MeterData>().Where(x => x.FileImport == fileImport).ToList();
            DA.Current.Delete(query);
            return query.Count;
        }
    }
}
