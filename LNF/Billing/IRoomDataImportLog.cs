using System;

namespace LNF.Billing
{
    public interface IRoomDataImportLog
    {
        int RoomDataImportLogID { get; set; }
        DateTime ImportDateTime { get; set; }
        int RowsImported { get; set; }
        DateTime PriorMaxEventDate { get; set; }
        DateTime NewMaxEventDate { get; set; }
    }
}