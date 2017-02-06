using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace LNF.Repository
{
    public abstract class QueryBase
    {
        /// <summary>
        /// Convenience method to return a single instance that matches the query, or null if the query returns no results.
        /// </summary>
        public abstract T Result<T>() where T : struct;

        /// <summary>
        /// Execute the update or delete statement.
        /// </summary>
        public abstract int Update();
    }

    public abstract class NamedQuery : QueryBase
    {
        public abstract IList<T> List<T>() where T : IDataItem;
    }

    public abstract class SqlQuery : QueryBase
    {
        public abstract IList<T> List<T>();
        public abstract IList<IDictionary> List();
    }

    public abstract class QueryBuilder
    {
        protected QueryParameters queryParams;

        public QueryBuilder()
        {
            queryParams = QueryParameters.Create();
        }

        public abstract NamedQuery NamedQuery(string name);

        public abstract SqlQuery SqlQuery(string sql);

        public QueryBuilder ApplyParameters(object obj)
        {
            if (obj != null)
            {
                queryParams.AddRange(obj.GetType()
                    .GetProperties()
                    .Select(p => new QueryParameter(p.Name, p.GetValue(obj, null))));
            }

            return this;
        }

        public QueryBuilder ApplyParameters(IDictionary<string, object> dict)
        {
            if (dict != null)
                queryParams.AddRange(dict.Select(kvp => new QueryParameter(kvp.Key, kvp.Value)));
            return this;
        }

        public QueryBuilder ApplyParameters(QueryParameters queryParams)
        {
            this.queryParams.AddRange(queryParams);
            return this;
        }

        public QueryBuilder AddParameter<T>(string name, T value)
        {
            queryParams.AddParameter(name, value);
            return this;
        }

        public QueryBuilder AddParameterIf<T>(string name, bool condition, T value)
        {
            queryParams.AddParameterIf(name, condition, value);
            return this;
        }

        public QueryBuilder AddParameterIf<T>(string name, bool condition, T trueValue, object falseValue)
        {
            if (condition)
                queryParams.AddParameter(name, trueValue);
            else
            {
                if (falseValue == null)
                    queryParams.AddParameter(new QueryParameter(name, typeof(T)));
                else
                    queryParams.AddParameter(name, (T)falseValue);
            }

            return this;
        }
    }
}
