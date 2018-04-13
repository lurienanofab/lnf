using LNF.Billing;
using LNF.Repository;
using LNF.Repository.Billing;
using LNF.Repository.Data;
using LNF.Repository.Scheduler;

namespace LNF.Data
{
    public class ToolDataManager : ManagerBase
    {
        public ToolDataManager(ISession session) : base(session) { }

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

        public BillingType GetBillingType(ToolData item)
        {
            var client = Session.Single<Client>(item.ClientID);
            var account = Session.Single<Account>(item.AccountID);
            return Session.BillingTypeManager().GetBillingType(client, account, item.Period);
        }
    }
}
