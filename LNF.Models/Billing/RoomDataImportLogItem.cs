using System;

namespace LNF.Models.Billing
{
    public class RoomDataImportLogItem : IRoomDataImportLog
    {
        public int RoomDataImportLogID { get; set; }
        public DateTime ImportDateTime { get; set; }
        public int RowsImported { get; set; }
        public DateTime PriorMaxEventDate { get; set; }
        public DateTime NewMaxEventDate { get; set; }
    }
}
