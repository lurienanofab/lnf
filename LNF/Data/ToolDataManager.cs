using LNF.Models.Billing;
using LNF.Repository;
using LNF.Repository.Billing;
using LNF.Repository.Data;
using LNF.Repository.Scheduler;

namespace LNF.Data
{
    public class ToolDataManager : ManagerBase, IToolDataManager
    {
        public ToolDataManager(IProvider provider) : base(provider) { }

        //RoomID might be null
        public Room GetRoom(ToolData item)
        {
            if (item.RoomID.HasValue)
                return Session.Single<Room>(item.RoomID.Value);
            else
                return null;
        }

        //ReservationID might be null
        public Reservation GetReservation(ToolData item)
        {
            if (item.ReservationID.HasValue)
                return Session.Single<Reservation>(item.ReservationID.Value);
            else
                return null;
        }

        public IBillingType GetBillingType(ToolData item)
        {
            return Provider.Billing.BillingType.GetBillingType(item.ClientID, item.AccountID, item.Period);
        }
    }
}
