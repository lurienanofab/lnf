using NHibernate.Context;

namespace LNF.Impl.DependencyInjection
{
    public interface IIOC
    {
        IDependencyResolver Resolver { get; }
    }
}
