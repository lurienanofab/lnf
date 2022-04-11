using LNF.Data;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;

namespace OnlineServices.Api.Data
{
    public class DryBoxRepository : ApiClient, IDryBoxRepository
    {
        internal DryBoxRepository(IRestClient rc) : base(rc) { }

        public IEnumerable<DryBox> GetDryBoxes()
        {
            return Get<List<DryBox>>("webapi/data/drybox");
        }

        public bool UpdateDryBox(DryBox model)
        {
            return Put("webapi/data/drybox", model);
        }

        public DryBoxAssignmentInfo GetCurrentDryBoxAssignment(int dryBoxId)
        {
            return Get<DryBoxAssignmentInfo>("webapi/data/drybox/{dryBoxId}/assignment", UrlSegments(new { dryBoxId }));
        }

        public DryBoxAssignmentInfo Request(DryBoxRequest request)
        {
            return Post<DryBoxAssignmentInfo>("webapi/data/drybox/{dryBoxId}/request", request);
        }

        public DryBoxAssignmentInfo Reject(int dryBoxAssignmentId)
        {
            return Get<DryBoxAssignmentInfo>("webapi/data/drybox/assignment/{dryBoxAssignmentId}/reject", UrlSegments(new { dryBoxAssignmentId }));
        }

        public DryBoxAssignmentInfo Approve(int dryBoxAssignmentId, DryBoxAssignmentUpdate update)
        {
            return Put<DryBoxAssignmentInfo>("webapi/data/drybox/assignment/{dryBoxAssignmentId}/approve", update, UrlSegments(new { dryBoxAssignmentId }));
        }

        public DryBoxAssignmentInfo UpdateDryBoxAssignment(int dryBoxAssignmentId, DryBoxAssignmentUpdate update)
        {
            return Put<DryBoxAssignmentInfo>("webapi/data/drybox/assignment/{dryBoxAssignmentId}", update, UrlSegments(new { dryBoxAssignmentId }));
        }

        public bool? IsAccountActive(int dryBoxId)
        {
            return Get<bool?>("webapi/data/drybox/{dryBoxId}/account/active", UrlSegments(new { dryBoxId }));
        }

        public bool Remove(int dryBoxAssignmentId, int modifiedByClientId)
        {
            return Get<bool>("webapi/data/drybox/assignment/{dryBoxAssignmentId}/remove",
                UrlSegments(new { dryBoxAssignmentId }) & QueryStrings(new { modifiedByClientId }));
        }

        public DryBoxAssignment GetDryBoxAssignment(int dryBoxAssignmentId)
        {
            return Get<DryBoxAssignment>("webapi/data/drybox/assignment/{dryBoxAssignmentId}", UrlSegments(new { dryBoxAssignmentId }));
        }

        public IEnumerable<DryBoxAssignment> GetActiveDryBoxAssignments(DateTime sd, DateTime ed)
        {
            return Get<List<DryBoxAssignment>>("webapi/data/drybox/assignment/active");
        }

        public bool ClientAccountHasDryBox(int clientAccountId)
        {
            return Get<bool>("webapi/data/drybox/clientaccount/{clientAccountId}/exists", UrlSegments(new { clientAccountId }));
        }

        public bool ClientOrgHasDryBox(int clientOrgId)
        {
            return Get<bool>("webapi/data/drybox/clientorg/{clientOrgId}/exists", UrlSegments(new { clientOrgId }));
        }

        public IClientAccount GetDryBoxClientAccount(int clientOrgId)
        {
            return Get<ClientAccountItem>("webapi/data/drybox/clientorg/{clientOrgId}/clientaccount", UrlSegments(new { clientOrgId }));
        }

        public IEnumerable<DryBoxAssignmentInfo> GetCurrentDryBoxAssignments()
        {
            return Get<List<DryBoxAssignmentInfo>>("webapi/data/drybox/assignment/current");
        }

        public DryBoxAssignmentInfo CancelRequest(int dryBoxAssignmentId)
        {
            return Get<DryBoxAssignmentInfo>("webapi/data/drybox/assignment/{dryBoxAssignmentId}/cancel-request", UrlSegments(new { dryBoxAssignmentId }));
        }

        public DryBoxAssignmentInfo RequestRemove(int dryBoxAssignmentId)
        {
            return Get<DryBoxAssignmentInfo>("webapi/data/drybox/assignment/{dryBoxAssignmentId}/request-remove", UrlSegments(new { dryBoxAssignmentId }));
        }

        public IEnumerable<DryBoxHistory> GetDryBoxHistory(string dryBoxName)
        {
            return Get<List<DryBoxHistory>>("webapi/data/drybox/history/{dryBoxName}", UrlSegments(new { dryBoxName }));
        }

        public IEnumerable<DryBoxHistory> GetDryBoxHistory(int clientId)
        {
            return Get<List<DryBoxHistory>>("webapi/data/drybox/history/client/{clientId}", UrlSegments(new { clientId }));
        }

        public DataSet ReadDryBoxData(DateTime sd, DateTime ed, int clientId = 0)
        {
            throw new NotImplementedException();
        }
    }
}
