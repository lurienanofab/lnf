using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Reporting
{
    public enum ResultType
    {
        Unspecified = 0,
        Ajax = 1,
        DataTables = 2,
        HTML = 3
    }

    public interface IReport<T> : IReport where T : IReportCriteria
    {
        T Criteria { get; }
        IReport<T> Configure(T criteria);
    }

    public interface IReport
    {
        string Key { get; }
        string Title { get; }
        string CategoryName { get; }
        void WriteCriteria(StringBuilder sb);
        GenericResult Execute();
        GenericResult Execute(ResultType requestType);
    }
}
