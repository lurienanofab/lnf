using LNF.Scheduler;
using RestSharp;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Scheduler
{
    public class KioskRepository : ApiClient, IKioskRepository
    {
        internal KioskRepository(IRestClient rc) : base(rc) { }

        public IKioskConfig GetKioskConfig()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IKiosk> GetKiosks()
        {
            throw new NotImplementedException();
        }

        public bool RefreshCache()
        {
            throw new NotImplementedException();
        }
    }
}
