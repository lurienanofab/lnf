using LNF.Models.Billing;
using LNF.Models.Billing.Reports;
using LNF.Repository;
using LNF.Repository.Billing;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;

namespace LNF.Billing
{
    public interface IApportionmentManager : IManager
    {
        void CheckClientIssues();
        CheckPassbackViolationsProcessResult CheckPassbackViolations(DateTime sd, DateTime ed);
        decimal GetAccountEntries(DateTime period, int clientId, int roomId, int accountId);
        decimal GetApportionment(ClientAccount ca, Room room, DateTime period);
        decimal GetDefaultApportionmentPercentage(int clientId, int roomId, int accountId);
        int GetMinimumDays(DateTime period, int clientId, int roomId, int orgId);
        int GetPhysicalDays(DateTime period, int clientId, int roomId);
        decimal GetTotalEntries(DateTime period, int clientId, int roomId);
        bool IsInOrg(ToolData td, int orgId);
        bool IsInRoom(ToolData td, int roomId);
        int PopulateRoomApportionData(DateTime period);
        IEnumerable<ApportionmentClient> SelectApportionmentClients(DateTime sd, DateTime ed);
        IEnumerable<UserApportionmentReportEmail> GetMonthlyApportionmentEmails(DateTime period, string message = null);
        SendMonthlyApportionmentEmailsProcessResult SendMonthlyApportionmentEmails(DateTime period, string message = null, bool noEmail = false);
        int UpdateChildRoomEntryApportionment(DateTime period, int clientId, int parentRoomId);
        void UpdateRoomBillingEntries(DateTime period, int clientId, int roomId, int accountId, decimal entries);
    }
}