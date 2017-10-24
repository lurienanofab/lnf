using System;
using System.Linq.Expressions;

namespace LNF
{
    public class ItemNotFoundException<TSource, TProperty> : Exception
    {
        public string TypeName { get; }
        public string PropertyName { get; }
        public TProperty Value { get; }

        public override string Message { get { return string.Format("Unable to find {0} with {1} = {2}.", TypeName, PropertyName, Value); } }

        public ItemNotFoundException(Expression<Func<TSource, TProperty>> expression, TProperty value)
        {
            TypeName = typeof(TSource).Name;

            var body = (MemberExpression)expression.Body;
            PropertyName = body.Member.Name;

            Value = value;
        }
    }

    // Defining a default type for TProperty here because int is used most of the time.
    public class ItemNotFoundException<T> : ItemNotFoundException<T, int>
    {
        public ItemNotFoundException(Expression<Func<T, int>> expression, int value) : base(expression, value) { }
    }
}
