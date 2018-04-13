using LNF.Models.Ordering;

namespace LNF.Repository.Ordering
{


    public class Status : IDataItem
    {
        public virtual int StatusID { get; set; }
        public virtual string StatusName { get; set; }

        public static Status Draft
        {
            get { return DA.Current.Single<Status>((int)OrderStatus.Draft); }
        }

        public static Status AwaitingApproval
        {
            get { return DA.Current.Single<Status>((int)OrderStatus.AwaitingApproval); }
        }

        public static Status Approved
        {
            get { return DA.Current.Single<Status>((int)OrderStatus.Approved); }
        }

        public static Status Ordered
        {
            get { return DA.Current.Single<Status>((int)OrderStatus.Ordered); }
        }

        public static Status Completed
        {
            get { return DA.Current.Single<Status>((int)OrderStatus.Completed); }
        }

        public static Status Cancelled
        {
            get { return DA.Current.Single<Status>((int)OrderStatus.Cancelled); }
        }

        public static Status ProcessedManually
        {
            get { return DA.Current.Single<Status>((int)OrderStatus.ProcessedManually); }
        }
    }
}
