using System;

namespace LNF.Repository.Billing
{
    public class RoomDataImportLog : IDataItem
    {
        public virtual int RoomDataImportLogID { get; set; }
        public virtual DateTime ImportDateTime { get; set; }
        public virtual int RowsImported { get; set; }
        public virtual DateTime PriorMaxEventDate { get; set; }
        public virtual DateTime NewMaxEventDate { get; set; }
    }
}
