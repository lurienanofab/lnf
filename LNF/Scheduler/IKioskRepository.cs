using System.Collections.Generic;

namespace LNF.Scheduler
{
    public interface IKioskRepository
    {
        IKioskConfig GetKioskConfig();
        IEnumerable<IKiosk> GetKiosks();
        bool RefreshCache();
    }
}
