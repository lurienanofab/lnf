using LNF.Repository;
using LNF.Repository.Data;
using System;
using System.Data;

namespace LNF.Data
{
    public interface IDryBoxManager : IManager
    {
        DryBoxAssignment[] ActiveAssignments(DateTime startDate, DateTime endDate);
        void Approve(DryBoxAssignment dba, Client modifiedBy);
        DryBoxAssignment CurrentAssignment(DryBox item);
        bool? IsAccountActive(DryBox item);
        void Reject(DryBoxAssignment dba);
        void Remove(DryBoxAssignment dba, Client modifiedBy);
        DryBoxAssignment Request(DryBox db, ClientAccount ca);
        void Update(DryBoxAssignment dba, ClientAccount ca, Client modifiedBy);
        bool HasDryBox(ClientAccount item);
        ClientAccount GetDryBoxClientAccount(ClientOrg item);
        bool HasDryBox(ClientOrg item);
        DataSet ReadDryBoxData(DateTime sd, DateTime ed, int clientId = 0);
    }
}