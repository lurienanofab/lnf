using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace LNF.Cache
{
    public class CacheItemNotFoundException<TSource> : ItemNotFoundException<TSource>
    {
        public CacheItemNotFoundException(Expression<Func<TSource, int>> expression, int value) : base(expression, value) { }

        public override string Message => string.Format("Unable to find {0} with {1} = {2} in cache.", TypeName, PropertyName, Value);
    }
}
