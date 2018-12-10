using System;
using System.Linq;

namespace LNF.Repository.Meter
{
    public class MeterData : IDataItem
    {
        public virtual int MeterDataID { get; set; }
        public virtual int FileIndex { get; set; }
        public virtual int LineIndex { get; set; }
        public virtual string Header { get; set; }
        public virtual DateTime TimeStamp { get; set; }
        public virtual double Value { get; set; }

        public static int DeleteByFileImport(int fileIndex)
        {
            var query = DA.Current.Query<MeterData>().Where(x => x.FileIndex == fileIndex);
            var result = query.Count();
            DA.Current.Delete(query);
            return result;
        }
    }
}
