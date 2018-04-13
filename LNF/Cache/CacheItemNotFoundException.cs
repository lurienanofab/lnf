using System;
using System.Linq.Expressions;

namespace LNF.Cache
{
    public class CacheItemNotFoundException<TSource> : ItemNotFoundException<TSource>
    {
        public CacheItemNotFoundException(Expression<Func<TSource, int>> expression, int value) : base(expression, value) { }

        public override string Message => $"{base.Message} in cache";
    }
}
