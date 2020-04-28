using LNF.Data;
using System;
using System.Collections.Generic;
using System.Data;

namespace OnlineServices.Api.Data
{
    public class DryBoxRepository : ApiClient, IDryBoxRepository
    {
        public IEnumerable<IDryBox> GetDryBoxes()
        {
            return Get<List<DryBoxItem>>("webapi/data/drybox");
        }

        public bool UpdateDryBox(IDryBox model)
        {
            return Put("webapi/data/drybox", model);
        }

        public IEnumerable<IDryBoxAssignment> GetActiveAssignments(DateTime sd, DateTime ed)
        {
            return Get<List<DryBoxAssignmentItem>>("webapi/data/drybox/assignment/active");
        }

        public void Approve(int dryBoxAssignmentId, int modifiedByClientId)
        {
            Get("webapi/data/drybox/approve", QueryStrings(new { dryBoxAssignmentId, modifiedByClientId }));
        }

        public bool ClientAccountHasDryBox(int clientAccountId)
        {
            return Get<bool>("webapi/data/drybox/clientaccount/{clientAccountId}/exists", UrlSegments(new { clientAccountId }));
        }

        public bool ClientOrgHasDryBox(int clientOrgId)
        {
            return Get<bool>("webapi/data/drybox/clientorg/{clientOrgId}/exists", UrlSegments(new { clientOrgId }));
        }

        public IDryBoxAssignment GetCurrentAssignment(int dryBoxId)
        {
            return Get<DryBoxAssignmentItem>("webapi/data/drybox/{dryBoxId}/assignment", UrlSegments(new { dryBoxId }));
        }

        public IClientAccount GetDryBoxClientAccount(int clientOrgId)
        {
            return Get<ClientAccountItem>("webapi/data/drybox/clientorg/{clientOrgId}/clientaccount", UrlSegments(new { clientOrgId }));
        }

        public bool? IsAccountActive(int dryBoxId)
        {
            return Get<bool?>("webapi/data/drybox/{dryBoxId}/account/active", UrlSegments(new { dryBoxId }));
        }

        public DataSet ReadDryBoxData(DateTime sd, DateTime ed, int clientId = 0)
        {
            throw new NotImplementedException();
        }

        public void Reject(int dryBoxAssignmentId)
        {
            throw new NotImplementedException();
        }

        public void Remove(int dryBoxAssignmentId, int modifiedByClientId)
        {
            throw new NotImplementedException();
        }

        public IDryBoxAssignment Request(int dryBoxId, int clientAccountId)
        {
            throw new NotImplementedException();
        }

        public void UpdateDryBoxAssignment(IDryBoxAssignment dba, int clientAccountId, int modifiedByClientId)
        {
            throw new NotImplementedException();
        }
    }
}
