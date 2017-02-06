using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Type;
using NHibernate.Engine;
using NHibernate.Criterion;

namespace LNF.Impl
{
    public class BitwiseFlags
    {
        /// <summary>
        /// Performs a bitwise comparison of a flags property and a flags value.
        /// </summary>
        /// <param name="propertyName">A property that contains one or more flags.</param>
        /// <param name="flags">A value that contains one or more flags. True is returned if any match one of the flags in the property.</param>
        /// <returns>NHibernate.Criterion.SimpleExpression</returns>
        public static SimpleExpression Compare(string propertyName, int flags)
        {
            string sql = string.Format("({0} & {1}) as FlagsCompare", propertyName, flags);
            string[] columnAliases = new string[] { "FlagsCompare" };
            IType[] types = new IType[] { NHibernateUtil.Int32 };
            return Restrictions.Gt(Projections.SqlProjection(sql, columnAliases, types), 0);
        }
    }
}
