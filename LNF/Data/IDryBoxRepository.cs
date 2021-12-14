using System;
using System.Collections.Generic;
using System.Data;

namespace LNF.Data
{
    public interface IDryBoxRepository
    {
        IEnumerable<DryBox> GetDryBoxes();
        bool UpdateDryBox(DryBox model);
        DryBoxAssignmentInfo GetCurrentDryBoxAssignment(int dryBoxId);
        DryBoxAssignmentInfo Request(DryBoxRequest request);
        DryBoxAssignmentInfo CancelRequest(int dryBoxAssignmentId);
        DryBoxAssignmentInfo RequestRemove(int dryBoxAssignmentId);
        DryBoxAssignmentInfo Reject(int dryBoxAssignmentId);
        DryBoxAssignmentInfo Approve(int dryBoxAssignmentId, int modifiedByClientId);
        DryBoxAssignmentInfo UpdateDryBoxAssignment(int dryBoxAssignmentId, DryBoxAssignmentUpdate update);
        bool? IsAccountActive(int dryBoxId);
        bool Remove(int dryBoxAssignmentId, int modifiedByClientId);
        DryBoxAssignment GetDryBoxAssignment(int dryBoxAssignmentId);
        IEnumerable<DryBoxAssignment> GetActiveDryBoxAssignments(DateTime sd, DateTime ed);
        IEnumerable<DryBoxAssignmentInfo> GetCurrentDryBoxAssignments();
        bool ClientAccountHasDryBox(int clientAccountId);
        bool ClientOrgHasDryBox(int clientOrgId);
        IClientAccount GetDryBoxClientAccount(int clientOrgId);
        IEnumerable<DryBoxHistory> GetDryBoxHistory(string dryBoxName);
        IEnumerable<DryBoxHistory> GetDryBoxHistory(int clientId);
        DataSet ReadDryBoxData(DateTime sd, DateTime ed, int clientId = 0);
    }
}