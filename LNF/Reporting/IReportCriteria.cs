using System.Text;

namespace LNF.Reporting
{
    public interface IReportCriteria
    {
        T GetValue<T>(string key, T defval);
        CriteriaWriter CreateWriter(StringBuilder sb);
    }
}
