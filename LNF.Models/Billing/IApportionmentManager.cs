using LNF.Models.Data;
using System;
using System.Collections.Generic;

namespace LNF.Models.Billing
{
    public interface IApportionmentManager
    {
        void CheckClientIssues();
        CheckPassbackViolationsProcessResult CheckPassbackViolations(DateTime sd, DateTime ed);
        decimal GetAccountEntries(DateTime period, int clientId, int roomId, int accountId);
        decimal GetApportionment(IClientAccount ca, IRoom room, DateTime period);
        decimal GetDefaultApportionmentPercentage(int clientId, int roomId, int accountId);
        int GetMinimumDays(DateTime period, int clientId, int roomId, int orgId);
        int GetPhysicalDays(DateTime period, int clientId, int roomId);
        decimal GetTotalEntries(DateTime period, int clientId, int roomId);
        bool IsInOrg(IToolData td, int orgId);
        bool IsInRoom(IToolData td, int roomId);
        int PopulateRoomApportionData(DateTime period);
        IEnumerable<IApportionmentClient> SelectApportionmentClients(DateTime sd, DateTime ed);
        int UpdateChildRoomEntryApportionment(DateTime period, int clientId, int parentRoomId);
        void UpdateRoomBillingEntries(DateTime period, int clientId, int roomId, int accountId, decimal entries);
    }
}