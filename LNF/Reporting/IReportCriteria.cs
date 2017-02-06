using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using LNF.Repository.Data;

namespace LNF.Reporting
{
    public interface IReportCriteria
    {
        T GetValue<T>(string key, T defval);
        CriteriaWriter CreateWriter(StringBuilder sb);
    }
}
