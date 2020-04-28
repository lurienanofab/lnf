using LNF.DataAccess;
using Omu.ValueInjecter;
using System;

namespace LNF.Impl.DataAccess.ModelFactory
{
    public abstract class ModelBuilder
    {
        private readonly ISessionManager _mgr;
        protected NHibernate.ISession Session => _mgr.Session;

        public ModelBuilder(ISessionManager mgr)
        {
            _mgr = mgr;
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
            TProxy source = Session.Get<TProxy>(id);
            result.InjectFrom(source);
            return result;
        }

        protected void Map<TSource, TResult>(Func<TSource, TResult> fn) => Mapper.AddMap(fn);

        protected void Map<TSource, TConcrete, TResult>() where TConcrete : TResult, new()
        {
            Map<TSource, TResult>(x => MapFrom<TConcrete>(x));
        }

        protected void Map<TSource, TProxy, TResult>(Func<TSource, object> id) where TProxy : IDataItem, TResult, new()
        {
            Map<TSource, TResult>(x => Session.Get<TProxy>(id(x)));
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
