using System.Text;

namespace LNF.Reporting
{
    public abstract class DefaultReport<T> : IReport<T> where T : IReportCriteria
    {
        protected T _Criteria;

        public virtual IReport<T> Configure(T criteria)
        {
            _Criteria = criteria;
            return this;
        }

        public T Criteria
        {
            get { return _Criteria; }
        }

        public abstract void WriteCriteria(StringBuilder sb);

        public abstract string Key { get; }

        public abstract string Title { get; }

        public abstract string CategoryName { get; }

        public virtual GenericResult Execute()
        {
            return Execute(ResultType.Unspecified);
        }

        public abstract GenericResult Execute(ResultType resultType);
    }
}
