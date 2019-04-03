using LNF.Impl.ModelFactory.Injections;
using LNF.Repository;
using Omu.ValueInjecter;
using Omu.ValueInjecter.Injections;

namespace LNF.Impl.ModelFactory
{
    public class ValueInjecterModelFactory : IModelFactory
    {
        protected ISession Session { get; }

        public ValueInjecterModelFactory(IDataAccessService service)
        {
            // xxxxx Data Maps
            var data = new DataModelBuilder(service.Session);
            data.AddMaps();

            // xxxxx Scheduler Maps
            var scheduler = new SchedulerModelBuilder(service.Session);
            scheduler.AddMaps();

            // xxxxx Ordering Maps
            var ordering = new OrderingModelBuilder(service.Session);
            ordering.AddMaps();

            // xxxxx Billing Maps
            var billing = new BillingModelBuilder(service.Session);
            billing.AddMaps();

            Session = service.Session;
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

            // need to unproxy otherwise Mapper won't recognize the type (nhibernate proxy class)
            T result = Mapper.Map<T>(Session.Unproxy(source));

            foreach (var injection in _injections)
            {
                result.InjectFrom(injection, source);
            }

            return result;
        }
    }
}
