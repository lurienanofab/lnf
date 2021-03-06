using LNF.DataAccess;
using System;

namespace LNF.Impl.Repository.Billing
{
    public class RoomDataImport : IDataItem
    {
        public virtual int RoomDataImportID { get; set; }
        public virtual byte[] RID { get; set; }
        public virtual int ClientID { get; set; }
        public virtual string RoomName { get; set; }
        public virtual DateTime EventDate { get; set; }
        public virtual string EventDescription { get; set; }
    }
}
