using LNF.Repository;
using Omu.ValueInjecter;
using System;

namespace LNF.Impl.ModelFactory
{
    public abstract class ModelBuilder
    {
        protected ISession Session { get; }

        public ModelBuilder(ISession session)
        {
            Session = session;
        }

        protected T CreateModelFrom<T>(object source) where T : new()
        {
            var result = new T();
            result.InjectFrom(source);
            return result;
        }

        protected TResult CreateModelFrom<TSource, TResult>(object id)
            where TResult : new()
            where TSource : IDataItem
        {
            var result = new TResult();
            TSource source = Session.Single<TSource>(id);
            result.InjectFrom(source);
            return result;
        }

        protected void Map<TSource, TResult>(Func<TSource, TResult> fn) => Mapper.AddMap(fn);

        public abstract void AddMaps();
    }
}
