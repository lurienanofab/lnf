using LNF.Models.Data;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Data
{
    public class ClientRemoteManager : ApiClient, IClientRemoteManager
    {
        public IClientRemote Create(int clientId, int remoteClientId, int accountId, DateTime period, out bool success)
        {
            throw new NotImplementedException();
        }

        public void Delete(int clientRemoteId, DateTime period)
        {
            throw new NotImplementedException();
        }

        public void Enable(int clientRemoteId, DateTime period)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClientRemote> SelectByPeriod(DateTime period)
        {
            throw new NotImplementedException();
        }
    }
}
