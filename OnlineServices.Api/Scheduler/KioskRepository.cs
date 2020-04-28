using LNF.Scheduler;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Scheduler
{
    public class KioskRepository : ApiClient, IKioskRepository
    {
        public IEnumerable<IKiosk> GetKiosks()
        {
            throw new NotImplementedException();
        }
    }
}
