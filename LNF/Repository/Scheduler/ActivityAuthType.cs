namespace LNF.Repository.Scheduler
{
    public class ActivityAuthType : IDataItem
    {
        public virtual int ActivityAuthTypeID { get; set; }
        public virtual string AuthTypeName { get; set; }
        public virtual string AuthTypeDescription { get; set; }
    }
}
