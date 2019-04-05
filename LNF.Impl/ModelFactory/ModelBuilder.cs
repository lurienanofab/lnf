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

        protected T MapFrom<T>(object source) where T : new()
        {
            var result = new T();
            result.InjectFrom(source);
            return result;
        }

        private TResult MapFrom<TProxy, TResult>(object id)
            where TResult : new()
            where TProxy : IDataItem
        {
            var result = new TResult();
            TProxy source = Session.Single<TProxy>(id);
            result.InjectFrom(source);
            return result;
        }

        protected void Map<TSource, TResult>(Func<TSource, TResult> fn) => Mapper.AddMap(fn);

        protected void Map<TSource, TConcrete, TResult>() where TConcrete : TResult, new()
        {
            Map<TSource, TResult>(x => MapFrom<TConcrete>(x));
        }

        protected void Map<TSource, TProxy, TConcrete, TResult>(Func<TSource, object> id)
            where TProxy : IDataItem
            where TConcrete : TResult, new()
        {
            Map<TSource, TResult>(x => MapFrom<TProxy, TConcrete>(id(x)));
        }

        public abstract void AddMaps();
    }
}
