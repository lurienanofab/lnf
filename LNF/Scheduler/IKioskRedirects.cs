using System.Collections.Generic;

namespace LNF.Scheduler
{
    public interface IKioskRedirects
    {
        string Default { get; set; }

        IEnumerable<IKioskRedirectItem> Items { get; set; }
    }
}
