using LNF.DataAccess;
using System;
using System.Linq;

namespace LNF.Impl.Repository.Meter
{
    public class MeterData : IDataItem
    {
        public virtual int MeterDataID { get; set; }
        public virtual int FileIndex { get; set; }
        public virtual int LineIndex { get; set; }
        public virtual string Header { get; set; }
        public virtual DateTime TimeStamp { get; set; }
        public virtual double Value { get; set; }

        public static int DeleteByFileImport(NHibernate.ISession session, int fileIndex)
        {
            var query = session.Query<MeterData>().Where(x => x.FileIndex == fileIndex);
            var result = query.Count();
            session.DeleteMany(query);
            return result;
        }
    }
}
