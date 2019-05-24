using LNF.Models.Billing;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Repository.Scheduler;

namespace LNF.Data
{
    public interface IToolDataManager : IManager
    {
        IBillingType GetBillingType(ToolData item);
        Reservation GetReservation(ToolData item);
        Room GetRoom(ToolData item);
    }
}