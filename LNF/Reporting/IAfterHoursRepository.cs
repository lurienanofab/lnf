using System.Collections.Generic;

namespace LNF.Reporting
{
    public interface IAfterHoursRepository
    {
        IEnumerable<IAfterHours> GetAfterHours(string name);
    }
}
