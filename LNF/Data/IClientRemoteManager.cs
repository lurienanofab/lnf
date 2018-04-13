using System;
using System.Collections.Generic;
using LNF.Repository;
using LNF.Repository.Data;

namespace LNF.Data
{
    public interface IClientRemoteManager : IManager
    {
        ClientRemote Create(int clientId, int remoteClientId, int accountId, DateTime period, out bool success);
        void Delete(int clientRemoteId, DateTime period);
        void Enable(ClientRemote item, DateTime period);
        IEnumerable<ClientRemote> SelectByPeriod(DateTime period);
    }
}