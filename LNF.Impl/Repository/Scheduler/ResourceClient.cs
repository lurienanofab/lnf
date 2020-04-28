using LNF.DataAccess;
using LNF.Scheduler;
using System;

namespace LNF.Impl.Repository.Scheduler
{
    public class ResourceClient : IDataItem
    {
        public virtual int ResourceClientID { get; set; }
        public virtual int ResourceID { get; set; }
        public virtual int ClientID { get; set; }
        public virtual ClientAuthLevel AuthLevel { get; set; }
        public virtual DateTime? Expiration { get; set; }
        public virtual int? EmailNotify { get; set; }
        public virtual int? PracticeResEmailNotify { get; set; }
    }
}
