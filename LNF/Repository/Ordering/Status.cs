namespace LNF.Repository.Ordering
{
    public class Status : IDataItem
    {
        public virtual int StatusID { get; set; }
        public virtual string StatusName { get; set; }

        public static Status Draft
        {
            get { return DA.Current.Single<Status>(1); }
        }

        public static Status AwaitingApproval
        {
            get { return DA.Current.Single<Status>(2); }
        }

        public static Status Approved
        {
            get { return DA.Current.Single<Status>(3); }
        }

        public static Status Ordered
        {
            get { return DA.Current.Single<Status>(4); }
        }

        public static Status Completed
        {
            get { return DA.Current.Single<Status>(5); }
        }

        public static Status Cancelled
        {
            get { return DA.Current.Single<Status>(6); }
        }

        public static Status ProcessedManually
        {
            get { return DA.Current.Single<Status>(7); }
        }
    }
}
