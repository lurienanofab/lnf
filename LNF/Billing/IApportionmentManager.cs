using System;
using System.Collections.Generic;
using LNF.Repository;
using LNF.Repository.Billing;
using LNF.Repository.Data;

namespace LNF.Billing
{
    public interface IApportionmentManager : IManager
    {
        void CheckClientIssues();
        int CheckPassbackViolations();
        decimal GetAccountEntries(DateTime period, int clientId, int roomId, int accountId);
        decimal GetApportionment(ClientAccount ca, Room room, DateTime period);
        decimal GetDefaultApportionmentPercentage(int clientId, int roomId, int accountId);
        int GetMinimumDays(DateTime period, int clientId, int roomId, int orgId);
        int GetPhysicalDays(DateTime period, int clientId, int roomId);
        decimal GetTotalEntries(DateTime period, int clientId, int roomId);
        bool IsInOrg(ToolData td, int orgId);
        bool IsInRoom(ToolData td, int roomId);
        int PopulateRoomApportionData(DateTime period);
        IList<ApportionmentClient> SelectApportionmentClients(DateTime startDate, DateTime endDate);
        int SendMonthlyApportionmentEmails(DateTime period, string message = null, string[] recipients = null, bool noEmail = false);
        int UpdateChildRoomEntryApportionment(DateTime period, int clientId, int parentRoomId);
        void UpdateRoomBillingEntries(DateTime period, int clientId, int roomId, int accountId, decimal entries);
    }
}