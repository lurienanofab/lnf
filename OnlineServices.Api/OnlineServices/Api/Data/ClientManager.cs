using LNF.Models.Data;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Data
{
    public class ClientManager : IClientManager
    {
        public IEnumerable<IAccount> ActiveAccounts(int clientId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IAccount> ActiveAccounts(int clientId, DateTime sd, DateTime ed)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClientAccount> ActiveClientAccounts(int clientId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClientAccount> ActiveClientAccounts(int clientId, DateTime sd, DateTime ed)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClient> ActiveClientOrgs(int clientId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClient> ActiveClientOrgs(int clientId, DateTime sd, DateTime ed)
        {
            throw new NotImplementedException();
        }

        public string[] ActiveEmails(int clientId)
        {
            throw new NotImplementedException();
        }

        public bool CheckPassword(int clientId, string password)
        {
            throw new NotImplementedException();
        }

        public string CleanMiddleName(string raw)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClientAccount> ClientAccounts(int clientId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClient> ClientOrgs(int clientId)
        {
            throw new NotImplementedException();
        }

        public IClient Find(string username)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClient> FindByCommunity(int flag, bool? active = true)
        {
            throw new NotImplementedException();
        }

        public IClient FindByDisplayName(string displayName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClient> FindByManager(int managerClientId, bool? active = true)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClient> FindByPeriod(IClient client, DateTime period, bool displayAllUsersToStaff = false)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClient> FindByPrivilege(ClientPrivilege priv, bool? active = true)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClient> FindByTools(int[] resourceIds, bool? active = true)
        {
            throw new NotImplementedException();
        }

        public int GetActiveAccountCount(int clientId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClient> GetActiveClients()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClient> GetActiveClients(ClientPrivilege priv)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClient> GetActiveClients(DateTime sd, DateTime ed)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClient> GetActiveClients(DateTime sd, DateTime ed, ClientPrivilege priv)
        {
            throw new NotImplementedException();
        }

        public IClient GetClient(int clientId)
        {
            throw new NotImplementedException();
        }

        public IClient GetClient(int clientId, int rank)
        {
            throw new NotImplementedException();
        }

        public DateTime? LastReservation(int clientId)
        {
            throw new NotImplementedException();
        }

        public DateTime? LastRoomEntry(int clientId)
        {
            throw new NotImplementedException();
        }

        public IClient Login(string username, string password)
        {
            throw new NotImplementedException();
        }

        public IChargeType MaxChargeType(int clientId)
        {
            throw new NotImplementedException();
        }

        public int SetPassword(int clientId, string password)
        {
            throw new NotImplementedException();
        }

        public IClient StoreClientInfo(ref int clientId, string lname, string fname, string mname, string username, IClientDemographics demographics, IEnumerable<IPriv> privs, IEnumerable<ICommunity> communities, int technicalFieldId, int orgId, int roleId, int deptId, string email, string phone, bool isManager, bool isFinManager, DateTime? subsidyStart, DateTime? newFacultyStart, int[] addedAddressIds, int[] deletedAddressIds, int[] clientManagerIds, int[] clientAccountIds, out string alert)
        {
            throw new NotImplementedException();
        }

        public ITechnicalField TechnicalField(int clientId)
        {
            throw new NotImplementedException();
        }

        public string TechnicalFieldName(int clientId)
        {
            throw new NotImplementedException();
        }

        public int TotalDaysInLab(int clientId, int roomId, DateTime period)
        {
            throw new NotImplementedException();
        }

        public void Update(IClient client)
        {
            throw new NotImplementedException();
        }

        public bool UpdatePhysicalAccess(IClient client, out string alert)
        {
            throw new NotImplementedException();
        }
    }
}