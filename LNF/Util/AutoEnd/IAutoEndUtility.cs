using System;
using System.Collections.Generic;

namespace LNF.Util.AutoEnd
{
    public interface IAutoEndUtility
    {
        IEnumerable<AutoEndProblem> GetAutoEndProblems(DateTime period);
        int FixAllAutoEndProblems(DateTime period);
        int FixAutoEndProblem(DateTime period, int reservationId);
    }
}
