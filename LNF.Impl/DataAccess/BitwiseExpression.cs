using NHibernate;
using NHibernate.Criterion;
using NHibernate.Impl;
using NHibernate.Type;
using System;
using System.Linq.Expressions;

namespace LNF.Impl.DataAccess
{
    public class BitwiseExpression
    {
        public static BitwiseExpressionBuilder On<T>(Expression<Func<T, object>> expression)
        {
            string propertyName = ExpressionProcessor.FindMemberExpression(expression.Body);
            return On(propertyName);
        }

        public static BitwiseExpressionBuilder On(string propertyName)
        {
            return new BitwiseExpressionBuilder(propertyName);
        }
    }

    public class BitwiseExpressionBuilder
    {
        private string _propertyName;

        internal BitwiseExpressionBuilder(string propertyName)
        {
            _propertyName = propertyName;
        }

        public ICriterion HasBit(object value)
        {
            var types = GetTypes(value);
            var columnAliases = new string[] { "BitwiseHasBit" };

            string column = "{alias}." + _propertyName;
            string sql = string.Format("({0} & {1}) as BitwiseHasBit", column, (int)value);

            return Restrictions.Eq(Projections.SqlProjection(sql, columnAliases, types), value);
        }

        public ICriterion HasAny(object value)
        {
            var types = GetTypes(value);
            var columnAliases = new string[] { "BitwiseHasAny" };

            var column = "{alias}." + _propertyName;
            var sql = string.Format("({0} & {1}) as BitwiseHasAny", column, (int)value);

            return Restrictions.Gt(Projections.SqlProjection(sql, columnAliases, types), 0);
        }

        private static IType[] GetTypes(object value)
        {
            var type = NHibernateUtil.Enum(value.GetType());
            return new IType[] { type };
        }
    }
}
