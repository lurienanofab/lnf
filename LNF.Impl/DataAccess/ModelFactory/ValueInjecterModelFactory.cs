using LNF.DataAccess;
using LNF.Impl.DataAccess.ModelFactory.Injections;
using Omu.ValueInjecter;
using Omu.ValueInjecter.Injections;

namespace LNF.Impl.DataAccess.ModelFactory
{
    public class ValueInjecterModelFactory : IModelFactory
    {
        private readonly ISessionManager _sessionManager;

        public NHibernate.ISession Session => _sessionManager.Session;

        public ValueInjecterModelFactory(ISessionManager mgr)
        {
            _sessionManager = mgr;

            // xxxxx Data Maps
            var data = new DataModelBuilder(mgr);
            data.AddMaps();

            // xxxxx Scheduler Maps
            var scheduler = new SchedulerModelBuilder(mgr);
            scheduler.AddMaps();

            // xxxxx Ordering Maps
            var ordering = new OrderingModelBuilder(mgr);
            ordering.AddMaps();

            // xxxxx Billing Maps
            var billing = new BillingModelBuilder(mgr);
            billing.AddMaps();
        }

        private static readonly IValueInjection[] _injections =
        {
            new ExtendedFlatLoopInjection(),
            new NullableInjection(),
            new NullableUseDefaultInjection()
        };

        public T Create<T>(IDataItem source)
        {
            if (source == null) return default(T);

            if (typeof(T).IsAssignableFrom(source.GetType()))
                return (T)source;

            // need to unproxy otherwise Mapper won't recognize the type (nhibernate proxy class)
            T result = Mapper.Map<T>(NHibernateUtility.Unproxy(Session, source));

            foreach (var injection in _injections)
            {
                result.InjectFrom(injection, source);
            }

            return result;
        }
    }
}
