using System;
using System.Collections.Generic;

namespace LNF.Models.Data
{
    public interface IClientRemoteManager
    {
        void Enable(int clientRemoteId, DateTime period);
        IClientRemote Create(int clientId, int remoteClientId, int accountId, DateTime period, out bool success);
        void Delete(int clientRemoteId, DateTime period);
        IEnumerable<IClientRemote> SelectByPeriod(DateTime period);
    }
}
