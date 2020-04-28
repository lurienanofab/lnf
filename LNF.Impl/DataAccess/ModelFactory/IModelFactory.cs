using LNF.DataAccess;

namespace LNF.Impl.DataAccess.ModelFactory
{
    public interface IModelFactory
    {
        NHibernate.ISession Session { get; }
        T Create<T>(IDataItem source);
    }
}
