using System.Collections.Generic;

namespace LNF.Scheduler
{
    public interface IKioskRepository
    {
        IEnumerable<IKiosk> GetKiosks();
        bool RefreshCache();
    }
}
