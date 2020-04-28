using System;
using System.Linq.Expressions;

namespace LNF
{
    public class ItemNotFoundException : Exception
    {
        public string TypeName { get; }
        public string PropertyMessage { get; }

        public override string Message => $"Unable to find {TypeName} with {PropertyMessage}.";

        public ItemNotFoundException(string typeName, string propertyMessage)
        {
            TypeName = typeName;
            PropertyMessage = propertyMessage;
        }

        public ItemNotFoundException(string typeName, string idName, object idValue) : this(typeName, $"{idName}: {idValue}") { }

        public ItemNotFoundException(string typeName, object id) : this(typeName, "id", id) { }

        public ItemNotFoundException(Type t, object id) : this(t.Name, id) { }
    }

    public class ItemNotFoundException<TSource, TProperty> : ItemNotFoundException
    {
        public ItemNotFoundException(Expression<Func<TSource, TProperty>> expression, TProperty value)
            : base(typeof(TSource).Name, GetPropertyName(expression), value) { }

        private static string GetPropertyName(Expression<Func<TSource, TProperty>> expression)
        {
            var body = (MemberExpression)expression.Body;
            return body.Member.Name;
        }
    }

    // Defining a default type for TProperty here because int is used most of the time.
    public class ItemNotFoundException<T> : ItemNotFoundException<T, int>
    {
        public ItemNotFoundException(Expression<Func<T, int>> expression, int value) : base(expression, value) { }
    }
}
