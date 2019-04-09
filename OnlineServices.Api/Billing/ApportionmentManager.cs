using LNF.Models.Billing;
using LNF.Models.Billing.Reports;
using LNF.Models.Data;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Billing
{
    public class ApportionmentManager : ApiClient, IApportionmentManager
    {
        public void CheckClientIssues()
        {
            throw new NotImplementedException();
        }

        public CheckPassbackViolationsProcessResult CheckPassbackViolations(DateTime sd, DateTime ed)
        {
            throw new NotImplementedException();
        }

        public decimal GetAccountEntries(DateTime period, int clientId, int roomId, int accountId)
        {
            throw new NotImplementedException();
        }

        public decimal GetApportionment(IClientAccount ca, IRoom room, DateTime period)
        {
            throw new NotImplementedException();
        }

        public decimal GetDefaultApportionmentPercentage(int clientId, int roomId, int accountId)
        {
            throw new NotImplementedException();
        }

        public int GetMinimumDays(DateTime period, int clientId, int roomId, int orgId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<UserApportionmentReportEmail> GetMonthlyApportionmentEmails(UserApportionmentReportOptions options)
        {
            throw new NotImplementedException();
        }

        public int GetPhysicalDays(DateTime period, int clientId, int roomId)
        {
            throw new NotImplementedException();
        }

        public decimal GetTotalEntries(DateTime period, int clientId, int roomId)
        {
            throw new NotImplementedException();
        }

        public bool IsInOrg(IToolData td, int orgId)
        {
            throw new NotImplementedException();
        }

        public bool IsInRoom(IToolData td, int roomId)
        {
            throw new NotImplementedException();
        }

        public int PopulateRoomApportionData(DateTime period)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IApportionmentClient> SelectApportionmentClients(DateTime sd, DateTime ed)
        {
            throw new NotImplementedException();
        }

        public SendMonthlyApportionmentEmailsProcessResult SendMonthlyApportionmentEmails(UserApportionmentReportOptions options)
        {
            throw new NotImplementedException();
        }

        public int UpdateChildRoomEntryApportionment(DateTime period, int clientId, int parentRoomId)
        {
            throw new NotImplementedException();
        }

        public void UpdateRoomBillingEntries(DateTime period, int clientId, int roomId, int accountId, decimal entries)
        {
            throw new NotImplementedException();
        }
    }
}
