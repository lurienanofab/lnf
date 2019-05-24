using System;
using System.Collections.Generic;
using System.Data;

namespace LNF.Models.Data
{
    public interface IDryBoxManager
    {
        IEnumerable<IDryBox> GetDryBoxes();
        bool UpdateDryBox(IDryBox model);
        IDryBoxAssignment GetCurrentAssignment(int dryBoxId);
        bool? IsAccountActive(int dryBoxId);
        IDryBoxAssignment Request(int dryBoxId, int clientAccountId);
        void Reject(int dryBoxAssignmentId);
        void Approve(int dryBoxAssignmentId, int modifiedByClientId);
        void UpdateDryBoxAssignment(IDryBoxAssignment dba, int clientAccountId, int modifiedByClientId);
        void Remove(int dryBoxAssignmentId, int modifiedByClientId);
        IEnumerable<IDryBoxAssignment> GetActiveAssignments(DateTime sd, DateTime ed);
        bool ClientAccountHasDryBox(int clientAccountId);
        bool ClientOrgHasDryBox(int clientOrgId);
        IClientAccount GetDryBoxClientAccount(int clientOrgId);
        DataSet ReadDryBoxData(DateTime sd, DateTime ed, int clientId = 0);
    }
}