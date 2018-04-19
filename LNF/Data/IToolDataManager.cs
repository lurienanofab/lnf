using LNF.Repository;
using LNF.Repository.Billing;
using LNF.Repository.Data;
using LNF.Repository.Scheduler;

namespace LNF.Data
{
    public interface IToolDataManager : IManager
    {
        BillingType GetBillingType(ToolData item);
        Reservation GetReservation(ToolData item);
        Room GetRoom(ToolData item);
    }
}