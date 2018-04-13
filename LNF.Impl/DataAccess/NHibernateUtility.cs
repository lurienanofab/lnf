using FluentNHibernate.Cfg;
using NHibernate;
using System;
using System.Collections.Generic;

namespace LNF.Impl.DataAccess
{
    public static class NHibernateUtility
    {
        public static IQuery ApplyParameters(this IQuery query, QueryParameters queryParams)
        {
            if (queryParams != null)
            {
                foreach (QueryParameter qp in queryParams)
                {
                    NHibernate.Type.IType type;
                    if (qp.Type.IsAssignableFrom(typeof(bool)))
                        type = NHibernateUtil.Boolean;
                    else
                        type = NHibernateUtil.GuessType(qp.Type);

                    query.SetParameter(qp.Name, qp.Value, type);
                }
            }
            return query;
        }

        public static IList<string> GetConfigExceptionPotentialReasons(this Exception ex)
        {
            if (ex.InnerException != null && ex.InnerException is FluentConfigurationException)
            {
                var inner = (FluentConfigurationException)ex.InnerException;
                return inner.PotentialReasons;
            }
            else
                return null;
        }
    }
}
